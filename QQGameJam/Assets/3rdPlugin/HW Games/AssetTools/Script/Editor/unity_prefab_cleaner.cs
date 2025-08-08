using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace HWGames.HWEditor.Tool {
    public class PrefabMissingScriptCleaner : EditorWindow {
        private List<Object> draggedObjects = new List<Object>();
        private Vector2 scrollPosition;
        private bool showProcessedFiles = false;
        private List<string> processedFiles = new List<string>();
        private int totalPrefabsFound = 0;
        private int cleanedPrefabsCount = 0;

        // 分页相关
        private int currentPage = 0;
        private string[] pageNames = { "失效脚本清理", "字体替换" };

        // 字体替换相关
        private Font targetFont = null;
        private List<string> fontReplacedFiles = new List<string>();
        private int totalTextsFound = 0;
        private int replacedTextsCount = 0;

        [MenuItem("HW Games/Asset Tools/预制体处理工具", false, 230)]
        public static void ShowWindow() {
            var window = GetWindow<PrefabMissingScriptCleaner>("预制体处理工具");
            window.minSize = new Vector2(400, 500);
        }

        void OnGUI() {
            GUILayout.Label("预制体处理工具", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // 分页选项卡
            DrawPageTabs();
            GUILayout.Space(10);

            // 根据当前页面显示不同内容
            switch (currentPage) {
                case 0:
                    DrawScriptCleanerPage();
                    break;
                case 1:
                    DrawFontReplacerPage();
                    break;
            }
        }

        void DrawPageTabs() {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < pageNames.Length; i++) {
                if (GUILayout.Toggle(currentPage == i, pageNames[i], EditorStyles.toolbarButton)) {
                    if (currentPage != i) {
                        currentPage = i;
                        ClearCurrentPageData();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        void DrawScriptCleanerPage() {
            GUILayout.Label("失效脚本清理", EditorStyles.boldLabel);
            GUILayout.Space(5);

            // 拖拽区域
            DrawDropArea();

            GUILayout.Space(10);

            // 显示已拖入的对象
            if (draggedObjects.Count > 0) {
                GUILayout.Label($"已添加 {draggedObjects.Count} 个对象:", EditorStyles.boldLabel);

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(100));
                for (int i = 0; i < draggedObjects.Count; i++) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(draggedObjects[i], typeof(Object), false);
                    if (GUILayout.Button("移除", GUILayout.Width(50))) {
                        draggedObjects.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();

                GUILayout.Space(10);

                // 操作按钮
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("清理失效脚本", GUILayout.Height(30))) {
                    CleanMissingSripts();
                }
                if (GUILayout.Button("清空列表", GUILayout.Height(30))) {
                    ClearList();
                }
                EditorGUILayout.EndHorizontal();
            }

            // 显示处理结果
            if (processedFiles.Count > 0) {
                GUILayout.Space(10);
                GUILayout.Label($"处理结果: 共发现 {totalPrefabsFound} 个预制体，清理了 {cleanedPrefabsCount} 个", EditorStyles.boldLabel);

                showProcessedFiles = EditorGUILayout.Foldout(showProcessedFiles, "查看处理的文件列表");
                if (showProcessedFiles) {
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
                    foreach (string file in processedFiles) {
                        EditorGUILayout.LabelField(file);
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
        }

        void DrawFontReplacerPage() {
            GUILayout.Label("字体替换", EditorStyles.boldLabel);
            GUILayout.Space(5);

            // 目标字体选择
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("目标字体 (将替换为此字体):", EditorStyles.boldLabel);

            // 字体拖拽区域
            DrawFontDropArea();

            // 字体对象字段
            Font newFont = (Font)EditorGUILayout.ObjectField("或选择字体:", targetFont, typeof(Font), false);
            if (newFont != targetFont) {
                targetFont = newFont;
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // 预制体拖拽区域
            DrawDropArea();

            GUILayout.Space(10);

            // 显示已拖入的对象
            if (draggedObjects.Count > 0) {
                GUILayout.Label($"已添加 {draggedObjects.Count} 个对象:", EditorStyles.boldLabel);

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(100));
                for (int i = 0; i < draggedObjects.Count; i++) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(draggedObjects[i], typeof(Object), false);
                    if (GUILayout.Button("移除", GUILayout.Width(50))) {
                        draggedObjects.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();

                GUILayout.Space(10);

                // 操作按钮
                EditorGUILayout.BeginHorizontal();
                GUI.enabled = targetFont != null;
                if (GUILayout.Button("替换字体", GUILayout.Height(30))) {
                    ReplaceFonts();
                }
                GUI.enabled = true;
                if (GUILayout.Button("清空列表", GUILayout.Height(30))) {
                    ClearList();
                }
                EditorGUILayout.EndHorizontal();

                if (targetFont == null) {
                    EditorGUILayout.HelpBox("请先选择要替换的目标字体", MessageType.Warning);
                }
            }

            // 显示字体替换结果
            if (fontReplacedFiles.Count > 0) {
                GUILayout.Space(10);
                GUILayout.Label($"替换结果: 共发现 {totalTextsFound} 个Text组件，替换了 {replacedTextsCount} 个", EditorStyles.boldLabel);

                showProcessedFiles = EditorGUILayout.Foldout(showProcessedFiles, "查看处理的文件列表");
                if (showProcessedFiles) {
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
                    foreach (string file in fontReplacedFiles) {
                        EditorGUILayout.LabelField(file);
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
        }

        void DrawDropArea() {
            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));

            GUI.Box(dropArea, "将文件夹或预制体拖拽到此处", EditorStyles.helpBox);

            switch (evt.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();

                        foreach (Object draggedObject in DragAndDrop.objectReferences) {
                            if (draggedObject != null && !draggedObjects.Contains(draggedObject)) {
                                // 检查是否为文件夹或预制体
                                string path = AssetDatabase.GetAssetPath(draggedObject);
                                if (AssetDatabase.IsValidFolder(path) ||
                                    (draggedObject is GameObject && PrefabUtility.GetPrefabAssetType(draggedObject) != PrefabAssetType.NotAPrefab)) {
                                    draggedObjects.Add(draggedObject);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        void DrawFontDropArea() {
            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));

            GUI.Box(dropArea, "将字体文件拖拽到此处", EditorStyles.helpBox);

            switch (evt.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();

                        foreach (Object draggedObject in DragAndDrop.objectReferences) {
                            if (draggedObject is Font) {
                                targetFont = draggedObject as Font;
                                break;
                            }
                        }
                    }
                    break;
            }
        }

        void ClearCurrentPageData() {
            switch (currentPage) {
                case 0:
                    processedFiles.Clear();
                    totalPrefabsFound = 0;
                    cleanedPrefabsCount = 0;
                    break;
                case 1:
                    fontReplacedFiles.Clear();
                    totalTextsFound = 0;
                    replacedTextsCount = 0;
                    break;
            }
            showProcessedFiles = false;
        }

        void CleanMissingSripts() {
            if (draggedObjects.Count == 0) {
                EditorUtility.DisplayDialog("提示", "请先拖入文件夹或预制体", "确定");
                return;
            }

            processedFiles.Clear();
            totalPrefabsFound = 0;
            cleanedPrefabsCount = 0;

            try {
                EditorUtility.DisplayProgressBar("处理中", "正在搜索预制体...", 0);

                List<string> allPrefabPaths = new List<string>();

                // 收集所有预制体路径
                foreach (Object obj in draggedObjects) {
                    string path = AssetDatabase.GetAssetPath(obj);

                    if (AssetDatabase.IsValidFolder(path)) {
                        // 如果是文件夹，递归查找所有预制体
                        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { path });
                        foreach (string guid in prefabGuids) {
                            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                            allPrefabPaths.Add(prefabPath);
                        }
                    }
                    else if (obj is GameObject) {
                        // 如果是预制体，直接添加
                        allPrefabPaths.Add(path);
                    }
                }

                totalPrefabsFound = allPrefabPaths.Count;

                // 处理每个预制体
                for (int i = 0; i < allPrefabPaths.Count; i++) {
                    string prefabPath = allPrefabPaths[i];
                    float progress = (float)i / allPrefabPaths.Count;

                    EditorUtility.DisplayProgressBar("处理中",
                        $"正在处理: {Path.GetFileName(prefabPath)} ({i + 1}/{allPrefabPaths.Count})",
                        progress);

                    if (CleanPrefabMissingScripts(prefabPath)) {
                        cleanedPrefabsCount++;
                        processedFiles.Add($"✓ {prefabPath}");
                    }
                    else {
                        processedFiles.Add($"○ {prefabPath}");
                    }
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("完成",
                    $"处理完成！\n共处理 {totalPrefabsFound} 个预制体\n清理了 {cleanedPrefabsCount} 个包含失效脚本的预制体",
                    "确定");
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
        }

        void ReplaceFonts() {
            if (draggedObjects.Count == 0 || targetFont == null) {
                EditorUtility.DisplayDialog("提示", "请先拖入文件夹或预制体，并选择目标字体", "确定");
                return;
            }

            fontReplacedFiles.Clear();
            totalTextsFound = 0;
            replacedTextsCount = 0;

            try {
                EditorUtility.DisplayProgressBar("处理中", "正在搜索预制体...", 0);

                List<string> allPrefabPaths = new List<string>();

                // 收集所有预制体路径
                foreach (Object obj in draggedObjects) {
                    string path = AssetDatabase.GetAssetPath(obj);

                    if (AssetDatabase.IsValidFolder(path)) {
                        // 如果是文件夹，递归查找所有预制体
                        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { path });
                        foreach (string guid in prefabGuids) {
                            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                            allPrefabPaths.Add(prefabPath);
                        }
                    }
                    else if (obj is GameObject) {
                        // 如果是预制体，直接添加
                        allPrefabPaths.Add(path);
                    }
                }

                // 处理每个预制体
                for (int i = 0; i < allPrefabPaths.Count; i++) {
                    string prefabPath = allPrefabPaths[i];
                    float progress = (float)i / allPrefabPaths.Count;

                    EditorUtility.DisplayProgressBar("处理中",
                        $"正在处理: {Path.GetFileName(prefabPath)} ({i + 1}/{allPrefabPaths.Count})",
                        progress);

                    int replacedInThisFile = ReplaceFontInPrefab(prefabPath);
                    if (replacedInThisFile > 0) {
                        fontReplacedFiles.Add($"✓ {prefabPath} ({replacedInThisFile} 个Text组件)");
                        replacedTextsCount += replacedInThisFile;
                    }
                    else {
                        fontReplacedFiles.Add($"○ {prefabPath}");
                    }
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("完成",
                    $"字体替换完成！\n共处理 {allPrefabPaths.Count} 个预制体\n发现 {totalTextsFound} 个Text组件\n替换了 {replacedTextsCount} 个字体",
                    "确定");
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
        }

        int ReplaceFontInPrefab(string prefabPath) {
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefabAsset == null) return 0;

            int replacedCount = 0;

            try {
                // 创建预制体实例
                GameObject tempInstance = PrefabUtility.InstantiatePrefab(prefabAsset) as GameObject;

                if (tempInstance != null) {
                    // 获取所有UGUI Text组件（不包括TextMeshPro）
                    UnityEngine.UI.Text[] textComponents = tempInstance.GetComponentsInChildren<UnityEngine.UI.Text>(true);
                    totalTextsFound += textComponents.Length;

                    foreach (UnityEngine.UI.Text textComp in textComponents) {
                        if (textComp.font != targetFont) {
                            textComp.font = targetFont;
                            replacedCount++;
                        }
                    }

                    // 如果有修改，应用到预制体
                    if (replacedCount > 0) {
                        PrefabUtility.ApplyPrefabInstance(tempInstance, InteractionMode.AutomatedAction);
                    }

                    // 清理临时实例
                    DestroyImmediate(tempInstance);
                }
            }
            catch (System.Exception e) {
                Debug.LogError($"处理预制体字体时出错 {prefabPath}: {e.Message}");
                return 0;
            }

            return replacedCount;
        }

        bool CleanPrefabMissingScripts(string prefabPath) {
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefabAsset == null) return false;

            bool hasMissingScripts = false;
            bool wasModified = false;

            try {
                // 直接使用AssetDatabase操作，不加载PrefabContents
                // 先检查是否有失效脚本
                if (HasMissingScripts(prefabAsset)) {
                    hasMissingScripts = true;

                    // 使用GameObjectUtility移除失效脚本
                    GameObject tempInstance = PrefabUtility.InstantiatePrefab(prefabAsset) as GameObject;

                    if (tempInstance != null) {
                        // 递归移除所有失效脚本
                        int removedCount = RemoveMissingScriptsRecursive(tempInstance);

                        if (removedCount > 0) {
                            // 应用修改到预制体
                            PrefabUtility.ApplyPrefabInstance(tempInstance, InteractionMode.AutomatedAction);
                            wasModified = true;
                        }

                        // 清理临时实例
                        DestroyImmediate(tempInstance);
                    }
                }
            }
            catch (System.Exception e) {
                Debug.LogError($"处理预制体时出错 {prefabPath}: {e.Message}");
                return false;
            }

            return hasMissingScripts;
        }

        bool HasMissingScripts(GameObject prefab) {
            Transform[] allTransforms = prefab.GetComponentsInChildren<Transform>(true);

            foreach (Transform t in allTransforms) {
                Component[] components = t.GetComponents<Component>();
                foreach (Component comp in components) {
                    if (comp == null) {
                        return true;
                    }
                }
            }
            return false;
        }

        int RemoveMissingScriptsRecursive(GameObject go) {
            int totalRemoved = 0;

            // 处理当前GameObject
            totalRemoved += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);

            // 递归处理所有子对象
            Transform[] children = go.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children) {
                if (child != go.transform) {
                    totalRemoved += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(child.gameObject);
                }
            }

            return totalRemoved;
        }

        void ClearList() {
            draggedObjects.Clear();
            ClearCurrentPageData();
        }
    }
}