using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System;

namespace HWGames.HWEditor.Tool {
    public class LangProxyToolWindow : EditorWindow {
        private Vector2 scrollPosition;
        private Vector2 folderScrollPosition;
        private Vector2 logScrollPosition;

        // 文件夹选择相关
        private List<string> selectedFolders = new List<string>();
        private bool useDefaultFolders = true;
        private List<string> defaultFolders = new List<string> { "Assets/Resources" };

        // 脚本文件夹
        private string scriptFolder = "Assets/Scripts";

        // 日志信息
        private List<string> logs = new List<string>();

        // 统计信息
        private int processedPrefabCount = 0;
        private int addedLangProxyCount = 0;
        private int processedScriptCount = 0;
        private int modifiedTextCallCount = 0;
        private int skippedLambdaCount = 0; // 新增：跳过的lambda表达式计数

        [MenuItem("HW Games/Asset Tools/多语言工具", false, 202)]
        public static void ShowWindow() {
            GetWindow<LangProxyToolWindow>("多语言工具");
        }

        void OnGUI() {
            GUILayout.Label("Unity 多语言工具", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 预制体处理区域
            DrawPrefabSection();

            EditorGUILayout.Space();
            EditorGUILayout.Separator();
            EditorGUILayout.Space();

            // 脚本处理区域
            DrawScriptSection();

            EditorGUILayout.Space();
            EditorGUILayout.Separator();
            EditorGUILayout.Space();

            // 操作按钮
            DrawActionButtons();

            EditorGUILayout.Space();

            // 统计信息
            DrawStatistics();

            EditorGUILayout.Space();

            // 日志显示
            DrawLogs();
        }

        void DrawPrefabSection() {
            GUILayout.Label("预制体处理设置:", EditorStyles.boldLabel);

            useDefaultFolders = EditorGUILayout.Toggle("使用默认文件夹 (Resources)", useDefaultFolders);

            if (useDefaultFolders) {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("将处理以下默认文件夹:", EditorStyles.miniLabel);
                foreach (string folder in defaultFolders) {
                    EditorGUILayout.LabelField("• " + folder, EditorStyles.miniLabel);
                }
                EditorGUI.indentLevel--;
            }
            else {
                EditorGUILayout.LabelField("拖拽文件夹到下方区域来选择处理范围:");

                // 文件夹拖拽区域
                Rect dropArea = GUILayoutUtility.GetRect(0.0f, 100.0f, GUILayout.ExpandWidth(true));
                GUI.Box(dropArea, "将文件夹拖拽到这里\n(支持多个文件夹)", EditorStyles.helpBox);

                // 处理拖拽事件
                HandleDragAndDrop(dropArea);

                // 显示已选择的文件夹
                if (selectedFolders.Count > 0) {
                    EditorGUILayout.LabelField($"已选择 {selectedFolders.Count} 个文件夹:", EditorStyles.boldLabel);

                    folderScrollPosition = EditorGUILayout.BeginScrollView(folderScrollPosition, GUILayout.MaxHeight(120));

                    for (int i = selectedFolders.Count - 1; i >= 0; i--) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("• " + selectedFolders[i], EditorStyles.miniLabel);
                        if (GUILayout.Button("移除", GUILayout.Width(50))) {
                            selectedFolders.RemoveAt(i);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();

                    if (GUILayout.Button("清空所有文件夹")) {
                        selectedFolders.Clear();
                    }
                }
                else {
                    EditorGUILayout.HelpBox("请拖拽文件夹到上方区域，或启用默认文件夹选项", MessageType.Info);
                }
            }
        }

        void DrawScriptSection() {
            GUILayout.Label("脚本处理设置:", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            scriptFolder = EditorGUILayout.TextField("脚本文件夹:", scriptFolder);
            if (GUILayout.Button("浏览", GUILayout.Width(60))) {
                string path = EditorUtility.OpenFolderPanel("选择脚本文件夹", "Assets", "");
                if (!string.IsNullOrEmpty(path)) {
                    // 转换为相对路径
                    if (path.StartsWith(Application.dataPath)) {
                        scriptFolder = "Assets" + path.Substring(Application.dataPath.Length);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox("将会修改指定文件夹下所有脚本中的Text接口调用:\n" +
                                   "xxx.text = \"hello\" → xxx.SetText(\"hello\")\n" +
                                   "注意：Lambda表达式中的text赋值会被自动跳过", MessageType.Info);
        }

        void DrawActionButtons() {
            GUILayout.Label("操作:", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("处理预制体\n(添加LangProxy)", GUILayout.Height(40))) {
                if (CanProcessPrefabs()) {
                    ProcessPrefabs();
                }
            }

            if (GUILayout.Button("处理脚本\n(修改Text接口)", GUILayout.Height(40))) {
                if (CanProcessScripts()) {
                    ProcessScripts();
                }
            }

            if (GUILayout.Button("全部处理", GUILayout.Height(40))) {
                if (CanProcessPrefabs() && CanProcessScripts()) {
                    ProcessAll();
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("清空日志")) {
                ClearLogs();
            }

            EditorGUILayout.EndHorizontal();
        }

        void DrawStatistics() {
            GUILayout.Label("统计信息:", EditorStyles.boldLabel);

            EditorGUILayout.LabelField($"处理的预制体数量: {processedPrefabCount}");
            EditorGUILayout.LabelField($"添加的LangProxy数量: {addedLangProxyCount}");
            EditorGUILayout.LabelField($"处理的脚本数量: {processedScriptCount}");
            EditorGUILayout.LabelField($"修改的Text调用数量: {modifiedTextCallCount}");
            EditorGUILayout.LabelField($"跳过的Lambda表达式: {skippedLambdaCount}");
        }

        void DrawLogs() {
            GUILayout.Label($"日志信息 ({logs.Count}):", EditorStyles.boldLabel);

            logScrollPosition = EditorGUILayout.BeginScrollView(logScrollPosition, GUILayout.MaxHeight(200));

            foreach (string log in logs) {
                EditorGUILayout.LabelField(log, EditorStyles.wordWrappedMiniLabel);
            }

            EditorGUILayout.EndScrollView();
        }

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
                                if (!selectedFolders.Contains(draggedPath)) {
                                    selectedFolders.Add(draggedPath);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        bool CanProcessPrefabs() {
            if (useDefaultFolders) {
                return true;
            }

            if (selectedFolders.Count == 0) {
                EditorUtility.DisplayDialog("提示", "请先选择要处理的文件夹，或启用默认文件夹选项。", "确定");
                return false;
            }

            return true;
        }

        bool CanProcessScripts() {
            if (!AssetDatabase.IsValidFolder(scriptFolder)) {
                EditorUtility.DisplayDialog("提示", "脚本文件夹路径无效，请选择正确的文件夹。", "确定");
                return false;
            }

            return true;
        }

        List<string> GetTargetFolders() {
            if (useDefaultFolders) {
                return defaultFolders.Where(folder => AssetDatabase.IsValidFolder(folder)).ToList();
            }
            else {
                return selectedFolders.Where(folder => AssetDatabase.IsValidFolder(folder)).ToList();
            }
        }

        void ProcessPrefabs() {
            AddLog("开始处理预制体...");

            processedPrefabCount = 0;
            addedLangProxyCount = 0;

            List<string> targetFolders = GetTargetFolders();

            foreach (string folder in targetFolders) {
                ProcessPrefabsInFolder(folder);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            AddLog($"预制体处理完成! 处理了 {processedPrefabCount} 个预制体，添加了 {addedLangProxyCount} 个LangProxy组件");
        }

        void ProcessPrefabsInFolder(string folder) {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { folder });

            foreach (string guid in prefabGuids) {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab != null) {
                    processedPrefabCount++;

                    // 获取所有Text组件
                    Text[] textComponents = prefab.GetComponentsInChildren<Text>(true);
                    TextMeshProUGUI[] tmpComponents = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);

                    bool modified = false;

                    // 处理Text组件
                    foreach (Text textComp in textComponents) {
                        if (AddLangProxyToGameObject(textComp.gameObject, path)) {
                            modified = true;
                            addedLangProxyCount++;
                        }
                    }

                    // 处理TextMeshPro组件
                    foreach (TextMeshProUGUI tmpComp in tmpComponents) {
                        if (AddLangProxyToGameObject(tmpComp.gameObject, path)) {
                            modified = true;
                            addedLangProxyCount++;
                        }
                    }

                    if (modified) {
                        EditorUtility.SetDirty(prefab);
                        AddLog($"已为预制体添加LangProxy: {path}");
                    }
                }
            }
        }

        bool AddLangProxyToGameObject(GameObject go, string prefabPath) {
            // 检查是否已经有LangProxy组件
            if (go.GetComponent<LangProxy>() != null) {
                return false;
            }

            // 添加LangProxy组件
            go.AddComponent<LangProxy>();
            AddLog($"  添加LangProxy到: {GetGameObjectPath(go)} (预制体: {Path.GetFileName(prefabPath)})");

            return true;
        }

        void ProcessScripts() {
            AddLog("开始处理脚本...");

            processedScriptCount = 0;
            modifiedTextCallCount = 0;
            skippedLambdaCount = 0;

            ProcessScriptsInFolder(scriptFolder);

            AssetDatabase.Refresh();

            AddLog($"脚本处理完成! 处理了 {processedScriptCount} 个脚本，修改了 {modifiedTextCallCount} 个Text调用，跳过了 {skippedLambdaCount} 个Lambda表达式");
        }

        void ProcessScriptsInFolder(string folder) {
            string[] scriptGuids = AssetDatabase.FindAssets("t:Script", new[] { folder });

            foreach (string guid in scriptGuids) {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string extension = Path.GetExtension(path).ToLower();

                if (extension == ".cs") {
                    ProcessScriptFile(path);
                }
            }
        }

        void ProcessScriptFile(string filePath) {
            if (Path.GetFileName(filePath).Equals("Util.cs", System.StringComparison.OrdinalIgnoreCase))
                return;

            try {
                string content = File.ReadAllText(filePath);
                string originalContent = content;
                int fileModificationCount = 0;

                // 按优先级处理，从最具体到最通用

                // 1. 处理 string.Format 的情况
                content = ProcessStringFormatAssignments(content, ref fileModificationCount, filePath);

                // 2. 处理简单的直接字符串赋值（最安全）
                content = ProcessSimpleStringAssignments(content, ref fileModificationCount, filePath);

                // 3. 处理通用情况（使用括号匹配确保完整性，加入Lambda检测）
                content = ProcessGeneralAssignments(content, ref fileModificationCount, filePath);

                // 写入修改
                if (fileModificationCount > 0) {
                    processedScriptCount++;
                    File.WriteAllText(filePath, content, Encoding.UTF8);
                    AddLog($"已修改脚本: {filePath} (修改了 {fileModificationCount} 处)");
                }
            }
            catch (System.Exception e) {
                AddLog($"处理脚本文件时出错 {filePath}: {e.Message}");
            }
        }

        string ProcessStringFormatAssignments(string content, ref int modificationCount, string filePath) {
            // 使用更精确的正则来匹配完整的string.Format赋值
            Regex regex = new Regex(@"(\w+)\.text\s*=\s*string\.Format\s*\(([^;]+)\)\s*;",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            MatchCollection matches = regex.Matches(content);

            for (int i = matches.Count - 1; i >= 0; i--) {
                Match match = matches[i];
                string objectName = match.Groups[1].Value;
                string formatArgs = match.Groups[2].Value;

                // 检查是否在lambda表达式中
                if (IsInLambdaExpression(content, match.Index)) {
                    skippedLambdaCount++;
                    AddLog($"  跳过: {objectName}.text = string.Format(...) (在Lambda表达式中)");
                    continue;
                }

                // 验证括号是否匹配
                if (IsParenthesesBalanced(formatArgs)) {
                    string newCode = $"{objectName}.SetTextFormat({formatArgs});";

                    content = content.Remove(match.Index, match.Length);
                    content = content.Insert(match.Index, newCode);

                    modificationCount++;
                    modifiedTextCallCount++;
                    AddLog($"  修改: {objectName}.text = string.Format(...) → {objectName}.SetTextFormat(...)");
                }
                else {
                    // 如果括号不匹配，使用更安全的方法
                    string safeReplacement = ProcessStringFormatSafely(content, match, objectName);
                    if (!string.IsNullOrEmpty(safeReplacement)) {
                        content = safeReplacement;
                        modificationCount++;
                        modifiedTextCallCount++;
                        AddLog($"  修改: {objectName}.text = string.Format(...) → {objectName}.SetTextFormat(...) [安全模式]");
                    }
                }
            }

            return content;
        }

        string ProcessStringFormatSafely(string content, Match match, string objectName) {
            // 安全地处理复杂的string.Format表达式
            int startPos = match.Index;
            int assignmentStart = content.IndexOf("string.Format(", startPos);

            if (assignmentStart == -1) return null;

            int parenStart = assignmentStart + "string.Format".Length;
            int parenEnd = FindMatchingParenthesis(content, parenStart);

            if (parenEnd == -1) return null;

            // 找到分号
            int semicolonPos = content.IndexOf(';', parenEnd);
            if (semicolonPos == -1) return null;

            // 提取完整的参数
            string formatArgs = content.Substring(parenStart + 1, parenEnd - parenStart - 1);
            string newCode = $"{objectName}.SetTextFormat({formatArgs});";

            // 替换从赋值开始到分号结束的整个语句
            string result = content.Remove(startPos, semicolonPos - startPos + 1);
            result = result.Insert(startPos, newCode);

            return result;
        }

        bool IsParenthesesBalanced(string text) {
            int count = 0;
            bool inString = false;
            bool escapeNext = false;

            foreach (char c in text) {
                if (escapeNext) {
                    escapeNext = false;
                    continue;
                }

                if (c == '\\') {
                    escapeNext = true;
                    continue;
                }

                if (c == '"') {
                    inString = !inString;
                    continue;
                }

                if (inString) continue;

                if (c == '(') count++;
                else if (c == ')') count--;

                if (count < 0) return false;
            }

            return count == 0;
        }

        string ProcessSimpleStringAssignments(string content, ref int modificationCount, string filePath) {
            // 只处理简单的字符串字面量赋值，不包含拼接或复杂表达式
            Regex regex = new Regex(@"(\w+)\.text\s*=\s*@?""([^""\\]*(?:\\.[^""\\]*)*)""\s*;");

            MatchCollection matches = regex.Matches(content);

            for (int i = matches.Count - 1; i >= 0; i--) {
                Match match = matches[i];
                string objectName = match.Groups[1].Value;
                string textValue = match.Groups[2].Value;

                // 检查是否在lambda表达式中
                if (IsInLambdaExpression(content, match.Index)) {
                    skippedLambdaCount++;
                    AddLog($"  跳过: {objectName}.text = \"...\" (在Lambda表达式中)");
                    continue;
                }

                string newCode = $"{objectName}.SetText(\"{textValue}\");";
                content = content.Remove(match.Index, match.Length);
                content = content.Insert(match.Index, newCode);

                modificationCount++;
                modifiedTextCallCount++;
                AddLog($"  修改: {objectName}.text = \"...\" → {objectName}.SetText(\"...\")");
            }

            return content;
        }

        string ProcessGeneralAssignments(string content, ref int modificationCount, string filePath) {
            Regex regex = new Regex(@"(\w+)\.text\s*=\s*([^;]+);");
            MatchCollection matches = regex.Matches(content);

            for (int i = matches.Count - 1; i >= 0; i--) {
                Match match = matches[i];
                string objectName = match.Groups[1].Value;
                string assignment = match.Groups[2].Value.Trim();

                // 检查是否在lambda表达式中
                if (IsInLambdaExpression(content, match.Index)) {
                    skippedLambdaCount++;
                    AddLog($"  跳过: {objectName}.text = 表达式 (在Lambda表达式中)");
                    continue;
                }

                // 修改跳过条件，只跳过纯字符串字面量，允许字符串拼接
                if ((assignment.StartsWith("\"") && assignment.EndsWith("\"") && !assignment.Contains("+")) || // 只跳过纯字符串
                    assignment.StartsWith("string.Format", StringComparison.OrdinalIgnoreCase) ||
                    assignment.Contains("SetText") ||
                    assignment.Contains("SetTextFormat")) {
                    continue;
                }

                string newCode = $"{objectName}.SetText({assignment});";
                content = content.Remove(match.Index, match.Length);
                content = content.Insert(match.Index, newCode);

                modificationCount++;
                modifiedTextCallCount++;
                AddLog($"  修改: {objectName}.text = 表达式 → {objectName}.SetText(表达式)");
            }

            return content;
        }

        // 检查指定位置是否在lambda表达式中
        bool IsInLambdaExpression(string content, int position) {
            try {
                // 向前搜索最近的 "=>" 符号
                int searchStart = Math.Max(0, position - 300); // 扩大搜索范围
                int searchEnd = Math.Min(content.Length, position + 100);

                // 在当前位置前查找 "=>"
                int lambdaPos = content.LastIndexOf("=>", position, position - searchStart);
                if (lambdaPos == -1) return false;

                // 向前查找lambda的开始位置，寻找参数定义
                string beforeLambda = content.Substring(searchStart, lambdaPos - searchStart);

                // 向后查找lambda的结束位置
                string afterLambda = content.Substring(lambdaPos + 2, searchEnd - lambdaPos - 2);

                // 检查lambda前的模式：参数定义
                // 常见模式：x =>, (x) =>, (x, y) =>
                Regex paramPattern = new Regex(@"(\w+|\([^)]*\))\s*$");
                if (!paramPattern.IsMatch(beforeLambda)) return false;

                // 检查是否在DOTween.To或类似的方法调用中
                string methodContext = content.Substring(Math.Max(0, lambdaPos - 150),
                                                       Math.Min(150, lambdaPos - Math.Max(0, lambdaPos - 150)));

                // 常见的lambda使用场景
                if (methodContext.Contains("DOTween.To") ||
                    methodContext.Contains(".To(") ||
                    methodContext.Contains("Tween.") ||
                    methodContext.Contains("LeanTween.") ||
                    methodContext.Contains("iTween.") ||
                    methodContext.IndexOf("(", Math.Max(0, methodContext.Length - 50)) != -1) { // 方法调用中

                    // 进一步验证：确保当前位置在lambda作用域内
                    int lambdaEnd = FindLambdaExpressionEnd(content, lambdaPos + 2);
                    if (lambdaEnd > position) {
                        return true;
                    }
                }

                return false;
            }
            catch {
                // 如果解析出错，采用保守策略：检查附近是否有 "=>"
                string nearbyText = content.Substring(Math.Max(0, position - 100),
                                                    Math.Min(200, content.Length - Math.Max(0, position - 100)));
                return nearbyText.Contains("=>");
            }
        }

        // 查找lambda表达式的结束位置
        int FindLambdaExpressionEnd(string content, int lambdaStart) {
            int braceCount = 0;
            int parenCount = 0;
            bool inString = false;
            bool escapeNext = false;
            bool foundStatement = false;

            for (int i = lambdaStart; i < content.Length; i++) {
                char c = content[i];

                if (escapeNext) {
                    escapeNext = false;
                    continue;
                }

                if (c == '\\') {
                    escapeNext = true;
                    continue;
                }

                if (c == '"') {
                    inString = !inString;
                    continue;
                }

                if (inString) continue;

                switch (c) {
                    case '{':
                        braceCount++;
                        foundStatement = true;
                        break;
                    case '}':
                        braceCount--;
                        if (braceCount < 0) return i; // lambda块结束
                        break;
                    case '(':
                        parenCount++;
                        break;
                    case ')':
                        parenCount--;
                        if (parenCount < 0 && braceCount == 0 && foundStatement) return i; // lambda在方法参数中结束
                        break;
                    case ',':
                        if (braceCount == 0 && parenCount == 0 && foundStatement) return i; // lambda参数结束
                        break;
                    case ';':
                        if (braceCount == 0) {
                            foundStatement = true;
                            // 如果不在任何括号内，这是lambda的结束
                            if (parenCount == 0) return i;
                        }
                        break;
                    default:
                        if (!char.IsWhiteSpace(c)) foundStatement = true;
                        break;
                }
            }

            return content.Length;
        }

        // 提取完整的赋值表达式（从等号后到分号前）
        string ExtractCompleteAssignment(string content, int startPos) {
            if (startPos >= content.Length) return null;

            int parenthesesCount = 0;
            int bracesCount = 0;
            int bracketsCount = 0;
            bool inString = false;
            bool inChar = false;
            bool escapeNext = false;

            for (int i = startPos; i < content.Length; i++) {
                char c = content[i];

                if (escapeNext) {
                    escapeNext = false;
                    continue;
                }

                if (c == '\\') {
                    escapeNext = true;
                    continue;
                }

                if (!inChar && c == '"') {
                    inString = !inString;
                    continue;
                }

                if (!inString && c == '\'') {
                    inChar = !inChar;
                    continue;
                }

                if (inString || inChar) continue;

                switch (c) {
                    case '(':
                        parenthesesCount++;
                        break;
                    case ')':
                        parenthesesCount--;
                        break;
                    case '{':
                        bracesCount++;
                        break;
                    case '}':
                        bracesCount--;
                        break;
                    case '[':
                        bracketsCount++;
                        break;
                    case ']':
                        bracketsCount--;
                        break;
                    case ';':
                        if (parenthesesCount == 0 && bracesCount == 0 && bracketsCount == 0) {
                            return content.Substring(startPos, i - startPos).Trim();
                        }
                        break;
                }
            }

            return null; // 没有找到完整的表达式
        }

        // 找到匹配的右括号位置
        int FindMatchingParenthesis(string text, int openPos) {
            if (openPos >= text.Length || text[openPos] != '(') return -1;

            int count = 1;
            bool inString = false;
            bool escapeNext = false;

            for (int i = openPos + 1; i < text.Length; i++) {
                char c = text[i];

                if (escapeNext) {
                    escapeNext = false;
                    continue;
                }

                if (c == '\\') {
                    escapeNext = true;
                    continue;
                }

                if (c == '"') {
                    inString = !inString;
                    continue;
                }

                if (inString) continue;

                if (c == '(') {
                    count++;
                }
                else if (c == ')') {
                    count--;
                    if (count == 0) {
                        return i;
                    }
                }
            }

            return -1;
        }

        void ProcessAll() {
            AddLog("开始全部处理...");

            ProcessPrefabs();
            ProcessScripts();

            AddLog("全部处理完成!");
        }

        void AddLog(string message) {
            logs.Add($"[{System.DateTime.Now:HH:mm:ss}] {message}");

            // 限制日志数量
            if (logs.Count > 1000) {
                logs.RemoveAt(0);
            }

            // 自动滚动到底部
            logScrollPosition.y = float.MaxValue;

            Repaint();
        }

        void ClearLogs() {
            logs.Clear();
            processedPrefabCount = 0;
            addedLangProxyCount = 0;
            processedScriptCount = 0;
            modifiedTextCallCount = 0;
            skippedLambdaCount = 0;
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
    }
}