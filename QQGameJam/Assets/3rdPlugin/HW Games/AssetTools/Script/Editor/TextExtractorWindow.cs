using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace HWGames.HWEditor.Tool {
    // 配置数据类
    [System.Serializable]
    public class TextExtractorConfig {
        public bool includeUITexts = true;
        public bool includeScriptTexts = true;
        public bool includeComments = false;
        public bool includeTableTexts = false; // 新增：是否包含表格文本
        public string outputPath = "Assets/HW Games/AssetTools/ExtractedTexts.txt";
        public string[] fileExtensions = { ".cs", ".js" };

        // Language表比对配置
        public bool enableLanguageComparison = true;
        public string languageTableName = "language"; // Language表名
        public string languageTablePath = ""; // Language表路径（自动查找）

        // 文件夹选择相关
        public List<string> selectedFolders = new List<string>();
        public bool useDefaultFolders = true;
        public List<string> defaultFolders = new List<string> { "Assets/Resources", "Assets/Scripts" };

        // 表格抓取配置 - 修改为按列名配置
        public string tableDataPath = "Assets/Resources/Refdata"; // 表格数据路径
        public List<string> targetColumnNames = new List<string>(); // 目标列名列表
        public List<string> columnHistory = new List<string>(); // 历史列名

        // 自定义过滤接口
        public bool enableCustomFilter = true;
        public string customFilterText = "";
        public List<string> customFilterList = new List<string>
        {
        "GetChildControl", "CreateSpecificCulture", "Debug.Log", "Debug.LogError",
        "Debug.LogWarning", "Debug.LogFormat", "Debug.LogErrorFormat", "SetSprite",
        "Resources.Load", "GetString", "GetEnum", "GetBool", "GetFloat", "GetDouble",
        "GetInt", "GetLong", "SetEnum", "SetBool", "SetFloat", "SetDouble", "SetInt",
        "SetLong", "GetList", "SetList", "TrackCustomEvent", "TrackPurchaseEvent",
        "TrackFaceBookCustomEvent", "TrackFirebaseCustomEvent", "TrackSingularCustomEvent",
        "ShowRewardAD", "AndroidJavaClass","AddData"
    };
        public bool showCustomFilterList = false;

        // 自定义文本过滤
        public bool enableTextFilter = true;
        public string customTextFilter = "";
        public List<string> customTextFilterList = new List<string>
        {
        "中文", "Deutsch", "Français", "Español (ES)", "Español (AL)",
        "Português (BR)", "Português (PT)", "Italiano", "Nederlands",
        "日本語", "한국어", "Русский", "Українська", "Ελληνικά", "Türk", "English"
    };
        public bool showCustomTextFilterList = false;

        // 脚本类名过滤
        public bool enableClassNameFilter = true;
        public string customClassNameFilter = "";
        public List<string> customClassNameFilterList = new List<string>
        {
        "IAPManager", "LocalSave", "Launch",
    };
        public bool showCustomClassNameFilterList = false;
    }

    // 表格信息数据类
    [System.Serializable]
    public class TableDataInfo {
        public string tableName;
        public List<string> columns = new List<string>();
        public string filePath;

        public TableDataInfo(string name, string path) {
            tableName = name;
            filePath = path;
        }
    }

    public class TextExtractorWindow : EditorWindow {
        private Vector2 mainScrollPosition;
        private Vector2 folderScrollPosition;
        private Vector2 tableScrollPosition;
        private Vector2 filterScrollPosition;
        private Vector2 textFilterScrollPosition;
        private Vector2 classNameFilterScrollPosition;
        private Vector2 columnScrollPosition;
        private Vector2 targetColumnsScrollPosition;
        private List<TextData> extractedTexts = new List<TextData>();

        // 配置实例
        private TextExtractorConfig config = new TextExtractorConfig();

        // 配置文件路径
        private static readonly string CONFIG_FILE_PATH = "Library/TextExtractorConfig.json";

        // 表格抓取相关
        private bool showTableSection = false;
        public bool showCustomFilters = false;
        public bool showCatchFilters = false;
        public bool showCatchOptionFilters = false;
        private List<TableDataInfo> allTables = new List<TableDataInfo>(); // 所有表格信息
        private List<string> allAvailableColumns = new List<string>(); // 所有可用列名
        private string currentColumnInput = "";
        private string columnSearchText = ""; // 列名搜索文本

        private HashSet<string> existingLanguageTexts = new HashSet<string>(); // 存储已存在的语言文本
        private bool languageTableExists = false;
        private string languageTableFullPath = "";

        // 窗口高度控制
        private const float SCROLL_VIEW_MAX_HEIGHT = 300f;
        private const float TABLE_SECTION_MAX_HEIGHT = 300f;
        private const float FILTER_SECTION_MAX_HEIGHT = 120f;

        [System.Serializable]
        public class TextData {
            public string text;
            public string source;
            public string type;
            public string filePath;
            public int lineNumber;

            public TextData(string text, string source, string type, string filePath = "", int lineNumber = 0) {
                this.text = text;
                this.source = source;
                this.type = type;
                this.filePath = filePath;
                this.lineNumber = lineNumber;
            }
        }

        [MenuItem("HW Games/Asset Tools/文本抓取工具", false, 201)]
        public static void ShowWindow() {
            var window = GetWindow<TextExtractorWindow>("文本抓取工具");
            window.minSize = new Vector2(400, 600);
        }

        void OnEnable() {
            LoadConfig();
            RefreshAllTables();
        }

        void OnDisable() {
            SaveConfig();
        }

        void OnGUI() {
            EditorGUI.BeginChangeCheck();

            // 主滚动视图
            mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition);

            GUILayout.Label("Unity 文本抓取工具", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 配置管理按钮
            DrawConfigManagement();

            // 文件夹选择区域
            DrawFolderSelection();

            // 抓取选项
            DrawExtractionOptions();

            // 表格抓取配置
            if (config.includeTableTexts) {
                DrawTableConfiguration();
            }

            // 自定义过滤设置
            DrawCustomFilters();

            // 输出设置
            DrawOutputSettings();

            // 操作按钮
            DrawActionButtons();

            // 显示结果
            DrawResults();

            EditorGUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck()) {
                EditorApplication.delayCall += SaveConfig;
            }
        }

        void DrawConfigManagement() {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("重置配置", GUILayout.Width(80))) {
                if (EditorUtility.DisplayDialog("确认重置", "是否要重置所有配置为默认值？", "确定", "取消")) {
                    ResetConfig();
                    SaveConfig();
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("配置会自动保存", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        void DrawFolderSelection() {
            showCatchFilters = EditorGUILayout.Foldout(showCatchFilters, "抓取范围:", true);
            if (showCatchFilters) {
                config.useDefaultFolders = EditorGUILayout.Toggle("使用默认文件夹 (Resources, Scripts)", config.useDefaultFolders);

                if (config.useDefaultFolders) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("将抓取以下默认文件夹:", EditorStyles.miniLabel);
                    foreach (string folder in config.defaultFolders) {
                        EditorGUILayout.LabelField("• " + folder, EditorStyles.miniLabel);
                    }
                    EditorGUI.indentLevel--;
                }
                else {
                    EditorGUILayout.LabelField("拖拽文件夹到下方区域来选择抓取范围:");

                    Rect dropArea = GUILayoutUtility.GetRect(0.0f, 100.0f, GUILayout.ExpandWidth(true));
                    GUI.Box(dropArea, "将文件夹拖拽到这里\n(支持多个文件夹)", EditorStyles.helpBox);
                    HandleDragAndDrop(dropArea);

                    if (config.selectedFolders.Count > 0) {
                        EditorGUILayout.LabelField($"已选择 {config.selectedFolders.Count} 个文件夹:", EditorStyles.boldLabel);

                        folderScrollPosition = EditorGUILayout.BeginScrollView(folderScrollPosition, GUILayout.MaxHeight(SCROLL_VIEW_MAX_HEIGHT));
                        for (int i = config.selectedFolders.Count - 1; i >= 0; i--) {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("• " + config.selectedFolders[i], EditorStyles.miniLabel);
                            if (GUILayout.Button("移除", GUILayout.Width(50))) {
                                config.selectedFolders.RemoveAt(i);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndScrollView();

                        if (GUILayout.Button("清空所有文件夹")) {
                            config.selectedFolders.Clear();
                        }
                    }
                    else {
                        EditorGUILayout.HelpBox("请拖拽文件夹到上方区域，或启用默认文件夹选项", MessageType.Info);
                    }
                }
            }
            EditorGUILayout.Space();
        }

        void DrawExtractionOptions() {
            showCatchOptionFilters = EditorGUILayout.Foldout(showCatchOptionFilters, "抓取选项:", true);
            if (showCatchOptionFilters) {
                config.includeUITexts = EditorGUILayout.Toggle("包含UI文本 (Text, TextMeshPro)", config.includeUITexts);
                config.includeScriptTexts = EditorGUILayout.Toggle("包含脚本中的字符串", config.includeScriptTexts);
                config.includeComments = EditorGUILayout.Toggle("包含注释", config.includeComments);
                config.includeTableTexts = EditorGUILayout.Toggle("包含表格文本", config.includeTableTexts);

                // 新增：Language表比对选项
                EditorGUILayout.Space();
                config.enableLanguageComparison = EditorGUILayout.Toggle("启用Language表比对过滤", config.enableLanguageComparison);

                if (config.enableLanguageComparison) {
                    EditorGUI.indentLevel++;

                    // 显示Language表状态
                    if (languageTableExists) {
                        EditorGUILayout.LabelField($"✓ 已找到Language表: {languageTableFullPath}", EditorStyles.miniLabel);
                        EditorGUILayout.LabelField($"表中包含 {existingLanguageTexts.Count} 个文本", EditorStyles.miniLabel);
                    }
                    else {
                        EditorGUILayout.LabelField("✗ 未找到Language表，将跳过比对", EditorStyles.miniLabel);
                    }

                    // Language表名配置
                    EditorGUILayout.BeginHorizontal();
                    config.languageTableName = EditorGUILayout.TextField("Language表名:", config.languageTableName);
                    if (GUILayout.Button("重新查找", GUILayout.Width(80))) {
                        CheckLanguageTable();
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.HelpBox("功能说明：\n" +
                        "1. 自动在表格数据路径中查找指定名称的Language表\n" +
                        "2. 如果表存在，读取第一列的所有文本值\n" +
                        "3. 在抓取结果中剔除已存在于Language表第一列的文本\n" +
                        "4. 如果表不存在，则正常进行抓取不做过滤", MessageType.Info);

                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.Space();
        }

        // 新增：检查Language表是否存在
        void CheckLanguageTable() {
            existingLanguageTexts.Clear();
            languageTableExists = false;
            languageTableFullPath = "";

            if (string.IsNullOrEmpty(config.tableDataPath) || !Directory.Exists(config.tableDataPath)) {
                Debug.Log("表格数据路径无效，无法查找Language表");
                return;
            }

            try {
                // 查找Language表文件
                string[] searchPatterns = {
            $"{config.languageTableName}.txt",
            $"{config.languageTableName.ToLower()}.txt",
            $"{config.languageTableName.ToUpper()}.txt"
        };

                foreach (string pattern in searchPatterns) {
                    string[] foundFiles = Directory.GetFiles(config.tableDataPath, pattern, SearchOption.AllDirectories);
                    if (foundFiles.Length > 0) {
                        languageTableFullPath = foundFiles[0];
                        languageTableExists = true;
                        LoadLanguageTableTexts(languageTableFullPath);
                        Debug.Log($"找到Language表: {languageTableFullPath}，包含 {existingLanguageTexts.Count} 个文本");
                        return;
                    }
                }

                Debug.Log($"未找到名为 '{config.languageTableName}' 的表格文件");
            }
            catch (System.Exception e) {
                Debug.LogError($"查找Language表时出错: {e.Message}");
            }
        }

        // 新增：加载Language表第一列的文本
        void LoadLanguageTableTexts(string filePath) {
            try {
                if (!File.Exists(filePath)) {
                    return;
                }

                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length < 3) { // 至少需要标题行、列名行和一行数据
                    Debug.LogWarning($"Language表 {filePath} 格式不正确，行数不足");
                    return;
                }

                // 检测分隔符
                string headerLine = lines[1]; // 第二行是列名行
                char separator = headerLine.Contains('\t') ? '\t' : ',';

                // 从第三行开始读取数据（跳过标题行和列名行）
                for (int i = 2; i < lines.Length; i++) {
                    string line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] cells = line.Split(separator);
                    if (cells.Length > 0) {
                        string firstColumnText = cells[0].Trim();

                        // 移除可能的引号
                        if (firstColumnText.StartsWith("\"") && firstColumnText.EndsWith("\"")) {
                            firstColumnText = firstColumnText.Substring(1, firstColumnText.Length - 2);
                        }

                        // 只添加有效的文本
                        if (!string.IsNullOrWhiteSpace(firstColumnText)) {
                            existingLanguageTexts.Add(firstColumnText);
                        }
                    }
                }

                Debug.Log($"从Language表加载了 {existingLanguageTexts.Count} 个文本用于比对");
            }
            catch (System.Exception e) {
                Debug.LogError($"加载Language表文本时出错: {e.Message}");
            }
        }

        void DrawTableConfiguration() {
            showTableSection = EditorGUILayout.Foldout(showTableSection, "表格抓取配置", true);

            if (showTableSection) {
                EditorGUI.indentLevel++;

                // 表格数据路径设置
                EditorGUILayout.BeginHorizontal();
                config.tableDataPath = EditorGUILayout.TextField("表格数据路径:", config.tableDataPath);
                if (GUILayout.Button("浏览", GUILayout.Width(60))) {
                    string path = EditorUtility.OpenFolderPanel("选择表格数据文件夹", config.tableDataPath, "");
                    if (!string.IsNullOrEmpty(path)) {
                        if (path.StartsWith(Application.dataPath)) {
                            config.tableDataPath = "Assets" + path.Substring(Application.dataPath.Length);
                        }
                        else {
                            config.tableDataPath = path;
                        }
                        RefreshAllTables();
                    }
                }
                if (GUILayout.Button("刷新", GUILayout.Width(60))) {
                    RefreshAllTables();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                // 显示表格统计信息
                DrawTableStatistics();

                // 目标列名配置
                DrawTargetColumnsConfiguration();

                // 显示所有可用列名
                DrawAllAvailableColumns();

                EditorGUILayout.HelpBox("输入列名来匹配所有表格中包含该列的数据\n支持多个列名，会自动搜索所有表格", MessageType.Info);

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
        }

        void DrawTableStatistics() {
            if (allTables.Count > 0) {
                EditorGUILayout.LabelField($"已扫描表格数量: {allTables.Count}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"所有可用列数量: {allAvailableColumns.Count}", EditorStyles.boldLabel);
                EditorGUILayout.Space();
            }
        }

        void DrawTargetColumnsConfiguration() {
            EditorGUILayout.LabelField("目标列名配置:", EditorStyles.boldLabel);

            // 输入新列名
            EditorGUILayout.BeginHorizontal();
            currentColumnInput = EditorGUILayout.TextField("列名:", currentColumnInput);

            // 下拉菜单显示可用列名
            if (allAvailableColumns.Count > 0 && GUILayout.Button("▼", GUILayout.Width(25))) {
                GenericMenu menu = new GenericMenu();

                // 如果有搜索文本，优先显示匹配的结果
                List<string> menuColumns = allAvailableColumns;
                if (!string.IsNullOrEmpty(currentColumnInput)) {
                    var filteredColumns = allAvailableColumns
                        .Where(col => col.ToLower().Contains(currentColumnInput.ToLower()))
                        .ToList();
                    if (filteredColumns.Count > 0) {
                        menuColumns = filteredColumns;
                    }
                }

                foreach (string column in menuColumns.Take(20)) { // 限制显示数量避免菜单太长
                    string columnName = column;
                    menu.AddItem(new GUIContent(columnName), currentColumnInput == columnName, () => {
                        currentColumnInput = columnName;
                    });
                }

                if (menuColumns.Count > 20) {
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent($"... 还有 {menuColumns.Count - 20} 个结果，请使用下方搜索"), false, null);
                }

                menu.ShowAsContext();
            }

            // 添加按钮
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(currentColumnInput));
            if (GUILayout.Button("添加", GUILayout.Width(50))) {
                if (!config.targetColumnNames.Contains(currentColumnInput)) {
                    config.targetColumnNames.Add(currentColumnInput);

                    // 添加到历史记录
                    if (!config.columnHistory.Contains(currentColumnInput)) {
                        config.columnHistory.Add(currentColumnInput);
                    }

                    currentColumnInput = "";
                }
                else {
                    EditorUtility.DisplayDialog("提示", "该列名已存在于目标列表中", "确定");
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            // 显示已选择的目标列名
            if (config.targetColumnNames.Count > 0) {
                EditorGUILayout.LabelField($"目标列名 ({config.targetColumnNames.Count}个):", EditorStyles.boldLabel);

                targetColumnsScrollPosition = EditorGUILayout.BeginScrollView(targetColumnsScrollPosition, GUILayout.MaxHeight(SCROLL_VIEW_MAX_HEIGHT));

                for (int i = config.targetColumnNames.Count - 1; i >= 0; i--) {
                    EditorGUILayout.BeginHorizontal("box");

                    // 显示列名
                    EditorGUILayout.LabelField(config.targetColumnNames[i], EditorStyles.label);

                    // 显示匹配的表格数量
                    int matchCount = GetMatchingTablesCount(config.targetColumnNames[i]);
                    EditorGUILayout.LabelField($"({matchCount}张表)", EditorStyles.miniLabel, GUILayout.Width(60));

                    // 删除按钮
                    if (GUILayout.Button("删除", GUILayout.Width(50))) {
                        config.targetColumnNames.RemoveAt(i);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("清空所有", GUILayout.Width(80))) {
                    config.targetColumnNames.Clear();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }

        void DrawAllAvailableColumns() {
            if (allAvailableColumns.Count > 0) {
                EditorGUILayout.LabelField($"所有可用列名 ({allAvailableColumns.Count}个):", EditorStyles.boldLabel);

                // 添加搜索框
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("搜索列名:", GUILayout.Width(80));
                string newSearchText = EditorGUILayout.TextField(columnSearchText);
                if (newSearchText != columnSearchText) {
                    columnSearchText = newSearchText;
                }
                if (GUILayout.Button("清空", GUILayout.Width(50))) {
                    columnSearchText = "";
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                // 过滤列名
                List<string> filteredColumns = allAvailableColumns;
                if (!string.IsNullOrEmpty(columnSearchText)) {
                    filteredColumns = allAvailableColumns
                        .Where(col => col.ToLower().Contains(columnSearchText.ToLower()))
                        .ToList();

                    EditorGUILayout.LabelField($"搜索结果: {filteredColumns.Count} 个匹配项", EditorStyles.miniLabel);
                }

                if (filteredColumns.Count > 0) {
                    columnScrollPosition = EditorGUILayout.BeginScrollView(columnScrollPosition, GUILayout.MaxHeight(SCROLL_VIEW_MAX_HEIGHT));

                    // 按多列显示
                    int columnsPerRow = 3;
                    for (int i = 0; i < filteredColumns.Count; i += columnsPerRow) {
                        EditorGUILayout.BeginHorizontal();
                        for (int j = i; j < Mathf.Min(i + columnsPerRow, filteredColumns.Count); j++) {
                            string columnName = filteredColumns[j];
                            bool isSelected = config.targetColumnNames.Contains(columnName);

                            if (isSelected) GUI.backgroundColor = Color.green;

                            if (GUILayout.Button(columnName, GUILayout.MinWidth(80), GUILayout.MaxWidth(150))) {
                                if (!isSelected) {
                                    config.targetColumnNames.Add(columnName);
                                    if (!config.columnHistory.Contains(columnName)) {
                                        config.columnHistory.Add(columnName);
                                    }
                                }
                            }

                            if (isSelected) GUI.backgroundColor = Color.white;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();
                }
                else if (!string.IsNullOrEmpty(columnSearchText)) {
                    EditorGUILayout.HelpBox("没有找到匹配的列名", MessageType.Info);
                }
            }
        }

        int GetMatchingTablesCount(string columnName) {
            int count = 0;
            foreach (var table in allTables) {
                if (table.columns.Any(col => string.Equals(col, columnName, System.StringComparison.OrdinalIgnoreCase))) {
                    count++;
                }
            }
            return count;
        }

        void DrawCustomFilters() {
            showCustomFilters = EditorGUILayout.Foldout(showCustomFilters, "自定义过滤", true);
            if (!showCustomFilters) {
                EditorGUILayout.Space();
                return;
            }
            config.enableCustomFilter = EditorGUILayout.Toggle("启用自定义接口过滤", config.enableCustomFilter);

            if (config.enableCustomFilter) {
                EditorGUILayout.BeginHorizontal();
                config.customFilterText = EditorGUILayout.TextField("过滤接口:", config.customFilterText);
                if (GUILayout.Button("添加", GUILayout.Width(50))) {
                    if (!string.IsNullOrWhiteSpace(config.customFilterText) && !config.customFilterList.Contains(config.customFilterText)) {
                        config.customFilterList.Add(config.customFilterText);
                        config.customFilterText = "";
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                config.showCustomFilterList = EditorGUILayout.Foldout(config.showCustomFilterList, $"当前过滤接口 ({config.customFilterList.Count}个)", true);
                if (GUILayout.Button("清空所有", GUILayout.Width(80))) {
                    config.customFilterList.Clear();
                }
                EditorGUILayout.EndHorizontal();

                if (config.showCustomFilterList && config.customFilterList.Count > 0) {
                    EditorGUI.indentLevel++;
                    filterScrollPosition = EditorGUILayout.BeginScrollView(filterScrollPosition, GUILayout.MaxHeight(FILTER_SECTION_MAX_HEIGHT));

                    for (int i = config.customFilterList.Count - 1; i >= 0; i--) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("• " + config.customFilterList[i], EditorStyles.miniLabel);
                        if (GUILayout.Button("删除", GUILayout.Width(50))) {
                            config.customFilterList.RemoveAt(i);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.HelpBox("将过滤包含指定接口调用的代码行中的字符串\n注意: GetLangDesc方法会被自动过滤", MessageType.Info);
            }

            EditorGUILayout.Space();

            // 自定义文本过滤
            config.enableTextFilter = EditorGUILayout.Toggle("启用自定义文本过滤", config.enableTextFilter);

            if (config.enableTextFilter) {
                EditorGUILayout.BeginHorizontal();
                config.customTextFilter = EditorGUILayout.TextField("过滤文本:", config.customTextFilter);
                if (GUILayout.Button("添加", GUILayout.Width(50))) {
                    if (!string.IsNullOrWhiteSpace(config.customTextFilter) && !config.customTextFilterList.Contains(config.customTextFilter)) {
                        config.customTextFilterList.Add(config.customTextFilter);
                        config.customTextFilter = "";
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                config.showCustomTextFilterList = EditorGUILayout.Foldout(config.showCustomTextFilterList, $"当前过滤文本 ({config.customTextFilterList.Count}个)", true);
                if (GUILayout.Button("清空所有", GUILayout.Width(80))) {
                    config.customTextFilterList.Clear();
                }
                if (GUILayout.Button("添加默认语言", GUILayout.Width(100))) {
                    AddDefaultLanguageTexts();
                }
                EditorGUILayout.EndHorizontal();

                if (config.showCustomTextFilterList && config.customTextFilterList.Count > 0) {
                    EditorGUI.indentLevel++;
                    textFilterScrollPosition = EditorGUILayout.BeginScrollView(textFilterScrollPosition, GUILayout.MaxHeight(FILTER_SECTION_MAX_HEIGHT));

                    for (int i = config.customTextFilterList.Count - 1; i >= 0; i--) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("• " + config.customTextFilterList[i], EditorStyles.miniLabel);
                        if (GUILayout.Button("删除", GUILayout.Width(50))) {
                            config.customTextFilterList.RemoveAt(i);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.HelpBox("直接过滤指定的文本内容，无论出现在哪里\n示例: \"中文\", \"English\", \"Français\"", MessageType.Info);
            }

            EditorGUILayout.Space();

            // 脚本类名过滤
            config.enableClassNameFilter = EditorGUILayout.Toggle("启用脚本类名过滤", config.enableClassNameFilter);

            if (config.enableClassNameFilter) {
                EditorGUILayout.BeginHorizontal();
                config.customClassNameFilter = EditorGUILayout.TextField("过滤类名:", config.customClassNameFilter);
                if (GUILayout.Button("添加", GUILayout.Width(50))) {
                    if (!string.IsNullOrWhiteSpace(config.customClassNameFilter) && !config.customClassNameFilterList.Contains(config.customClassNameFilter)) {
                        config.customClassNameFilterList.Add(config.customClassNameFilter);
                        config.customClassNameFilter = "";
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                config.showCustomClassNameFilterList = EditorGUILayout.Foldout(config.showCustomClassNameFilterList, $"当前过滤类名 ({config.customClassNameFilterList.Count}个)", true);
                if (GUILayout.Button("清空所有", GUILayout.Width(80))) {
                    config.customClassNameFilterList.Clear();
                }
                if (GUILayout.Button("添加常用类名", GUILayout.Width(100))) {
                    AddDefaultClassNames();
                }
                EditorGUILayout.EndHorizontal();

                if (config.showCustomClassNameFilterList && config.customClassNameFilterList.Count > 0) {
                    EditorGUI.indentLevel++;
                    classNameFilterScrollPosition = EditorGUILayout.BeginScrollView(classNameFilterScrollPosition, GUILayout.MaxHeight(FILTER_SECTION_MAX_HEIGHT));

                    for (int i = config.customClassNameFilterList.Count - 1; i >= 0; i--) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("• " + config.customClassNameFilterList[i], EditorStyles.miniLabel);
                        if (GUILayout.Button("删除", GUILayout.Width(50))) {
                            config.customClassNameFilterList.RemoveAt(i);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.HelpBox("过滤指定类名的脚本文件，不会从这些脚本中提取任何文本\n示例: \"UIManager\", \"GameManager\", \"PlayerController\"", MessageType.Info);
            }

            EditorGUILayout.Space();
        }

        void DrawOutputSettings() {
            GUILayout.Label("输出设置:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            config.outputPath = EditorGUILayout.TextField("输出路径:", config.outputPath);
            if (GUILayout.Button("浏览", GUILayout.Width(60))) {
                string path = EditorUtility.SaveFilePanel("保存文本文件", "Assets", "ExtractedTexts", "txt");
                if (!string.IsNullOrEmpty(path)) {
                    config.outputPath = path;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        void DrawActionButtons() {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("开始抓取", GUILayout.Height(30))) {
                if (CanStartExtraction()) {
                    ExtractAllTexts();
                }
            }
            if (GUILayout.Button("导出到文件", GUILayout.Height(30))) {
                ExportToFile();
            }
            if (GUILayout.Button("清空结果", GUILayout.Height(30))) {
                extractedTexts.Clear();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private Vector2 resultScrollPosition;

        void DrawResults() {
            if (extractedTexts.Count > 0) {
                GUILayout.Label($"找到 {extractedTexts.Count} 个文本:", EditorStyles.boldLabel);

                resultScrollPosition = EditorGUILayout.BeginScrollView(resultScrollPosition, GUILayout.MaxHeight(SCROLL_VIEW_MAX_HEIGHT));
                var uniqueTexts = extractedTexts.Select(t => t.text).Distinct().ToList();

                foreach (string text in uniqueTexts) {
                    EditorGUILayout.SelectableLabel(text, GUILayout.MinHeight(18));
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.Space();
                GUILayout.Label($"去重后文本数量: {uniqueTexts.Count}", EditorStyles.miniLabel);
            }
        }

        #region 表格处理相关方法

        void RefreshAllTables() {
            allTables.Clear();
            allAvailableColumns.Clear();

            if (string.IsNullOrEmpty(config.tableDataPath) || !Directory.Exists(config.tableDataPath)) {
                return;
            }

            try {
                string[] txtFiles = Directory.GetFiles(config.tableDataPath, "*.txt", SearchOption.AllDirectories);

                foreach (string file in txtFiles) {
                    string fileName = Path.GetFileNameWithoutExtension(file);

                    if (fileName.Equals("数据表目录", System.StringComparison.OrdinalIgnoreCase)) {
                        continue;
                    }

                    TableDataInfo tableInfo = new TableDataInfo(fileName, file);
                    LoadTableColumns(tableInfo);

                    if (tableInfo.columns.Count > 0) {
                        allTables.Add(tableInfo);

                        // 收集所有唯一的列名
                        foreach (string column in tableInfo.columns) {
                            if (!allAvailableColumns.Contains(column)) {
                                allAvailableColumns.Add(column);
                            }
                        }
                    }
                }

                allAvailableColumns.Sort();
                Debug.Log($"扫描完成: {allTables.Count} 张表格, {allAvailableColumns.Count} 个唯一列名");

                // 检查Language表
                if (config.enableLanguageComparison) {
                    CheckLanguageTable();
                }
            }
            catch (System.Exception e) {
                Debug.LogError($"刷新所有表格时出错: {e.Message}");
            }
        }

        void LoadTableColumns(TableDataInfo tableInfo) {
            try {
                if (!File.Exists(tableInfo.filePath)) {
                    return;
                }

                string[] lines = File.ReadAllLines(tableInfo.filePath);
                if (lines.Length >= 2) {
                    string headerLine = lines[1];
                    char separator = headerLine.Contains('\t') ? '\t' : ',';

                    string[] headers = headerLine.Split(separator);

                    foreach (string header in headers) {
                        string cleanHeader = header.Trim();
                        if (!string.IsNullOrEmpty(cleanHeader)) {
                            tableInfo.columns.Add(cleanHeader);
                        }
                    }
                }
            }
            catch (System.Exception e) {
                Debug.LogError($"加载表格 {tableInfo.tableName} 的列信息时出错: {e.Message}");
            }
        }

        void ExtractTableTexts() {
            foreach (string targetColumn in config.targetColumnNames) {
                ExtractTextsByColumnName(targetColumn);
            }
        }

        void ExtractTextsByColumnName(string columnName) {
            if (string.IsNullOrEmpty(columnName)) {
                return;
            }

            int totalExtracted = 0;
            int matchingTables = 0;

            foreach (var table in allTables) {
                // 检查该表格是否包含目标列
                string matchingColumn = table.columns.FirstOrDefault(col =>
                    string.Equals(col, columnName, System.StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(matchingColumn)) {
                    matchingTables++;
                    int extracted = ExtractSingleTableColumn(table, matchingColumn);
                    totalExtracted += extracted;
                }
            }

            Debug.Log($"列名 '{columnName}' 在 {matchingTables} 张表格中找到，共提取 {totalExtracted} 个文本");
        }

        int ExtractSingleTableColumn(TableDataInfo table, string columnName) {
            try {
                if (!File.Exists(table.filePath)) {
                    return 0;
                }

                string[] lines = File.ReadAllLines(table.filePath);
                if (lines.Length < 3) {
                    return 0;
                }

                string headerLine = lines[1];
                char separator = headerLine.Contains('\t') ? '\t' : ',';

                string[] headers = headerLine.Split(separator);
                int targetColumnIndex = -1;

                for (int i = 0; i < headers.Length; i++) {
                    if (string.Equals(headers[i].Trim(), columnName, System.StringComparison.OrdinalIgnoreCase)) {
                        targetColumnIndex = i;
                        break;
                    }
                }

                if (targetColumnIndex == -1) {
                    return 0;
                }

                int extractedCount = 0;
                for (int lineIndex = 2; lineIndex < lines.Length; lineIndex++) {
                    string line = lines[lineIndex];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] cells = line.Split(separator);
                    if (targetColumnIndex < cells.Length) {
                        string cellText = cells[targetColumnIndex].Trim();

                        if (cellText.StartsWith("\"") && cellText.EndsWith("\"")) {
                            cellText = cellText.Substring(1, cellText.Length - 2);
                        }

                        if (IsValidTableText(cellText)) {
                            extractedTexts.Add(new TextData(
                                cellText,
                                $"{table.tableName}.{columnName}",
                                "表格文本",
                                table.filePath,
                                lineIndex + 1
                            ));
                            extractedCount++;
                        }
                    }
                }

                return extractedCount;
            }
            catch (System.Exception e) {
                Debug.LogError($"提取表格 {table.tableName} 的 {columnName} 列时出错: {e.Message}");
                return 0;
            }
        }

        bool IsValidTableText(string text) {
            if (string.IsNullOrWhiteSpace(text) || text.Length <= 1)
                return false;

            if (text.All(c => char.IsLetter(c) || c == '_') && text.Length < 20)
                return false;

            if (config.enableTextFilter && config.customTextFilterList.Count > 0) {
                foreach (string filterText in config.customTextFilterList) {
                    if (text.Trim() == filterText) {
                        return false;
                    }
                }
            }

            bool hasValidContent = false;
            foreach (char c in text) {
                if (char.IsLetter(c) || char.IsDigit(c)) {
                    hasValidContent = true;
                    break;
                }
            }

            if (!hasValidContent)
                return false;

            string trimmedText = text.Trim();

            // 过滤纯数字
            if (Regex.IsMatch(trimmedText, @"^[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$"))
                return false;

            // 过滤数字+单位的格式（时间、数量、货币等）
            if (IsNumericWithUnit(trimmedText))
                return false;

            if (Regex.IsMatch(trimmedText, @"^[A-Za-z]*\d+$") && trimmedText.Length < 10)
                return false;

            if (trimmedText.Length < 4 && Regex.IsMatch(trimmedText, @"^[A-Za-z]+$"))
                return false;

            return true;
        }

        // 检查是否为数字+单位的格式
        bool IsNumericWithUnit(string text) {
            if (string.IsNullOrEmpty(text))
                return false;

            string trimmedText = text.Trim();

            // 时间单位格式：12h, 30m, 7d, 24h, 33m33s, 55m55s 等
            if (Regex.IsMatch(trimmedText, @"^-?\d+(\.\d+)?[hmdsy]$", RegexOptions.IgnoreCase))
                return true;

            // 复合时间格式：33m33s, 55m55s, 1h30m 等
            if (Regex.IsMatch(trimmedText, @"^-?\d+[hmd]\d+[ms]$", RegexOptions.IgnoreCase))
                return true;

            // 数量单位格式：16.44k, 333.33k, 999.99ab, 99999m, 9999M 等
            if (Regex.IsMatch(trimmedText, @"^-?\d+(\.\d+)?[kmgtpKMGTPabcdefABCDEF]+$"))
                return true;

            // 范围格式：3-4, 1-10 等（纯数字范围）
            if (Regex.IsMatch(trimmedText, @"^\d+-\d+$"))
                return true;

            // 特殊游戏格式：CEO cards（数字+空格+单词）
            if (Regex.IsMatch(trimmedText, @"^\d+\s+(card|cards|coin|coins|gem|gems|gold|silver|bronze|point|points|star|stars|level|lvl|exp|xp)s?$", RegexOptions.IgnoreCase))
                return true;

            // 百分比格式
            if (Regex.IsMatch(trimmedText, @"^-?\d+(\.\d+)?%$"))
                return true;

            // 货币符号格式：$100, €50, ¥1000 等
            if (Regex.IsMatch(trimmedText, @"^[$€¥£₹₽₩₪₫₨₦₱₡₴₸₵₲₴₼₺₾₿]+\d+(\.\d+)?$"))
                return true;
            if (Regex.IsMatch(trimmedText, @"^\d+(\.\d+)?[$€¥£₹₽₩₪₫₨₦₱₡₴₸₵₲₴₼₺₾₿]+$"))
                return true;

            // 版本号格式：1.0, 2.1.3, v1.2 等
            if (Regex.IsMatch(trimmedText, @"^v?\d+(\.\d+)*$", RegexOptions.IgnoreCase))
                return true;

            // 倍数格式：2x, 10X, 0.5x 等
            if (Regex.IsMatch(trimmedText, @"^\d+(\.\d+)?[xX]$"))
                return true;

            // 温度格式：25°C, 77°F 等
            if (Regex.IsMatch(trimmedText, @"^-?\d+(\.\d+)?°[CFK]$"))
                return true;

            // 数据大小格式：100MB, 2GB, 1TB 等
            if (Regex.IsMatch(trimmedText, @"^\d+(\.\d+)?(B|KB|MB|GB|TB|PB|b|kb|mb|gb|tb|pb)$", RegexOptions.IgnoreCase))
                return true;

            // 频率格式：60Hz, 120fps 等
            if (Regex.IsMatch(trimmedText, @"^\d+(\.\d+)?(Hz|fps|FPS|rpm|RPM)$"))
                return true;

            // 分辨率格式：1920x1080, 4K 等
            if (Regex.IsMatch(trimmedText, @"^\d+[xX]\d+$"))
                return true;
            if (Regex.IsMatch(trimmedText, @"^\d+[kK]$"))
                return true;

            return false;
        }

        #endregion

        #region 配置保存和加载

        void SaveConfig() {
            try {
                string json = JsonUtility.ToJson(config, true);
                File.WriteAllText(CONFIG_FILE_PATH, json);
            }
            catch (System.Exception e) {
                Debug.LogError($"保存配置失败: {e.Message}");
            }
        }

        void LoadConfig() {
            try {
                if (File.Exists(CONFIG_FILE_PATH)) {
                    string json = File.ReadAllText(CONFIG_FILE_PATH);
                    JsonUtility.FromJsonOverwrite(json, config);
                }
            }
            catch (System.Exception e) {
                Debug.LogError($"加载配置失败: {e.Message}");
                ResetConfig();
            }
        }

        void ResetConfig() {
            config = new TextExtractorConfig();
        }

        #endregion

        void HandleDragAndDrop(Rect dropArea) {
            Event evt = Event.current;

            switch (evt.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();

                        foreach (string draggedPath in DragAndDrop.paths) {
                            if (AssetDatabase.IsValidFolder(draggedPath)) {
                                if (!config.selectedFolders.Contains(draggedPath)) {
                                    config.selectedFolders.Add(draggedPath);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        bool CanStartExtraction() {
            if (config.includeTableTexts && config.targetColumnNames.Count == 0) {
                EditorUtility.DisplayDialog("提示", "启用表格抓取时，请至少添加一个目标列名。", "确定");
                return false;
            }

            if (!config.includeTableTexts && config.useDefaultFolders) {
                return true;
            }

            if (!config.includeTableTexts && config.selectedFolders.Count == 0) {
                EditorUtility.DisplayDialog("提示", "请先选择要抓取的文件夹，或启用默认文件夹选项，或启用表格抓取。", "确定");
                return false;
            }

            return true;
        }

        void ExtractAllTexts() {
            extractedTexts.Clear();

            // 如果启用了Language表比对，先检查并加载Language表
            if (config.enableLanguageComparison) {
                CheckLanguageTable();
            }

            if (config.includeTableTexts) {
                ExtractTableTexts();
            }

            List<string> targetFolders = GetTargetFolders();

            if (targetFolders.Count == 0 && !config.includeTableTexts) {
                Debug.LogWarning("没有有效的文件夹用于抓取，且未启用表格抓取");
                return;
            }

            if (config.includeUITexts && targetFolders.Count > 0) {
                ExtractUITexts(targetFolders);
            }

            if (config.includeScriptTexts && targetFolders.Count > 0) {
                ExtractScriptTexts(targetFolders);
            }

            extractedTexts = extractedTexts.OrderBy(t => t.type).ThenBy(t => t.source).ToList();

            string extractionInfo = "";
            if (config.includeTableTexts) {
                extractionInfo += $"列名: {string.Join(", ", config.targetColumnNames)}";
            }
            if (targetFolders.Count > 0) {
                if (!string.IsNullOrEmpty(extractionInfo)) extractionInfo += ", ";
                extractionInfo += $"文件夹: {string.Join(", ", targetFolders)}";
            }

            // 添加Language表比对信息
            string languageInfo = "";
            if (config.enableLanguageComparison) {
                if (languageTableExists) {
                    languageInfo = $", 已过滤Language表中的 {existingLanguageTexts.Count} 个文本";
                }
                else {
                    languageInfo = ", Language表未找到";
                }
            }

            Debug.Log($"文本抓取完成! 共找到 {extractedTexts.Count} 个文本，抓取范围: {extractionInfo}{languageInfo}");
        }

        List<string> GetTargetFolders() {
            if (config.useDefaultFolders) {
                return config.defaultFolders.Where(folder => AssetDatabase.IsValidFolder(folder)).ToList();
            }
            else {
                return config.selectedFolders.Where(folder => AssetDatabase.IsValidFolder(folder)).ToList();
            }
        }

        void ExtractUITexts(List<string> targetFolders) {
            Text[] sceneTexts = FindObjectsOfType<Text>();
            foreach (Text text in sceneTexts) {
                if (IsValidText(text.text)) {
                    extractedTexts.Add(new TextData(
                        text.text,
                        GetGameObjectPath(text.gameObject),
                        "UI Text",
                        GetScenePath(text.gameObject)
                    ));
                }
            }

            TextMeshProUGUI[] tmpTexts = FindObjectsOfType<TextMeshProUGUI>();
            foreach (TextMeshProUGUI tmp in tmpTexts) {
                if (IsValidText(tmp.text)) {
                    extractedTexts.Add(new TextData(
                        tmp.text,
                        GetGameObjectPath(tmp.gameObject),
                        "TextMeshPro",
                        GetScenePath(tmp.gameObject)
                    ));
                }
            }

            ExtractPrefabTexts(targetFolders);
        }

        void ExtractPrefabTexts(List<string> targetFolders) {
            foreach (string folder in targetFolders) {
                string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { folder });

                foreach (string guid in prefabGuids) {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                    if (prefab != null) {
                        Text[] texts = prefab.GetComponentsInChildren<Text>(true);
                        foreach (Text text in texts) {
                            if (IsValidText(text.text)) {
                                extractedTexts.Add(new TextData(
                                    text.text,
                                    GetGameObjectPath(text.gameObject),
                                    "Prefab Text",
                                    path
                                ));
                            }
                        }

                        TextMeshProUGUI[] tmpTexts = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);
                        foreach (TextMeshProUGUI tmp in tmpTexts) {
                            if (IsValidText(tmp.text)) {
                                extractedTexts.Add(new TextData(
                                    tmp.text,
                                    GetGameObjectPath(tmp.gameObject),
                                    "Prefab TextMeshPro",
                                    path
                                ));
                            }
                        }
                    }
                }
            }
        }

        void ExtractScriptTexts(List<string> targetFolders) {
            foreach (string folder in targetFolders) {
                string[] scriptGuids = AssetDatabase.FindAssets("t:Script", new[] { folder });

                foreach (string guid in scriptGuids) {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    string extension = Path.GetExtension(path).ToLower();

                    if (System.Array.Exists(config.fileExtensions, ext => ext == extension)) {
                        ExtractTextsFromFile(path);
                    }
                }
            }
        }

        void ExtractTextsFromFile(string filePath) {
            // 检查是否需要过滤该脚本类名
            if (config.enableClassNameFilter && ShouldFilterByClassName(filePath)) {
                return;
            }

            try {
                string content = File.ReadAllText(filePath);
                string[] lines = File.ReadAllLines(filePath);
                bool insideFilteredMethod = false;
                int braceCount = 0;
                bool insideMultilineComment = false;

                for (int i = 0; i < lines.Length; i++) {
                    string line = lines[i];
                    int lineNumber = i + 1;

                    if (line.Contains("/*")) {
                        insideMultilineComment = true;
                    }
                    if (line.Contains("*/")) {
                        insideMultilineComment = false;
                        if (config.includeComments) {
                            ExtractMultilineComments(line, filePath, lineNumber);
                        }
                        continue;
                    }

                    if (insideMultilineComment) {
                        if (config.includeComments) {
                            ExtractComments(line, filePath, lineNumber);
                        }
                        continue;
                    }

                    if (!insideFilteredMethod && IsMethodToFilter(line)) {
                        insideFilteredMethod = true;
                        braceCount = 0;
                    }

                    if (insideFilteredMethod) {
                        braceCount += CountBraces(line);

                        if (braceCount <= 0) {
                            insideFilteredMethod = false;
                        }

                        continue;
                    }

                    if (config.enableCustomFilter && config.customFilterList.Count > 0) {
                        bool shouldFilter = false;
                        foreach (string filterInterface in config.customFilterList) {
                            if (line.Contains(filterInterface)) {
                                shouldFilter = true;
                                break;
                            }
                        }

                        if (shouldFilter)
                            continue;
                    }

                    ExtractStringLiterals(line, filePath, lineNumber);

                    if (config.includeComments) {
                        ExtractComments(line, filePath, lineNumber);
                    }
                }
            }
            catch (System.Exception e) {
                Debug.LogError($"读取文件 {filePath} 时出错: {e.Message}");
            }
        }

        bool IsMethodToFilter(string line) {
            if (line.Contains("GetLangDesc") && (line.Contains("public") || line.Contains("private") || line.Contains("protected"))) {
                return true;
            }

            return false;
        }

        // 检查是否应该根据类名过滤脚本
        bool ShouldFilterByClassName(string filePath) {
            if (!config.enableClassNameFilter || config.customClassNameFilterList.Count == 0) {
                return false;
            }

            try {
                string content = File.ReadAllText(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                // 方法1：检查文件名是否在过滤列表中
                foreach (string className in config.customClassNameFilterList) {
                    if (string.Equals(fileName, className, System.StringComparison.OrdinalIgnoreCase)) {
                        Debug.Log($"根据文件名过滤脚本: {filePath}");
                        return true;
                    }
                }

                // 方法2：检查脚本内容中的类声明
                foreach (string className in config.customClassNameFilterList) {
                    // 匹配类声明模式：public class ClassName, class ClassName, internal class ClassName 等
                    string classPattern = @"\b(public\s+|private\s+|internal\s+|protected\s+)?(partial\s+)?class\s+" + Regex.Escape(className) + @"\b";
                    if (Regex.IsMatch(content, classPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline)) {
                        Debug.Log($"根据类声明过滤脚本: {filePath} (类名: {className})");
                        return true;
                    }

                    // 匹配MonoBehaviour类声明：class ClassName : MonoBehaviour
                    string monoPattern = @"\bclass\s+" + Regex.Escape(className) + @"\s*:\s*MonoBehaviour\b";
                    if (Regex.IsMatch(content, monoPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline)) {
                        Debug.Log($"根据MonoBehaviour类声明过滤脚本: {filePath} (类名: {className})");
                        return true;
                    }

                    // 匹配ScriptableObject类声明
                    string scriptablePattern = @"\bclass\s+" + Regex.Escape(className) + @"\s*:\s*ScriptableObject\b";
                    if (Regex.IsMatch(content, scriptablePattern, RegexOptions.IgnoreCase | RegexOptions.Multiline)) {
                        Debug.Log($"根据ScriptableObject类声明过滤脚本: {filePath} (类名: {className})");
                        return true;
                    }
                }

                return false;
            }
            catch (System.Exception e) {
                Debug.LogError($"检查脚本类名时出错 {filePath}: {e.Message}");
                return false;
            }
        }

        void AddDefaultClassNames() {
            string[] defaultClassNames = {
            "UIManager", "GameManager", "PlayerController", "CameraController",
            "AudioManager", "DataManager", "SceneManager", "NetworkManager",
            "InputManager", "TimeManager", "ConfigManager", "SaveManager",
            "EventManager", "PoolManager", "ResourceManager", "LocalizationManager",
            "InventoryManager", "QuestManager", "DialogueManager", "MenuManager",
            "Singleton", "MonoSingleton", "DontDestroyOnLoad", "GameController",
            "LevelManager", "SpawnManager", "EnemyController", "NPCController",
            "WeaponController", "AbilityManager", "SkillManager", "BuffManager"
        };

            foreach (string className in defaultClassNames) {
                if (!config.customClassNameFilterList.Contains(className)) {
                    config.customClassNameFilterList.Add(className);
                }
            }
        }

        int CountBraces(string line) {
            int count = 0;
            bool inString = false;
            bool inChar = false;
            bool escaped = false;

            for (int i = 0; i < line.Length; i++) {
                char c = line[i];

                if (escaped) {
                    escaped = false;
                    continue;
                }

                if (c == '\\') {
                    escaped = true;
                    continue;
                }

                if (c == '"' && !inChar) {
                    inString = !inString;
                }
                else if (c == '\'' && !inString) {
                    inChar = !inChar;
                }
                else if (!inString && !inChar) {
                    if (c == '{')
                        count++;
                    else if (c == '}')
                        count--;
                }
            }

            return count;
        }

        void ExtractStringLiterals(string line, string filePath, int lineNumber) {
            Regex stringRegex = new Regex(@"""([^""\\]|\\.)*""");
            MatchCollection matches = stringRegex.Matches(line);

            foreach (Match match in matches) {
                string text = match.Value.Trim('"');
                if (IsValidText(text)) {
                    extractedTexts.Add(new TextData(
                        text,
                        Path.GetFileName(filePath),
                        "脚本字符串",
                        filePath,
                        lineNumber
                    ));
                }
            }

            Regex charRegex = new Regex(@"'([^'\\]|\\.)*'");
            MatchCollection charMatches = charRegex.Matches(line);

            foreach (Match match in charMatches) {
                string text = match.Value.Trim('\'');
                if (IsValidText(text)) {
                    extractedTexts.Add(new TextData(
                        text,
                        Path.GetFileName(filePath),
                        "脚本字符",
                        filePath,
                        lineNumber
                    ));
                }
            }
        }

        void ExtractComments(string line, string filePath, int lineNumber) {
            ExtractSingleLineComments(line, filePath, lineNumber);
            ExtractXmlDocComments(line, filePath, lineNumber);
        }

        void ExtractSingleLineComments(string line, string filePath, int lineNumber) {
            if (line.Contains("//")) {
                int commentStart = line.IndexOf("//");
                string comment = line.Substring(commentStart + 2).Trim();

                comment = CleanCommentContent(comment);

                if (IsValidCommentText(comment)) {
                    extractedTexts.Add(new TextData(
                        comment,
                        Path.GetFileName(filePath),
                        "单行注释",
                        filePath,
                        lineNumber
                    ));
                }
            }
        }

        void ExtractXmlDocComments(string line, string filePath, int lineNumber) {
            if (line.TrimStart().StartsWith("///")) {
                string comment = line.Substring(line.IndexOf("///") + 3).Trim();

                string cleanedComment = ExtractXmlTagContent(comment);

                if (IsValidCommentText(cleanedComment)) {
                    extractedTexts.Add(new TextData(
                        cleanedComment,
                        Path.GetFileName(filePath),
                        "XML文档注释",
                        filePath,
                        lineNumber
                    ));
                }
            }
        }

        void ExtractMultilineComments(string line, string filePath, int lineNumber) {
            string comment = "";

            int startIndex = line.IndexOf("/*");
            int endIndex = line.IndexOf("*/");

            if (startIndex != -1 && endIndex != -1 && endIndex > startIndex) {
                comment = line.Substring(startIndex + 2, endIndex - startIndex - 2).Trim();
            }
            else if (startIndex != -1) {
                comment = line.Substring(startIndex + 2).Trim();
            }
            else if (endIndex != -1) {
                comment = line.Substring(0, endIndex).Trim();
            }
            else {
                comment = line.Trim();
                if (comment.StartsWith("*")) {
                    comment = comment.Substring(1).Trim();
                }
            }

            comment = CleanCommentContent(comment);

            if (IsValidCommentText(comment)) {
                extractedTexts.Add(new TextData(
                    comment,
                    Path.GetFileName(filePath),
                    "多行注释",
                    filePath,
                    lineNumber
                ));
            }
        }

        string ExtractXmlTagContent(string xmlComment) {
            if (string.IsNullOrWhiteSpace(xmlComment)) {
                return "";
            }

            string result = Regex.Replace(xmlComment, @"<[^>]*>", " ");
            result = Regex.Replace(result, @"\s+", " ").Trim();

            return result;
        }

        string CleanCommentContent(string comment) {
            if (string.IsNullOrWhiteSpace(comment)) {
                return "";
            }

            comment = comment.TrimStart('/', '*', ' ', '\t');
            comment = comment.TrimEnd('/', '*', ' ', '\t');
            comment = Regex.Replace(comment, @"<[^>]*>", " ");
            comment = Regex.Replace(comment, @"\s+", " ").Trim();
            comment = Regex.Replace(comment, @"""[^""]*""", "");

            return comment;
        }

        bool IsValidCommentText(string comment) {
            if (string.IsNullOrWhiteSpace(comment) || comment.Length <= 2) {
                return false;
            }

            string[] meaninglessPatterns = {
            "param name", "returns", "summary", "remarks", "example",
            "see cref", "paramref name", "typeparam name", "exception cref",
            "value", "c>", "code>", "para>", "/param>", "/returns>",
            "/summary>", "/remarks>", "addTime", "test", "TODO", "FIXME",
            "HACK", "NOTE", "WARNING", "BUG", "DEBUG"
        };

            string lowerComment = comment.ToLower();
            foreach (string pattern in meaninglessPatterns) {
                if (lowerComment.Contains(pattern.ToLower())) {
                    return false;
                }
            }

            if (Regex.IsMatch(comment, @"^[\d\s\-=+*/.,:;!?(){}[\]_]+$")) {
                return false;
            }

            if (Regex.IsMatch(comment, @"^[a-zA-Z]+\s*=\s*[""'][^""']*[""']$")) {
                return false;
            }

            if (comment.Contains("(") && comment.Contains(")") &&
                (comment.Contains(".") || comment.Contains("Instance"))) {
                return false;
            }

            bool hasEnglishLetter = false;
            foreach (char c in comment) {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) {
                    hasEnglishLetter = true;
                    break;
                }
            }

            if (!hasEnglishLetter) {
                return false;
            }

            if (config.enableTextFilter && config.customTextFilterList.Count > 0) {
                foreach (string filterText in config.customTextFilterList) {
                    if (comment.Trim() == filterText) {
                        return false;
                    }
                }
            }

            return true;
        }

        bool IsValidText(string text) {
            if (string.IsNullOrWhiteSpace(text) || text.Length <= 1)
                return false;

            // 新增：Language表比对过滤
            if (config.enableLanguageComparison && languageTableExists) {
                if (existingLanguageTexts.Contains(text.Trim())) {
                    return false; // 如果文本已存在于Language表第一列中，则剔除
                }
            }

            if (config.enableTextFilter && config.customTextFilterList.Count > 0) {
                foreach (string filterText in config.customTextFilterList) {
                    if (text.Trim() == filterText) {
                        return false;
                    }
                }
            }

            bool hasEnglishLetter = false;
            foreach (char c in text) {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) {
                    hasEnglishLetter = true;
                    break;
                }
            }

            if (!hasEnglishLetter)
                return false;

            string trimmedText = text.Trim();

            if (trimmedText.Contains('_'))
                return false;

            if (trimmedText.Contains("/"))
                return false;

            if (Regex.IsMatch(trimmedText, @"^[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$"))
                return false;

            if (Regex.IsMatch(trimmedText, @"^#[0-9A-Fa-f]{6}$|^#[0-9A-Fa-f]{3}$"))
                return false;

            if (Regex.IsMatch(text, @"<color\s*=\s*[^>]*>.*?</color>", RegexOptions.IgnoreCase))
                return false;

            if (Regex.IsMatch(trimmedText, @"^(\*\.|\.)([a-zA-Z0-9]+)$"))
                return false;

            if (Regex.IsMatch(trimmedText, @"^[a-zA-Z0-9_/.-]*[/\\][a-zA-Z0-9_/.-]*$"))
                return false;

            if (Regex.IsMatch(trimmedText, @"^[/\\][a-zA-Z0-9_]+$"))
                return false;

            if (Regex.IsMatch(trimmedText, @"^(\\\\n|\\n|\\t|\\r|\\\\|\\')$"))
                return false;

            if (Regex.IsMatch(trimmedText, @"^[a-zA-Z0-9][\W_]*$") && trimmedText.Length <= 3)
                return false;

            // 剔除包含 {xxx}，但允许 {0}、{1}、{99} 这种纯数字占位符
            if (Regex.IsMatch(trimmedText, @"\{([^}]+)\}")) {
                // 查找所有 {} 内的内容
                var matches = Regex.Matches(trimmedText, @"\{([^}]+)\}");
                foreach (Match m in matches) {
                    string inside = m.Groups[1].Value;
                    // 只允许全部是数字的 {}，否则剔除
                    if (!Regex.IsMatch(inside, @"^\d+$"))
                        return false;
                }
            }

            // 过滤数字+单位的格式（时间、数量、货币等）
            if (IsNumericWithUnit(trimmedText))
                return false;

            if (Regex.IsMatch(trimmedText, @"^x\d+$", RegexOptions.IgnoreCase))
                return false;

            string lowerText = text.ToLower();
            string[] logKeywords = { "log", "debug", "warning", "error", "info", "trace", "console" };
            if (logKeywords.Any(keyword => lowerText.Contains(keyword)))
                return false;

            if (Regex.IsMatch(trimmedText, @"^[^\w\s]*$"))
                return false;

            if (Regex.IsMatch(trimmedText, @"^\([^)]*\)$|^\{[^}]*\}$|^\[[^\]]*\]$"))
                return false;

            if (Regex.IsMatch(trimmedText, @"^\d+\.\d+(\.\d+)*$"))
                return false;

            if (trimmedText.Length == 1 && !char.IsLetterOrDigit(trimmedText[0]))
                return false;

            string[] codeIdentifiers = { "null", "true", "false", "void", "int", "float", "string", "bool", "var", "const", "failed" };
            if (codeIdentifiers.Contains(trimmedText.ToLower()))
                return false;

            string[] meaninglessWords = { "failed" };
            if (meaninglessWords.Contains(trimmedText.ToLower()))
                return false;

            if (Regex.IsMatch(trimmedText, @"[a-zA-Z_][a-zA-Z0-9_]*\.[a-zA-Z_][a-zA-Z0-9_]*\s*\("))
                return false;

            if (trimmedText.Contains("You Get:") || trimmedText.Contains("x Income Boost") ||
                trimmedText.Contains("popup/icn/") || Regex.IsMatch(trimmedText, @"^x\d+$"))
                return false;

            return true;
        }

        string GetGameObjectPath(GameObject obj) {
            string path = obj.name;
            Transform parent = obj.transform.parent;

            while (parent != null) {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }

        string GetScenePath(GameObject obj) {
            UnityEngine.SceneManagement.Scene scene = obj.scene;
            return scene.path;
        }

        void ExportToFile() {
            if (extractedTexts.Count == 0) {
                EditorUtility.DisplayDialog("提示", "没有找到任何文本内容，请先执行抓取操作。", "确定");
                return;
            }

            try {
                var uniqueTexts = extractedTexts.Select(t => t.text)
                                              .Distinct()
                                              .Where(t => IsValidText(t))
                                              .OrderBy(t => t)
                                              .ToList();

                StringBuilder sb = new StringBuilder();

                foreach (string text in uniqueTexts) {
                    sb.AppendLine(text);
                }

                File.WriteAllText(config.outputPath, sb.ToString(), Encoding.UTF8);

                EditorUtility.DisplayDialog("导出成功",
                    $"已导出 {uniqueTexts.Count} 个唯一文本到: {config.outputPath}\n\n" +
                    "格式：每行一个文本，可直接粘贴到Excel", "确定");

                AssetDatabase.Refresh();
            }
            catch (System.Exception e) {
                EditorUtility.DisplayDialog("导出失败", $"导出文件时出错: {e.Message}", "确定");
            }
        }

        void AddDefaultLanguageTexts() {
            string[] defaultLanguages = {
            "中文", "Deutsch", "Français", "Español (ES)", "Español (AL)",
            "Português (BR)", "Português (PT)", "Italiano", "Nederlands",
            "日本語", "한국어", "Русский", "Українська", "Ελληνικά", "Türk", "English"
        };

            foreach (string lang in defaultLanguages) {
                if (!config.customTextFilterList.Contains(lang)) {
                    config.customTextFilterList.Add(lang);
                }
            }
        }
    }
}