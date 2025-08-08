using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace HWGames.HWEditor.Tool {
    public class RefTableScriptGenerator : EditorWindow {
        private Vector2 scrollPosition;
        private Vector2 tableScrollPosition;
        private bool showGeneratedFiles = false;
        private List<string> generatedFiles = new List<string>();
        private List<string> updatedFiles = new List<string>();
        private List<string> errorFiles = new List<string>();
        private List<string> deletedFiles = new List<string>();

        private string refdataPath = "Assets/Resources/Refdata";
        private string scriptOutputPath = "Assets/Scripts/GamePlay/RefData";
        private string refDataMgrPath = "Assets/Scripts/Engine/RefData/RefDataMgr.cs";

        private Dictionary<string, RefTableInfo> tableInfos = new Dictionary<string, RefTableInfo>();
        private bool manageRefDataMgr = true; // 是否管理RefDataMgr
        private bool deleteObsoleteScripts = true; // 是否删除废弃脚本

        [MenuItem("HW Games/Asset Tools/表格脚本生成工具", false, 240)]
        public static void ShowWindow() {
            var window = GetWindow<RefTableScriptGenerator>("表格脚本生成工具");
            window.minSize = new Vector2(600, 700);
            window.AutoScanOnOpen();
        }

        void OnEnable() {
            // 窗口激活时也自动扫描
            if (tableInfos.Count == 0) {
                AutoScanOnOpen();
            }
        }

        void AutoScanOnOpen() {
            // 延迟执行扫描，确保窗口完全加载
            EditorApplication.delayCall += () => {
                if (Directory.Exists(refdataPath)) {
                    ScanTableFiles();
                }
            };
        }

        void OnGUI() {
            GUILayout.Label("表格脚本生成工具", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // 路径设置
            DrawPathSettings();
            GUILayout.Space(5);

            // 功能选项
            DrawFeatureOptions();
            GUILayout.Space(5);

            // 忽略规则提示
            DrawIgnoreRules();
            GUILayout.Space(10);

            // 操作按钮
            DrawOperationButtons();
            GUILayout.Space(10);

            // 表格信息显示
            DrawTableInfos();
            GUILayout.Space(10);

            // 显示生成结果
            DrawGenerationResults();
        }

        void DrawFeatureOptions() {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("功能选项", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            refDataMgrPath = EditorGUILayout.TextField("RefDataMgr路径:", refDataMgrPath);
            if (GUILayout.Button("选择", GUILayout.Width(50))) {
                string selectedPath = EditorUtility.OpenFilePanel("选择RefDataMgr.cs文件", "Assets", "cs");
                if (!string.IsNullOrEmpty(selectedPath)) {
                    refDataMgrPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
            }
            EditorGUILayout.EndHorizontal();

            manageRefDataMgr = EditorGUILayout.Toggle("自动管理RefDataMgr读表代码", manageRefDataMgr);
            deleteObsoleteScripts = EditorGUILayout.Toggle("自动删除废弃的脚本", deleteObsoleteScripts);

            EditorGUILayout.EndVertical();
        }

        void DrawIgnoreRules() {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("读表模块规则", EditorStyles.boldLabel);

            GUI.color = new Color(0.125f, 0.867f, 0.894f, 1.000f); // 淡黄色背景
            EditorGUILayout.HelpBox(
                "• 自动忽略：Language.txt 文件不会生成脚本\n" +
                "• 枚举生成：带type的列名，会被识别为枚举，枚举类型需要先在脚本中定义，注意不要写在读表脚本中，防止覆盖\n" +
                "• 类名转换：小写文件名自动转为首字母大写 (例: task.txt → RefTask.cs)\n" +
                "• 强制类型识别（优先于自动识别）：\n" +
                "  - Desc 字段或包含 Str 的字段 → string 类型\n" +
                "  - 包含 double 的字段 → double 类型\n" +
                "  - 包含 float 的字段 → float 类型\n" +
                "  - 包含 type 的字段（不含id） → 枚举类型\n" +
                "• 自动类型识别：根据数据内容推断 int/float/bool/string\n" +
                "• 勾选控制：只有勾选的表格才会生成/更新脚本\n" +
                "• 自动管理：可自动管理RefDataMgr中的读表代码和删除废弃脚本",
                MessageType.Info);
            GUI.color = Color.white;

            EditorGUILayout.EndVertical();
        }

        void DrawPathSettings() {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("路径设置", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            refdataPath = EditorGUILayout.TextField("表格数据路径:", refdataPath);
            if (GUILayout.Button("选择", GUILayout.Width(50))) {
                string selectedPath = EditorUtility.OpenFolderPanel("选择表格数据文件夹", "Assets", "");
                if (!string.IsNullOrEmpty(selectedPath)) {
                    refdataPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            scriptOutputPath = EditorGUILayout.TextField("脚本输出路径:", scriptOutputPath);
            if (GUILayout.Button("选择", GUILayout.Width(50))) {
                string selectedPath = EditorUtility.OpenFolderPanel("选择脚本输出文件夹", "Assets", "");
                if (!string.IsNullOrEmpty(selectedPath)) {
                    scriptOutputPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        void SetAllTablesSelection(bool selected) {
            foreach (var tableInfo in tableInfos.Values) {
                tableInfo.IsSelected = selected;
            }
        }

        void InvertTableSelection() {
            foreach (var tableInfo in tableInfos.Values) {
                tableInfo.IsSelected = !tableInfo.IsSelected;
            }
        }

        void DrawOperationButtons() {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("扫描表格文件", GUILayout.Height(30))) {
                ScanTableFiles();
            }
            if (GUILayout.Button("生成/更新脚本", GUILayout.Height(30))) {
                GenerateScripts();
            }
            if (GUILayout.Button("清空结果", GUILayout.Height(30))) {
                ClearResults();
            }
            EditorGUILayout.EndHorizontal();

            // 选择操作按钮
            if (tableInfos.Count > 0) {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("全选", GUILayout.Height(25))) {
                    SetAllTablesSelection(true);
                }
                if (GUILayout.Button("全不选", GUILayout.Height(25))) {
                    SetAllTablesSelection(false);
                }
                if (GUILayout.Button("反选", GUILayout.Height(25))) {
                    InvertTableSelection();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        void DrawTableInfos() {
            if (tableInfos.Count > 0) {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                int selectedCount = tableInfos.Values.Count(t => t.IsSelected);
                GUILayout.Label($"发现的表格文件 ({tableInfos.Count}) - 已选择: {selectedCount}", EditorStyles.boldLabel);

                tableScrollPosition = EditorGUILayout.BeginScrollView(tableScrollPosition, GUILayout.Height(200));
                foreach (var tableInfo in tableInfos.Values) {
                    EditorGUILayout.BeginHorizontal();

                    // 复选框
                    tableInfo.IsSelected = EditorGUILayout.Toggle(tableInfo.IsSelected, GUILayout.Width(20));

                    EditorGUILayout.LabelField($"{tableInfo.FileName}", GUILayout.Width(120));
                    EditorGUILayout.LabelField($"类名: {tableInfo.ClassName}", GUILayout.Width(130));
                    EditorGUILayout.LabelField($"字段: {tableInfo.Fields.Count}", GUILayout.Width(60));

                    if (tableInfo.HasExistingScript) {
                        GUI.color = Color.yellow;
                        EditorGUILayout.LabelField("已存在", GUILayout.Width(60));
                        GUI.color = Color.white;
                    }
                    else {
                        GUI.color = Color.green;
                        EditorGUILayout.LabelField("新建", GUILayout.Width(60));
                        GUI.color = Color.white;
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
        }

        void DrawGenerationResults() {
            if (generatedFiles.Count > 0 || updatedFiles.Count > 0 || errorFiles.Count > 0 || deletedFiles.Count > 0) {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("生成结果", EditorStyles.boldLabel);

                if (generatedFiles.Count > 0) {
                    GUI.color = Color.green;
                    GUILayout.Label($"新生成: {generatedFiles.Count} 个文件", EditorStyles.boldLabel);
                    GUI.color = Color.white;
                }

                if (updatedFiles.Count > 0) {
                    GUI.color = Color.yellow;
                    GUILayout.Label($"已更新: {updatedFiles.Count} 个文件", EditorStyles.boldLabel);
                    GUI.color = Color.white;
                }

                if (deletedFiles.Count > 0) {
                    GUI.color = Color.red;
                    GUILayout.Label($"已删除: {deletedFiles.Count} 个文件", EditorStyles.boldLabel);
                    GUI.color = Color.white;
                }

                if (errorFiles.Count > 0) {
                    GUI.color = Color.red;
                    GUILayout.Label($"出错: {errorFiles.Count} 个文件", EditorStyles.boldLabel);
                    GUI.color = Color.white;
                }

                showGeneratedFiles = EditorGUILayout.Foldout(showGeneratedFiles, "查看详细结果");
                if (showGeneratedFiles) {
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));

                    foreach (string file in generatedFiles) {
                        GUI.color = Color.green;
                        EditorGUILayout.LabelField($"✓ 新生成: {file}");
                    }

                    foreach (string file in updatedFiles) {
                        GUI.color = Color.yellow;
                        EditorGUILayout.LabelField($"⟳ 已更新: {file}");
                    }

                    foreach (string file in deletedFiles) {
                        GUI.color = Color.red;
                        EditorGUILayout.LabelField($"🗑 已删除: {file}");
                    }

                    foreach (string file in errorFiles) {
                        GUI.color = Color.red;
                        EditorGUILayout.LabelField($"✗ 出错: {file}");
                    }

                    GUI.color = Color.white;
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
        }

        void ScanTableFiles() {
            tableInfos.Clear();

            if (!Directory.Exists(refdataPath)) {
                EditorUtility.DisplayDialog("错误", $"表格数据路径不存在: {refdataPath}", "确定");
                return;
            }

            string[] txtFiles = Directory.GetFiles(refdataPath, "*.txt", SearchOption.AllDirectories);
            int ignoredCount = 0;

            foreach (string filePath in txtFiles) {
                try {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);

                    // 忽略 Language.txt 文件
                    if (fileName.Equals("Language", StringComparison.OrdinalIgnoreCase)) {
                        Debug.Log($"[读表模块] 自动忽略Language文件: {fileName}.txt");
                        ignoredCount++;
                        continue;
                    }

                    RefTableInfo tableInfo = ParseTableFile(filePath);
                    if (tableInfo != null) {
                        tableInfos[tableInfo.FileName] = tableInfo;
                    }
                }
                catch (Exception e) {
                    Debug.LogError($"解析表格文件失败: {filePath}, 错误: {e.Message}");
                }
            }

            string scanResult = $"扫描完成，发现 {tableInfos.Count} 个表格文件";
            if (ignoredCount > 0) {
                scanResult += $"，忽略 {ignoredCount} 个文件";
            }
            Debug.Log($"[读表模块] {scanResult}");
        }

        RefTableInfo ParseTableFile(string filePath) {
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            if (lines.Length < 3) return null;

            string fileName = Path.GetFileNameWithoutExtension(filePath);

            // 确保类名首字母大写
            string capitalizedFileName = char.ToUpper(fileName[0]) + fileName.Substring(1);
            string className = "Ref" + capitalizedFileName;

            // 解析字段名（第二行）
            string[] fieldNames = lines[1].Split('\t');

            // 解析字段类型（基于数据行推断）
            List<FieldInfo> fields = new List<FieldInfo>();
            for (int i = 0; i < fieldNames.Length; i++) {
                string fieldName = fieldNames[i].Trim();
                if (string.IsNullOrEmpty(fieldName)) continue;

                string fieldType = InferFieldType(fieldName, lines, i);
                fields.Add(new FieldInfo {
                    Name = fieldName,
                    Type = fieldType
                });
            }

            // 检查是否已存在脚本
            string scriptPath = Path.Combine(scriptOutputPath, className + ".cs");
            bool hasExistingScript = File.Exists(scriptPath);

            return new RefTableInfo {
                FileName = fileName,
                ClassName = className,
                FilePath = filePath,
                ScriptPath = scriptPath,
                Fields = fields,
                HasExistingScript = hasExistingScript,
                IsSelected = true // 默认勾选
            };
        }

        string InferFieldType(string fieldName, string[] lines, int columnIndex) {
            // 根据字段名推断类型（强制识别，优先于自动识别）
            string lowerFieldName = fieldName.ToLower();

            // Desc字段或包含Str的字段强制为string类型
            if (lowerFieldName.Equals("desc") || lowerFieldName.Contains("str")) {
                return "string";
            }

            // 包含double的字段强制为double类型
            if (lowerFieldName.Contains("double")) {
                return "double";
            }

            // 包含float的字段强制为float类型
            if (lowerFieldName.Contains("float")) {
                return "float";
            }

            // 特殊类型推断：包含type但不含id的字段认为是枚举
            if (lowerFieldName.Contains("type") && !lowerFieldName.Contains("id")) {
                return fieldName; // 假设是枚举类型
            }

            // 根据数据内容自动推断类型
            for (int i = 2; i < lines.Length; i++) {
                string[] values = lines[i].Split('\t');
                if (columnIndex < values.Length) {
                    string value = values[columnIndex].Trim();
                    if (!string.IsNullOrEmpty(value)) {
                        if (int.TryParse(value, out _)) {
                            return "int";
                        }
                        if (float.TryParse(value, out _)) {
                            return "float";
                        }
                        if (bool.TryParse(value, out _)) {
                            return "bool";
                        }
                        return "string";
                    }
                }
            }

            return "string"; // 默认类型
        }

        void GenerateScripts() {
            if (tableInfos.Count == 0) {
                EditorUtility.DisplayDialog("提示", "请先扫描表格文件", "确定");
                return;
            }

            ClearResults();

            // 确保输出目录存在
            if (!Directory.Exists(scriptOutputPath)) {
                Directory.CreateDirectory(scriptOutputPath);
            }

            try {
                // 获取选中的表格
                var selectedTables = tableInfos.Values.Where(t => t.IsSelected).ToList();

                if (selectedTables.Count == 0) {
                    EditorUtility.DisplayDialog("提示", "请至少选择一个表格文件", "确定");
                    return;
                }

                // 处理删除废弃脚本
                if (deleteObsoleteScripts) {
                    DeleteObsoleteScripts();
                }

                int processedCount = 0;
                foreach (var tableInfo in selectedTables) {
                    float progress = (float)processedCount / selectedTables.Count;
                    EditorUtility.DisplayProgressBar("生成脚本",
                        $"正在处理: {tableInfo.ClassName} ({processedCount + 1}/{selectedTables.Count})",
                        progress);

                    try {
                        string scriptContent = GenerateScriptContent(tableInfo);
                        bool isUpdate = tableInfo.HasExistingScript;

                        // 检查是否需要更新
                        if (isUpdate) {
                            string existingContent = File.ReadAllText(tableInfo.ScriptPath);
                            if (existingContent.Equals(scriptContent)) {
                                // 内容相同，跳过
                                processedCount++;
                                continue;
                            }
                        }

                        File.WriteAllText(tableInfo.ScriptPath, scriptContent, Encoding.UTF8);

                        if (isUpdate) {
                            updatedFiles.Add(tableInfo.ClassName);
                        }
                        else {
                            generatedFiles.Add(tableInfo.ClassName);
                        }
                    }
                    catch (Exception e) {
                        errorFiles.Add($"{tableInfo.ClassName}: {e.Message}");
                        Debug.LogError($"生成脚本失败: {tableInfo.ClassName}, 错误: {e.Message}");
                    }

                    processedCount++;
                }

                // 更新RefDataMgr
                if (manageRefDataMgr && File.Exists(refDataMgrPath)) {
                    UpdateRefDataMgr();
                }

                AssetDatabase.Refresh();

                string resultMsg = $"脚本生成完成！\n新生成: {generatedFiles.Count} 个\n已更新: {updatedFiles.Count} 个";
                if (deletedFiles.Count > 0) {
                    resultMsg += $"\n已删除: {deletedFiles.Count} 个";
                }
                if (errorFiles.Count > 0) {
                    resultMsg += $"\n出错: {errorFiles.Count} 个";
                }

                EditorUtility.DisplayDialog("完成", resultMsg, "确定");
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
        }

        void DeleteObsoleteScripts() {
            if (!Directory.Exists(scriptOutputPath)) return;

            // 获取当前表格对应的类名
            var currentClassNames = tableInfos.Values.Select(t => t.ClassName).ToHashSet();

            // 扫描脚本文件夹中的Ref*.cs文件
            string[] existingScripts = Directory.GetFiles(scriptOutputPath, "Ref*.cs", SearchOption.TopDirectoryOnly);

            foreach (string scriptPath in existingScripts) {
                string fileName = Path.GetFileNameWithoutExtension(scriptPath);
                if (fileName.Contains("Language", StringComparison.OrdinalIgnoreCase)) {
                    Debug.Log($"[读表模块] 自动忽略Language文件: {fileName}.txt");
                    continue;
                }

                // 如果脚本对应的表格不存在，则删除
                if (!currentClassNames.Contains(fileName)) {
                    try {
                        File.Delete(scriptPath);
                        deletedFiles.Add(fileName);
                        Debug.Log($"[读表模块] 删除废弃脚本: {fileName}.cs");
                    }
                    catch (Exception e) {
                        Debug.LogError($"删除脚本失败: {fileName}.cs, 错误: {e.Message}");
                        errorFiles.Add($"{fileName}.cs: {e.Message}");
                    }
                }
            }
        }

        void UpdateRefDataMgr() {
            try {
                if (!File.Exists(refDataMgrPath)) {
                    Debug.LogWarning($"RefDataMgr文件不存在: {refDataMgrPath}");
                    return;
                }

                string content = File.ReadAllText(refDataMgrPath);
                string originalContent = content;

                // 获取当前有效的类名
                var currentClassNames = tableInfos.Values.Where(t => t.IsSelected).Select(t => t.ClassName).ToList();

                // 更新Init方法
                content = UpdateRefDataMgrMethod(content, "Init", currentClassNames, true);

                // 更新InitBasic方法
                content = UpdateRefDataMgrMethod(content, "InitBasic", currentClassNames, false);

                // 只有内容发生变化时才写入文件
                if (content != originalContent) {
                    // 验证生成的代码语法
                    if (ValidateGeneratedCode(content)) {
                        File.WriteAllText(refDataMgrPath, content);
                        Debug.Log("[读表模块] RefDataMgr已更新");
                    }
                    else {
                        Debug.LogError("生成的RefDataMgr代码有语法错误，已回滚");
                        // 可以选择写入原始内容或者提示用户
                    }
                }
                else {
                    Debug.Log("[读表模块] RefDataMgr无需更新");
                }
            }
            catch (Exception e) {
                Debug.LogError($"更新RefDataMgr失败: {e.Message}");
                errorFiles.Add($"RefDataMgr: {e.Message}");
            }
        }

        bool ValidateGeneratedCode(string code) {
            // 检查基本的语法结构
            int openBraces = code.Count(c => c == '{');
            int closeBraces = code.Count(c => c == '}');

            if (openBraces != closeBraces) {
                Debug.LogError("大括号不匹配");
                return false;
            }

            // 检查是否包含必要的方法声明
            if (!code.Contains("public IEnumerator Init()") || !code.Contains("public void InitBasic()")) {
                Debug.LogError("缺少必要的方法声明");
                return false;
            }

            return true;
        }

        string UpdateRefDataMgrMethod(string content, string methodName, List<string> classNames, bool isCoroutine) {
            try {
                // 更精确的方法匹配正则表达式
                string pattern = $@"(public\s+(?:IEnumerator|void)\s+{methodName}\s*\(\s*\)\s*\{{)([^{{}}]*(?:\{{[^{{}}]*\}}[^{{}}]*)*?)(\}})";

                Match match = Regex.Match(content, pattern, RegexOptions.Singleline);
                if (!match.Success) {
                    // 如果没找到方法，尝试更宽松的匹配
                    pattern = $@"(public\s+(?:IEnumerator|void)\s+{methodName}\s*\([^)]*\)\s*\{{)(.*?)(\}}\s*(?=\s*public|\s*private|\s*protected|\s*\}}\s*$))";
                    match = Regex.Match(content, pattern, RegexOptions.Singleline);

                    if (!match.Success) {
                        Debug.LogWarning($"未找到{methodName}方法");
                        return content;
                    }
                }

                string methodSignature = match.Groups[1].Value;
                string methodEnd = match.Groups[3].Value;

                // 生成新的方法体
                StringBuilder newMethodBody = new StringBuilder();

                if (methodName == "Init") {
                    newMethodBody.AppendLine();
                    newMethodBody.AppendLine("        Debug.Log(\"RefDataMgr Init Start!!!\");");
                    newMethodBody.AppendLine();
                    newMethodBody.AppendLine("        List<IEnumerator> co_list = new List<IEnumerator>() {");

                    foreach (string className in classNames) {
                        newMethodBody.AppendLine($"            Co_LoadGeneric({className}.cacheMap),");
                    }

                    newMethodBody.AppendLine("        };");
                    newMethodBody.AppendLine("        for (int index = 0, total = co_list.Count; index < total; index++) {");
                    newMethodBody.AppendLine("            yield return CoDelegator.Coroutine(co_list[index]);");
                    newMethodBody.AppendLine("            //WinMsg.SendMsg(WinMsgType.ProcessLoad_Refdata, index, total, (index + 1.0f) / total);");
                    newMethodBody.AppendLine("        }");
                    newMethodBody.AppendLine("        Debug.Log(\"RefDataMgr Init End!!!\");");
                    newMethodBody.AppendLine();
                    newMethodBody.AppendLine("        yield break;");
                    newMethodBody.AppendLine("    ");
                }
                else if (methodName == "InitBasic") {
                    newMethodBody.AppendLine();
                    foreach (string className in classNames) {
                        newMethodBody.AppendLine($"        LoadGeneric({className}.cacheMap);");
                    }
                    newMethodBody.AppendLine("    ");
                }

                // 替换整个方法
                string newMethod = methodSignature + newMethodBody.ToString() + methodEnd;
                return content.Replace(match.Value, newMethod);
            }
            catch (Exception e) {
                Debug.LogError($"更新{methodName}方法失败: {e.Message}");
                return content;
            }
        }

        string GenerateScriptContent(RefTableInfo tableInfo) {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine($"public class {tableInfo.ClassName} : RefBase {{");
            sb.AppendLine();
            sb.AppendLine($"    public static Dictionary<int, {tableInfo.ClassName}> cacheMap = new Dictionary<int, {tableInfo.ClassName}>();");
            sb.AppendLine();

            // 生成字段
            foreach (var field in tableInfo.Fields) {
                string comment = GetFieldComment(field.Name);
                if (!string.IsNullOrEmpty(comment)) {
                    sb.AppendLine($"    /// <summary>");
                    sb.AppendLine($"    /// {comment}");
                    sb.AppendLine($"    /// </summary>");
                }
                sb.AppendLine($"    public {field.Type} {field.Name};");
            }

            sb.AppendLine();
            sb.AppendLine("    public override string GetFirstKeyName() {");
            sb.AppendLine($"        return \"{tableInfo.Fields[0].Name}\";");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public override void LoadByLine(Dictionary<string, string> _value, int _line) {");
            sb.AppendLine("        base.LoadByLine(_value, _line);");

            foreach (var field in tableInfo.Fields) {
                string loadMethod = GetLoadMethod(field.Type, field.Name);
                sb.AppendLine($"        {field.Name} = {loadMethod};");
            }

            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine($"    public static {tableInfo.ClassName} GetRef(int {tableInfo.Fields[0].Name.ToLower()}) {{");
            sb.AppendLine($"        {tableInfo.ClassName} data = null;");
            sb.AppendLine($"        if (cacheMap.TryGetValue({tableInfo.Fields[0].Name.ToLower()}, out data)) {{");
            sb.AppendLine("            return data;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        if (data == null) {");
            sb.AppendLine($"            Debug.LogError(\"error {tableInfo.ClassName} key:\" + {tableInfo.Fields[0].Name.ToLower()});");
            sb.AppendLine("        }");
            sb.AppendLine("        return data;");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        string GetFieldComment(string fieldName) {
            // 可以根据需要添加字段注释映射
            switch (fieldName.ToLower()) {
                case "itemid": return "物品ID";
                case "unlocktype": return "解锁类型";
                case "param": return "参数";
                case "desc": return "描述";
                default: return "";
            }
        }

        string GetLoadMethod(string fieldType, string fieldName) {
            if (fieldType == "int") {
                return $"GetInt(\"{fieldName}\")";
            }
            if (fieldType == "float") {
                return $"GetFloat(\"{fieldName}\")";
            }
            if (fieldType == "double") {
                return $"GetDouble(\"{fieldName}\")";
            }
            if (fieldType == "bool") {
                return $"GetBool(\"{fieldName}\")";
            }
            if (fieldType == "string") {
                return $"GetString(\"{fieldName}\")";
            }
            // 枚举类型
            return $"({fieldType})GetEnum(\"{fieldName}\", typeof({fieldType}))";
        }

        void ClearResults() {
            generatedFiles.Clear();
            updatedFiles.Clear();
            errorFiles.Clear();
            deletedFiles.Clear();
            showGeneratedFiles = false;
        }
    }

    public class RefTableInfo {
        public string FileName;
        public string ClassName;
        public string FilePath;
        public string ScriptPath;
        public List<FieldInfo> Fields;
        public bool HasExistingScript;
        public bool IsSelected = true; // 默认勾选
    }

    public class FieldInfo {
        public string Name;
        public string Type;
    }
}