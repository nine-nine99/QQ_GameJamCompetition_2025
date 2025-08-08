using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine.UI;

namespace HW.HWEditor {
    /// <summary>
    /// WindowInfo组件的自定义编辑器
    /// 提供窗口配置界面和自动配置UI组件功能
    /// </summary>
    [CustomEditor(typeof(WindowInfo))]
    public class WindowInfoInspector : Editor {
        // 当前编辑的WindowInfo组件
        WindowInfo windowInfo;

        // 动画设置的界面值
        float minVal = 0f;
        float maxVal = 10.0f;
        bool animSet = true;

        // 自动配置相关的字段
        bool autoConfigFoldout = false;
        private SerializedProperty childObjectsProperty;

        // 用于拖拽新物体到列表中
        private GameObject newChildObject = null;

        /// <summary>
        /// 编辑器被启用时调用
        /// </summary>
        private void OnEnable() {
            windowInfo = (WindowInfo)target;

            // 尝试获取childObjects属性
            serializedObject.Update();
            childObjectsProperty = serializedObject.FindProperty("childObjects");
        }

        /// <summary>
        /// 绘制Inspector GUI界面
        /// </summary>
        public override void OnInspectorGUI() {
            //base.OnInspectorGUI();
            serializedObject.Update();

            // 检查子物体列表，移除已销毁或失效的物体
            CleanupChildObjectsList();

            EditorGUILayout.BeginVertical();

            //空两行
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            windowInfo.windowType = (WindowType)EditorGUILayout.EnumPopup("Window Type", windowInfo.windowType);
            EditorGUILayout.Space();

            // 动画设置折叠面板
            animSet = EditorGUILayout.Foldout(animSet, "Window Anim", true);
            if (animSet) {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                windowInfo.openAnimType = (OpenAnimType)EditorGUILayout.EnumPopup("Open Anim Type", windowInfo.openAnimType);
                windowInfo.closeAnimType = (OpenAnimType)EditorGUILayout.EnumPopup("Close Anim Type", windowInfo.closeAnimType);
                if (windowInfo.openAnimType != OpenAnimType.None || windowInfo.closeAnimType != OpenAnimType.None) {
                    windowInfo.animTime = EditorGUILayout.Slider("Anim Time", windowInfo.animTime, minVal, maxVal);
                }

                if (windowInfo.openAnimType == OpenAnimType.Position || windowInfo.closeAnimType == OpenAnimType.Position) {
                    if (windowInfo.defaultPos == Vector3.zero && windowInfo.openPos == Vector3.zero) {
                        EditorGUILayout.HelpBox("位移动画需要填入位置信息！！", MessageType.Warning);
                    }
                    windowInfo.defaultPos = EditorGUILayout.Vector3Field("Default Pos", windowInfo.defaultPos);
                    windowInfo.openPos = EditorGUILayout.Vector3Field("Open Pos", windowInfo.openPos);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space();

            windowInfo.closeOnEmpty = EditorGUILayout.Toggle("Close On Empty", windowInfo.closeOnEmpty);
            windowInfo.mask = EditorGUILayout.Toggle("Mask", windowInfo.mask);
            windowInfo.group = EditorGUILayout.IntField("Group", windowInfo.group);

            // 自动配置折叠面板
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUI.backgroundColor = new Color(0.8f, 0.9f, 1f); // 设置一个浅蓝色背景以突出显示
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.backgroundColor = Color.white; // 恢复正常颜色

            autoConfigFoldout = EditorGUILayout.Foldout(autoConfigFoldout, "自动配置", true);
            if (autoConfigFoldout) {
                EditorGUILayout.BeginVertical();

                // 子物体列表标题
                EditorGUILayout.LabelField("子物体列表", EditorStyles.boldLabel);

                EditorGUILayout.HelpBox("此列表仅在编辑器中使用，不会打包到游戏中。已自动移除失效物体。", MessageType.Info);

                // 如果childObjects属性存在则处理
                if (childObjectsProperty != null) {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    EditorGUILayout.LabelField("子物体列表", EditorStyles.boldLabel);
                    EditorGUILayout.HelpBox("将子物体直接拖入下方列表中，支持多选拖拽。", MessageType.Info);

                    // 显示列表元素并允许直接拖拽添加
                    EditorGUI.indentLevel++;

                    // 确定列表项的高度
                    float lineHeight = EditorGUIUtility.singleLineHeight;
                    float padding = 2f;

                    // 创建可滚动区域，限制最大高度
                    int maxVisibleItems = Mathf.Min(10, childObjectsProperty.arraySize + 1); // +1 用于显示空行
                    float listHeight = (lineHeight + padding * 2) * maxVisibleItems;

                    EditorGUILayout.BeginVertical(GUILayout.Height(listHeight));

                    // 现有元素显示
                    for (int i = 0; i < childObjectsProperty.arraySize; i++) {
                        EditorGUILayout.BeginHorizontal();

                        SerializedProperty elementProp = childObjectsProperty.GetArrayElementAtIndex(i);

                        // 绘制元素属性
                        EditorGUILayout.PropertyField(elementProp, new GUIContent($"元素 {i}"), GUILayout.ExpandWidth(true));

                        // 检查是否为当前预制体的子物体
                        GameObject obj = elementProp.objectReferenceValue as GameObject;
                        if (obj != null && !IsChildOf(windowInfo.gameObject, obj)) {
                            EditorGUILayout.LabelField(new GUIContent("⚠", "这不是当前窗口的子物体！"), GUILayout.Width(15));
                        }

                        // 移除按钮
                        if (GUILayout.Button("×", GUILayout.Width(20))) {
                            childObjectsProperty.DeleteArrayElementAtIndex(i);
                            serializedObject.ApplyModifiedProperties();
                            GUIUtility.ExitGUI(); // 防止列表修改后的索引问题
                            break;
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    // 添加一个空元素拖拽区域
                    Rect dropAreaRect = EditorGUILayout.GetControlRect(GUILayout.Height(lineHeight + padding * 2));
                    GUI.Box(dropAreaRect, "拖动子物体到此处添加", EditorStyles.helpBox);

                    // 空元素区域的拖拽处理
                    HandleDragAndDrop(dropAreaRect, true);

                    EditorGUILayout.EndVertical();

                    EditorGUI.indentLevel--;

                    // 功能按钮区域
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    // 清空列表按钮
                    GUI.backgroundColor = new Color(1f, 0.7f, 0.7f); // 设置红色背景
                    if (GUILayout.Button("清空列表") && childObjectsProperty.arraySize > 0) {
                        if (EditorUtility.DisplayDialog("确认操作", "确定要清空整个列表吗？", "确定", "取消")) {
                            childObjectsProperty.ClearArray();
                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                    // 从层级视图中选择按钮
                    GUI.backgroundColor = new Color(0.7f, 0.9f, 0.7f); // 设置绿色背景
                    if (GUILayout.Button("从选中对象添加")) {
                        AddSelectedObjectsToList();
                    }
                    GUI.backgroundColor = Color.white; // 恢复正常颜色

                    // 清理无效引用按钮
                    if (GUILayout.Button("挂载窗口脚本")) {
                        AttachScriptToGameObject();
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();

                    // 处理整个列表区域的拖拽
                    Rect listRect = GUILayoutUtility.GetLastRect();
                    HandleDragAndDrop(listRect, false);
                }
                else {
                    // 属性不存在时显示警告
                    EditorGUILayout.HelpBox("找不到子物体列表属性。请更新WindowInfo脚本。", MessageType.Warning);
                }

                EditorGUILayout.Space();

                // 自动配置按钮
                GUI.backgroundColor = new Color(0.6f, 0.8f, 0.6f); // 设置绿色背景以突出显示
                if (GUILayout.Button("执行自动配置", GUILayout.Height(30))) {
                    AutoConfigure();
                }
                GUI.backgroundColor = Color.white; // 恢复正常颜色

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical(); // 关闭自动配置区域的帮助框

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed) {
                EditorUtility.SetDirty(windowInfo);
            }
        }

        /// <summary>
        /// 处理拖放操作
        /// </summary>
        /// <param name="dropArea">拖放区域</param>
        /// <param name="isEmptySlot">是否为空槽位拖放区域</param>
        private void HandleDragAndDrop(Rect dropArea, bool isEmptySlot) {
            Event evt = Event.current;
            switch (evt.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();

                        foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences) {
                            GameObject go = draggedObject as GameObject;
                            if (go != null) {
                                // 检查是否为当前预制体的子物体
                                if (IsChildOf(windowInfo.gameObject, go)) {
                                    // 检查物体是否已经在列表中
                                    bool alreadyExists = false;
                                    for (int i = 0; i < childObjectsProperty.arraySize; i++) {
                                        if (childObjectsProperty.GetArrayElementAtIndex(i).objectReferenceValue == go) {
                                            alreadyExists = true;
                                            break;
                                        }
                                    }

                                    if (!alreadyExists) {
                                        // 添加到列表
                                        childObjectsProperty.arraySize++;
                                        childObjectsProperty.GetArrayElementAtIndex(childObjectsProperty.arraySize - 1).objectReferenceValue = go;
                                        serializedObject.ApplyModifiedProperties();
                                    }
                                }
                                else {
                                    // 可以添加提示消息，表明只能添加子物体
                                    Debug.LogWarning($"只能添加当前窗口的子物体: {go.name}");
                                }
                            }
                        }

                        GUI.changed = true;
                    }
                    evt.Use();
                    break;
            }
        }

        /// <summary>
        /// 将当前在层级视图中选中的对象添加到列表中
        /// </summary>
        private void AddSelectedObjectsToList() {
            foreach (GameObject selected in Selection.gameObjects) {
                // 检查是否为当前预制体的子物体
                if (IsChildOf(windowInfo.gameObject, selected)) {
                    // 检查物体是否已经在列表中
                    bool alreadyExists = false;
                    for (int i = 0; i < childObjectsProperty.arraySize; i++) {
                        if (childObjectsProperty.GetArrayElementAtIndex(i).objectReferenceValue == selected) {
                            alreadyExists = true;
                            break;
                        }
                    }

                    if (!alreadyExists) {
                        // 添加到列表
                        childObjectsProperty.arraySize++;
                        childObjectsProperty.GetArrayElementAtIndex(childObjectsProperty.arraySize - 1).objectReferenceValue = selected;
                    }
                }
                else {
                    Debug.LogWarning($"只能添加当前窗口的子物体: {selected.name}");
                }
            }

            serializedObject.ApplyModifiedProperties();
        }


        /// <summary>
        /// 自动配置方法 - 处理脚本生成或更新
        /// </summary>
        private void AutoConfigure() {
            // 获取预制体名称
            string prefabName = windowInfo.gameObject.name;

            // 如果存在"Win_"前缀，则移除
            string scriptName = prefabName.StartsWith("Win_") ? prefabName.Substring(4) : prefabName;

            // 检查脚本是否已存在
            string scriptPath = $"Assets/Scripts/GamePlay/UI/{scriptName}.cs";
            bool scriptExists = File.Exists(scriptPath);

            if (scriptExists) {
                // 更新已存在的脚本
                UpdateExistingScript(scriptPath, scriptName);
            }
            else {
                // 生成新脚本
                GenerateNewScript(scriptPath, scriptName);
            }

            // 刷新资源数据库，确保Unity识别新文件
            AssetDatabase.Refresh();

            Debug.Log($"WindowInfo 自动配置完成: {prefabName}");
        }

        /// <summary>
        /// 将脚本挂载到GameObject上
        /// </summary>
        public void AttachScriptToGameObject() {
            string prefabName = windowInfo.gameObject.name;
            string scriptName = prefabName.StartsWith("Win_") ? prefabName.Substring(4) : prefabName;

            // 尝试通过类的全名查找类型
            Type scriptType = null;

            // 尝试在所有已加载的程序集中查找类型
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                scriptType = assembly.GetType(scriptName);
                if (scriptType != null) break;
            }

            // 如果找不到类型，尝试查找脚本资产
            if (scriptType == null) {
                // 查找脚本资产
                string scriptPath = $"Assets/Scripts/GamePlay/UI/{scriptName}.cs";
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);

                if (monoScript != null) {
                    scriptType = monoScript.GetClass();
                }
            }

            // 如果找到类型并且GameObject上还没有该组件，则添加
            if (scriptType != null && windowInfo.gameObject.GetComponent(scriptType) == null) {
                Debug.Log($"正在将脚本 {scriptName} 挂载到 {windowInfo.gameObject.name} 上");
                windowInfo.gameObject.AddComponent(scriptType);
                EditorUtility.SetDirty(windowInfo.gameObject);
            }
            else if (scriptType == null) {
                Debug.LogWarning($"无法找到脚本类型: {scriptName}，请手动添加组件");
            }
        }

        /// <summary>
        /// 生成新的窗口脚本
        /// </summary>
        /// <param name="scriptPath">脚本保存路径</param>
        /// <param name="scriptName">脚本类名（不含后缀）</param>
        private void GenerateNewScript(string scriptPath, string scriptName) {
            // 如果目录不存在，则创建目录
            string directory = Path.GetDirectoryName(scriptPath);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            // 收集子物体中的UI组件
            Dictionary<string, UIComponentInfo> uiComponents = CollectUIComponents();

            // 生成脚本内容
            StringBuilder scriptContent = new StringBuilder();

            // 脚本头部
            scriptContent.AppendLine("using System;");
            scriptContent.AppendLine("using System.Collections;");
            scriptContent.AppendLine("using System.Collections.Generic;");
            scriptContent.AppendLine("using UnityEngine;");
            scriptContent.AppendLine("using UnityEngine.UI;");
            scriptContent.AppendLine();

            // 类声明
            scriptContent.AppendLine($"/// <summary>");
            scriptContent.AppendLine($"/// {scriptName} 窗口类");
            scriptContent.AppendLine($"/// </summary>");
            scriptContent.AppendLine($"public class {scriptName} : BaseWindowWrapper<{scriptName}> {{");
            scriptContent.AppendLine();

            // UI组件属性
            foreach (var component in uiComponents) {
                scriptContent.AppendLine($"    private {component.Value.ComponentType} {component.Value.VariableName};");
            }
            scriptContent.AppendLine();

            // InitCtrl方法 - 初始化控件
            scriptContent.AppendLine("    /// <summary>");
            scriptContent.AppendLine("    /// 初始化控件引用");
            scriptContent.AppendLine("    /// </summary>");
            scriptContent.AppendLine("    protected override void InitCtrl() {");
            foreach (var component in uiComponents) {
                scriptContent.AppendLine($"        {component.Value.VariableName} = gameObject.GetChildControl<{component.Value.ComponentType}>(\"{component.Value.Path}\");");
            }
            scriptContent.AppendLine("    }");
            scriptContent.AppendLine();

            // OnPreOpen方法 - 窗口打开前调用
            scriptContent.AppendLine("    /// <summary>");
            scriptContent.AppendLine("    /// 窗口打开前调用");
            scriptContent.AppendLine("    /// </summary>");
            scriptContent.AppendLine("    protected override void OnPreOpen() {");
            scriptContent.AppendLine("    }");
            scriptContent.AppendLine();

            // OnOpen方法 - 窗口打开时调用
            scriptContent.AppendLine("    /// <summary>");
            scriptContent.AppendLine("    /// 窗口打开时调用");
            scriptContent.AppendLine("    /// </summary>");
            scriptContent.AppendLine("    protected override void OnOpen() {");
            scriptContent.AppendLine("    }");
            scriptContent.AppendLine();

            // OnClose方法 - 窗口关闭时调用
            scriptContent.AppendLine("    /// <summary>");
            scriptContent.AppendLine("    /// 窗口关闭时调用");
            scriptContent.AppendLine("    /// </summary>");
            scriptContent.AppendLine("    protected override void OnClose() {");
            scriptContent.AppendLine("        base.OnClose();");
            scriptContent.AppendLine("    }");
            scriptContent.AppendLine();

            // InitMsg方法 - 初始化消息监听
            scriptContent.AppendLine("    /// <summary>");
            scriptContent.AppendLine("    /// 初始化消息监听");
            scriptContent.AppendLine("    /// </summary>");
            scriptContent.AppendLine("    protected override void InitMsg() {");
            foreach (var component in uiComponents.Where(c => c.Value.ComponentType == "Button")) {
                scriptContent.AppendLine($"        {component.Value.VariableName}.onClick.AddListener(On{char.ToUpper(component.Value.VariableName[0])}{component.Value.VariableName.Substring(1)}Click);");
            }
            scriptContent.AppendLine("    }");
            scriptContent.AppendLine();

            // ClearMsg方法 - 清除消息监听
            scriptContent.AppendLine("    /// <summary>");
            scriptContent.AppendLine("    /// 清除消息监听");
            scriptContent.AppendLine("    /// </summary>");
            scriptContent.AppendLine("    protected override void ClearMsg() {");
            foreach (var component in uiComponents.Where(c => c.Value.ComponentType == "Button")) {
                scriptContent.AppendLine($"        {component.Value.VariableName}.onClick.RemoveListener(On{char.ToUpper(component.Value.VariableName[0])}{component.Value.VariableName.Substring(1)}Click);");
            }
            scriptContent.AppendLine("    }");
            scriptContent.AppendLine();

            // 按钮点击处理方法
            foreach (var component in uiComponents.Where(c => c.Value.ComponentType == "Button")) {
                scriptContent.AppendLine($"    /// <summary>");
                scriptContent.AppendLine($"    /// {component.Value.VariableName} 按钮点击事件处理");
                scriptContent.AppendLine($"    /// </summary>");
                scriptContent.AppendLine($"    private void On{char.ToUpper(component.Value.VariableName[0])}{component.Value.VariableName.Substring(1)}Click() {{");
                scriptContent.AppendLine("    }");
                scriptContent.AppendLine();
            }

            // 关闭类
            scriptContent.AppendLine("}");

            // 写入文件
            File.WriteAllText(scriptPath, scriptContent.ToString());

            Debug.Log($"已创建新脚本: {scriptPath}");
        }

        /// <summary>
        /// 更新已存在的窗口脚本，使用路径+名称作为唯一标识
        /// </summary>
        /// <param name="scriptPath">脚本路径</param>
        /// <param name="scriptName">脚本类名（不含后缀）</param>
        private void UpdateExistingScript(string scriptPath, string scriptName) {
            // 读取现有脚本内容
            string existingScript = File.ReadAllText(scriptPath);

            // 收集子物体中的UI组件
            Dictionary<string, UIComponentInfo> uiComponents = CollectUIComponents();

            // 解析现有脚本，识别已存在的UI组件及其路径
            Dictionary<string, UIComponentInfo> existingComponents = ParseExistingComponentsWithPath(existingScript);

            // 找出需要添加的组件
            var componentsToAdd = new List<UIComponentInfo>();

            foreach (var component in uiComponents) {
                string uniqueKey = component.Key;

                // 如果这个路径_变量名的组合不存在，则添加
                if (!existingComponents.ContainsKey(uniqueKey)) {
                    // 检查是否有相同路径但不同变量名的组件
                    bool pathExists = false;
                    foreach (var existing in existingComponents) {
                        if (existing.Value.Path == component.Value.Path) {
                            pathExists = true;
                            Debug.Log($"跳过路径已存在的组件: {component.Value.VariableName}, 路径: {component.Value.Path}, 已存在变量: {existing.Value.VariableName}");
                            break;
                        }
                    }

                    // 检查是否有相同变量名但不同路径的组件
                    bool nameExists = false;
                    foreach (var existing in existingComponents) {
                        if (existing.Value.VariableName == component.Value.VariableName &&
                            existing.Value.Path != component.Value.Path) {
                            nameExists = true;

                            // 生成新的变量名
                            string baseVarName = component.Value.VariableName;
                            int suffix = 2;
                            string newVarName;

                            do {
                                newVarName = $"{baseVarName}{suffix}";
                                suffix++;
                            } while (existingComponents.Values.Any(v => v.VariableName == newVarName));

                            // 使用新名称
                            UIComponentInfo renamedComponent = new UIComponentInfo {
                                ComponentType = component.Value.ComponentType,
                                Path = component.Value.Path,
                                VariableName = newVarName
                            };

                            componentsToAdd.Add(renamedComponent);
                            Debug.Log($"添加重命名组件: {baseVarName} → {newVarName}, 路径: {component.Value.Path}");
                            break;
                        }
                    }

                    // 如果既没有相同路径也没有相同变量名的组件，直接添加
                    if (!pathExists && !nameExists) {
                        componentsToAdd.Add(component.Value);
                        Debug.Log($"添加新组件: {component.Value.VariableName}, 路径: {component.Value.Path}");
                    }
                }
                else {
                    Debug.Log($"跳过重复组件: {component.Value.VariableName}, 路径: {component.Value.Path}");
                }
            }

            if (componentsToAdd.Count == 0) {
                Debug.Log("没有新组件需要添加到现有脚本中。");
                return;
            }

            // 脚本更新部分 - 流程与之前相同，但使用新添加的组件列表

            // 首先识别类的声明部分和第一个方法的位置
            int classDeclarationStart = existingScript.IndexOf($"public class {scriptName}");
            if (classDeclarationStart < 0) {
                Debug.LogError($"无法在脚本中找到类定义: public class {scriptName}");
                return;
            }

            // 寻找类的开始大括号
            int classBodyStart = existingScript.IndexOf('{', classDeclarationStart);
            if (classBodyStart < 0) {
                Debug.LogError("无法找到类体的开始位置");
                return;
            }

            // 查找第一个方法声明的位置
            string[] methodIdentifiers = new string[] {
        "protected override void InitCtrl()",
        "protected override void OnPreOpen()",
        "protected override void OnOpen()",
        "protected override void OnClose()",
        "protected override void InitMsg()",
        "protected override void ClearMsg()"
    };

            int firstMethodPosition = int.MaxValue;
            foreach (var methodId in methodIdentifiers) {
                int pos = existingScript.IndexOf(methodId, classBodyStart);
                if (pos > 0 && pos < firstMethodPosition) {
                    firstMethodPosition = pos;
                }
            }

            if (firstMethodPosition == int.MaxValue) {
                // 如果找不到方法，就在类体结束前添加
                firstMethodPosition = existingScript.LastIndexOf('}');
                if (firstMethodPosition <= classBodyStart) {
                    Debug.LogError("无法确定在哪里插入新组件");
                    return;
                }
            }

            // 找到组件声明部分的结束位置（在第一个方法之前）
            // 找到最后一个私有组件声明
            int lastComponentDeclaration = -1;
            string searchPattern = "private ";
            int tempIndex = existingScript.IndexOf(searchPattern, classBodyStart);
            while (tempIndex >= 0 && tempIndex < firstMethodPosition) {
                int endOfLine = existingScript.IndexOf(';', tempIndex);
                if (endOfLine > lastComponentDeclaration) {
                    lastComponentDeclaration = endOfLine + 1; // +1 to include the semicolon
                }
                tempIndex = existingScript.IndexOf(searchPattern, tempIndex + 1);
            }

            // 确定在哪里插入新组件声明
            int insertPosition;
            if (lastComponentDeclaration > 0) {
                // 在最后一个组件之后插入
                insertPosition = lastComponentDeclaration;
            }
            else {
                // 在类声明之后立即插入
                insertPosition = classBodyStart + 1;
            }

            // 构建新组件声明
            StringBuilder newComponents = new StringBuilder();
            newComponents.AppendLine();
            foreach (var component in componentsToAdd) {
                newComponents.AppendLine($"    private {component.ComponentType} {component.VariableName};");
            }

            // 更新脚本内容
            string updatedScript = existingScript.Insert(insertPosition, newComponents.ToString());

            // 更新InitCtrl方法
            int initCtrlStart = updatedScript.IndexOf("protected override void InitCtrl()");
            if (initCtrlStart >= 0) {
                int initCtrlBodyStart = updatedScript.IndexOf('{', initCtrlStart);
                int initCtrlBodyEnd = FindMatchingBrace(updatedScript, initCtrlBodyStart);

                if (initCtrlBodyStart >= 0 && initCtrlBodyEnd > initCtrlBodyStart) {
                    StringBuilder initCtrlAdditions = new StringBuilder();
                    foreach (var component in componentsToAdd) {
                        initCtrlAdditions.AppendLine($"        {component.VariableName} = gameObject.GetChildControl<{component.ComponentType}>(\"{component.Path}\");");
                    }

                    // 插入在方法体的右括号前
                    updatedScript = updatedScript.Insert(initCtrlBodyEnd, initCtrlAdditions.ToString());
                }
            }

            // 更新按钮的InitMsg和ClearMsg方法
            var buttonsToAdd = componentsToAdd.Where(c => c.ComponentType == "Button").ToList();
            if (buttonsToAdd.Count > 0) {
                // 更新InitMsg方法
                int initMsgStart = updatedScript.IndexOf("protected override void InitMsg()");
                if (initMsgStart >= 0) {
                    int initMsgBodyStart = updatedScript.IndexOf('{', initMsgStart);
                    int initMsgBodyEnd = FindMatchingBrace(updatedScript, initMsgBodyStart);

                    if (initMsgBodyStart >= 0 && initMsgBodyEnd > initMsgBodyStart) {
                        StringBuilder initMsgAdditions = new StringBuilder();
                        foreach (var button in buttonsToAdd) {
                            initMsgAdditions.AppendLine($"        {button.VariableName}.onClick.AddListener(On{char.ToUpper(button.VariableName[0])}{button.VariableName.Substring(1)}Click);");
                        }

                        // 插入在方法体的右括号前
                        updatedScript = updatedScript.Insert(initMsgBodyEnd, initMsgAdditions.ToString());
                    }
                }

                // 更新ClearMsg方法
                int clearMsgStart = updatedScript.IndexOf("protected override void ClearMsg()");
                if (clearMsgStart >= 0) {
                    int clearMsgBodyStart = updatedScript.IndexOf('{', clearMsgStart);
                    int clearMsgBodyEnd = FindMatchingBrace(updatedScript, clearMsgBodyStart);

                    if (clearMsgBodyStart >= 0 && clearMsgBodyEnd > clearMsgBodyStart) {
                        StringBuilder clearMsgAdditions = new StringBuilder();
                        foreach (var button in buttonsToAdd) {
                            clearMsgAdditions.AppendLine($"        {button.VariableName}.onClick.RemoveListener(On{char.ToUpper(button.VariableName[0])}{button.VariableName.Substring(1)}Click);");
                        }

                        // 插入在方法体的右括号前
                        updatedScript = updatedScript.Insert(clearMsgBodyEnd, clearMsgAdditions.ToString());
                    }
                }

                // 在类结束前添加按钮点击处理方法
                int classEndPos = updatedScript.LastIndexOf('}');
                if (classEndPos > 0) {
                    StringBuilder handlersToAdd = new StringBuilder();
                    foreach (var button in buttonsToAdd) {
                        handlersToAdd.AppendLine();
                        handlersToAdd.AppendLine($"    /// <summary>");
                        handlersToAdd.AppendLine($"    /// {button.VariableName} 按钮点击事件处理");
                        handlersToAdd.AppendLine($"    /// </summary>");
                        handlersToAdd.AppendLine($"    private void On{char.ToUpper(button.VariableName[0])}{button.VariableName.Substring(1)}Click() {{");
                        handlersToAdd.AppendLine("    }");
                    }

                    // 在类结束前插入
                    updatedScript = updatedScript.Insert(classEndPos, handlersToAdd.ToString());
                }
            }

            // 将更新后的脚本写回文件
            File.WriteAllText(scriptPath, updatedScript);

            Debug.Log($"已更新现有脚本: {scriptPath}");
        }


        /// <summary>
        /// 查找匹配的右大括号位置
        /// </summary>
        private int FindMatchingBrace(string text, int openBracePos) {
            if (text[openBracePos] != '{') {
                return -1;
            }

            int braceCount = 1;
            for (int i = openBracePos + 1; i < text.Length; i++) {
                if (text[i] == '{') {
                    braceCount++;
                }
                else if (text[i] == '}') {
                    braceCount--;
                    if (braceCount == 0) {
                        return i;
                    }
                }
            }

            return -1; // 没有找到匹配的右大括号
        }

        /// <summary>
        /// 收集子物体中的UI组件，使用改进的变量名生成逻辑，处理重复变量名
        /// </summary>
        private Dictionary<string, UIComponentInfo> CollectUIComponents() {
            Dictionary<string, UIComponentInfo> components = new Dictionary<string, UIComponentInfo>();

            // 定义组件类型的优先级顺序（数字越小优先级越高）
            Dictionary<string, int> componentPriority = new Dictionary<string, int> {
                { "Button", 1 },
                { "Toggle", 2 },
                { "Dropdown", 3 },
                { "InputField", 4 },
                { "Slider", 5 },
                { "Scrollbar", 6 },
                { "ScrollRect", 7 },
                { "Text", 8 },
                { "Image", 9 },
                { "RectTransform", 90 }, // 较低优先级，只有在没有其他UI组件时使用
                { "Transform", 100 }      // 最低优先级，只有在没有其他任何组件时使用
            };

            // 获取需要处理的子物体列表
            List<GameObject> objectsToProcess = new List<GameObject>();

            // 尝试从序列化属性中获取子物体列表
            if (childObjectsProperty != null) {
                SerializedProperty arraySizeProp = childObjectsProperty.FindPropertyRelative("Array.size");
                for (int i = 0; i < arraySizeProp.intValue; i++) {
                    SerializedProperty elementProp = childObjectsProperty.GetArrayElementAtIndex(i);
                    GameObject obj = elementProp.objectReferenceValue as GameObject;
                    if (obj != null) {
                        objectsToProcess.Add(obj);
                    }
                }
            }

            // 跟踪已使用的变量名基础部分（不含数字后缀），用于处理重复
            Dictionary<string, int> baseNameCounter = new Dictionary<string, int>();

            // 跟踪最终使用的变量名，避免重复
            HashSet<string> usedVariableNames = new HashSet<string>();

            // 为每个GameObject收集所有组件，并按优先级排序
            foreach (var childObject in objectsToProcess) {
                // 获取相对于窗口对象的路径
                string path = GetRelativePath(windowInfo.gameObject, childObject);

                // 收集该GameObject上的所有UI组件
                List<ComponentInfo> objectComponents = new List<ComponentInfo>();

                // 检查UI组件
                Component[] uiComponents = childObject.GetComponents<Component>();
                foreach (var component in uiComponents) {
                    if (component == null) continue;

                    string componentType = component.GetType().Name;

                    // 只处理UI组件
                    if (IsUIComponent(componentType)) {
                        objectComponents.Add(new ComponentInfo {
                            Component = component,
                            Type = componentType,
                            Priority = componentPriority.ContainsKey(componentType) ? componentPriority[componentType] : 100,
                            Path = path
                        });
                    }
                }

                // 按优先级排序组件（优先级数字小的排前面）
                objectComponents.Sort((a, b) => a.Priority.CompareTo(b.Priority));

                // 只处理优先级最高的组件（列表中的第一个）
                if (objectComponents.Count > 0) {
                    var topComponent = objectComponents[0];

                    // 生成简洁的变量名 - 不包含路径信息，只包含有意义的最后一级父节点
                    string baseName = GenerateSimpleVariableName(childObject.name, topComponent.Type, path);

                    // RectTransform和Transform使用特殊的前缀
                    if (topComponent.Type == "RectTransform") {
                        baseName = "rect" + (baseName.Length > 0 && char.IsLower(baseName[0]) ?
                            char.ToUpper(baseName[0]) + baseName.Substring(1) : baseName);
                    }
                    else if (topComponent.Type == "Transform") {
                        baseName = "trans" + (baseName.Length > 0 && char.IsLower(baseName[0]) ?
                            char.ToUpper(baseName[0]) + baseName.Substring(1) : baseName);
                    }

                    baseName = HandleReservedNames(baseName);

                    // 这里使用处理后的变量名作为基础变量名
                    if (!baseNameCounter.ContainsKey(baseName)) {
                        baseNameCounter[baseName] = 1;
                    }
                    else {
                        baseNameCounter[baseName]++;
                    }

                    // 构建最终变量名，对于重复的变量名添加数字后缀
                    string finalVarName = baseName;
                    if (baseNameCounter[baseName] > 1) {
                        finalVarName = $"{baseName}{baseNameCounter[baseName] - 1}";
                    }

                    // 确保变量名唯一
                    int suffix = 1;
                    string originalVarName = finalVarName;
                    while (usedVariableNames.Contains(finalVarName)) {
                        finalVarName = $"{originalVarName}{suffix}";
                        suffix++;
                    }

                    usedVariableNames.Add(finalVarName);

                    // 只使用路径作为键，不影响变量名
                    string uniqueKey = $"{CleanPath(path)}_{finalVarName}";

                    components.Add(uniqueKey, new UIComponentInfo {
                        ComponentType = topComponent.Type,
                        Path = path,
                        VariableName = finalVarName
                    });
                }
            }

            // 创建一个有序的组件列表，按照组件类型的优先级排序
            var sortedComponents = new Dictionary<string, UIComponentInfo>();

            // 按照优先级顺序依次添加组件
            foreach (var priority in componentPriority) {
                string type = priority.Key;
                foreach (var kvp in components.Where(c => c.Value.ComponentType == type)) {
                    sortedComponents.Add(kvp.Key, kvp.Value);
                }
            }

            // 添加所有其他未在优先级列表中的组件
            foreach (var kvp in components) {
                if (!sortedComponents.ContainsKey(kvp.Key)) {
                    sortedComponents.Add(kvp.Key, kvp.Value);
                }
            }

            return sortedComponents;
        }

        /// <summary>
        /// 获取子物体相对于根物体的路径
        /// </summary>
        /// <param name="root">根物体</param>
        /// <param name="child">子物体</param>
        /// <returns>相对路径字符串</returns>
        private string GetRelativePath(GameObject root, GameObject child) {
            if (child == null || root == null) return "";
            if (child == root) return "";

            Transform current = child.transform;
            List<string> pathParts = new List<string>();

            // 收集从子物体到父物体的路径
            pathParts.Add(current.name);

            Transform parent = current.parent;
            while (parent != null && parent != root.transform) {
                pathParts.Add(parent.name);
                parent = parent.parent;
            }

            // 如果没有找到根物体，说明不是子物体关系
            if (parent != root.transform) {
                return "";
            }

            // 反转路径并用斜杠连接
            pathParts.Reverse();
            return string.Join("/", pathParts);
        }
        /// <summary>
        /// 判断组件类型是否为UI组件
        /// </summary>
        /// <param name="componentType">组件类型名称</param>
        /// <returns>是否为UI组件</returns>
        private bool IsUIComponent(string componentType) {
            // 我们关注的UI组件类型列表
            string[] uiTypes = { "Button", "Text", "Image", "Toggle", "Slider", "Dropdown", "InputField", "ScrollRect", "Scrollbar", "RectTransform", "Transform" };
            return uiTypes.Contains(componentType);
        }

        /// <summary>
        /// 检查target是否为parent的子物体
        /// </summary>
        private bool IsChildOf(GameObject parent, GameObject target) {
            if (target == null || parent == null) return false;
            if (target == parent) return true;

            Transform targetTransform = target.transform;
            Transform current = targetTransform.parent;

            while (current != null) {
                if (current.gameObject == parent) {
                    return true;
                }
                current = current.parent;
            }

            return false;
        }

        /// <summary>
        /// 检查并清理子物体列表中的无效引用
        /// </summary>
        private void CleanupChildObjectsList() {
            bool hasNullEntries = false;

            if (childObjectsProperty != null && childObjectsProperty.isArray) {
                int arraySize = childObjectsProperty.arraySize;
                for (int i = arraySize - 1; i >= 0; i--) {
                    SerializedProperty elementProp = childObjectsProperty.GetArrayElementAtIndex(i);
                    if (elementProp.objectReferenceValue == null) {
                        hasNullEntries = true;
                        childObjectsProperty.DeleteArrayElementAtIndex(i);
                    }
                }

                if (hasNullEntries) {
                    serializedObject.ApplyModifiedProperties();
                    Debug.Log("已自动移除失效的子物体引用");
                }
            }
        }

        /// <summary>
        /// 解析现有脚本中的组件变量及其路径
        /// </summary>
        /// <param name="script">脚本内容</param>
        /// <returns>已存在的组件信息字典，键为"路径_变量名"的组合，值为组件信息</returns>
        private Dictionary<string, UIComponentInfo> ParseExistingComponentsWithPath(string script) {
            Dictionary<string, UIComponentInfo> components = new Dictionary<string, UIComponentInfo>();

            // 解析脚本以查找组件声明和对应的GetChildControl调用
            string[] lines = script.Split('\n');

            // 首先找到所有私有组件声明和它们的类型
            Dictionary<string, string> varTypes = new Dictionary<string, string>();
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i].Trim();

                // 查找私有组件声明
                if (line.StartsWith("private ") && line.EndsWith(";")) {
                    // 提取变量名和类型
                    string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3) {
                        string type = parts[1];
                        string varName = parts[2].TrimEnd(';');

                        varTypes[varName] = type;
                    }
                }
            }

            // 然后查找InitCtrl方法中的GetChildControl调用来获取路径
            bool inInitCtrl = false;
            int braceCount = 0;

            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i].Trim();

                // 检查是否进入InitCtrl方法
                if (line.Contains("protected override void InitCtrl()")) {
                    inInitCtrl = true;
                    continue;
                }

                // 如果在InitCtrl方法内，计算大括号
                if (inInitCtrl) {
                    if (line.Contains("{")) braceCount++;
                    if (line.Contains("}")) braceCount--;

                    // 如果braceCount回到0，说明已经离开了InitCtrl方法
                    if (braceCount == 0 && line.Contains("}")) {
                        inInitCtrl = false;
                        continue;
                    }

                    // 查找GetChildControl调用
                    foreach (var varEntry in varTypes) {
                        string varName = varEntry.Key;
                        string type = varEntry.Value;

                        if (line.Contains($"{varName} = gameObject.GetChildControl<") ||
                            line.Contains($"{varName}=gameObject.GetChildControl<")) {
                            // 提取路径字符串
                            int pathStart = line.IndexOf("\"");
                            int pathEnd = line.LastIndexOf("\"");
                            if (pathStart >= 0 && pathEnd > pathStart) {
                                string path = line.Substring(pathStart + 1, pathEnd - pathStart - 1);

                                // 使用路径_变量名作为唯一标识
                                string uniqueKey = $"{path}_{varName}";

                                components[uniqueKey] = new UIComponentInfo {
                                    ComponentType = type,
                                    Path = path,
                                    VariableName = varName
                                };

                                // 调试日志
                                Debug.Log($"解析到现有组件: {type}, 变量名: {varName}, 路径: {path}, 键: {uniqueKey}");
                            }
                        }
                    }
                }
            }

            return components;
        }

        /// <summary>
        /// 生成简洁的变量名，避免使用完整路径，只包含有意义的父级名称
        /// </summary>
        private string GenerateSimpleVariableName(string objectName, string componentType, string path) {
            // 清理对象名称
            string cleanName = CleanObjectName(objectName);

            // 基础变量名
            string baseName = GetBaseNameForComponent(cleanName, componentType);

            // 检查是否生成的基础变量名和对象名称本身相似（避免btnClose_btnClose这种情况）
            string lowerBaseName = baseName.ToLower();
            string lowerObjectName = cleanName.ToLower();

            // 如果是类似btnClose命名的对象，直接返回baseName，不附加父级信息
            if (lowerBaseName == lowerObjectName) {
                return baseName;
            }

            // 处理按钮、文本等常见组件的特殊情况
            if ((baseName.StartsWith("btn") && lowerObjectName == "close") ||
                (baseName.StartsWith("txt") && lowerObjectName == "text") ||
                (baseName.StartsWith("img") && lowerObjectName == "image")) {
                return baseName;
            }

            // 附加父节点信息
            string parentInfo = GetMeaningfulParentName(path);
            if (!string.IsNullOrEmpty(parentInfo)) {
                // 处理重复前缀的问题
                if ((baseName.StartsWith("btn") && parentInfo.StartsWith("Btn")) ||
                    (baseName.StartsWith("txt") && parentInfo.StartsWith("Txt")) ||
                    (baseName.StartsWith("img") && parentInfo.StartsWith("Img")) ||
                    (baseName.StartsWith("tog") && parentInfo.StartsWith("Tog")) ||
                    (baseName.StartsWith("slider") && parentInfo.StartsWith("Slider"))) {
                    if (parentInfo.Length > 3) {
                        parentInfo = parentInfo.Substring(3);
                    }
                }

                // 检查对象名和父级名是否相似，避免重复
                if (!lowerBaseName.Contains(parentInfo.ToLower()) &&
                    !parentInfo.ToLower().Contains(lowerBaseName)) {
                    baseName += "At" + parentInfo;
                }
            }

            return baseName;
        }

        /// <summary>
        /// 获取组件的基础变量名
        /// </summary>
        private string GetBaseNameForComponent(string cleanName, string componentType) {
            // 检查是否已有标准前缀
            if (cleanName.StartsWith("btn") ||
                cleanName.StartsWith("txt") ||
                cleanName.StartsWith("img") ||
                cleanName.StartsWith("tog") ||
                cleanName.StartsWith("slider")) {
                return cleanName;
            }

            // 根据组件类型添加前缀
            switch (componentType) {
                case "Button": return "btn" + (cleanName.Length > 0 ? char.ToUpper(cleanName[0]) + cleanName.Substring(1) : "Button");
                case "Text": return "txt" + (cleanName.Length > 0 ? char.ToUpper(cleanName[0]) + cleanName.Substring(1) : "Text");
                case "Image": return "img" + (cleanName.Length > 0 ? char.ToUpper(cleanName[0]) + cleanName.Substring(1) : "Image");
                case "Toggle": return "tog" + (cleanName.Length > 0 ? char.ToUpper(cleanName[0]) + cleanName.Substring(1) : "Toggle");
                case "Slider": return "slider" + (cleanName.Length > 0 ? char.ToUpper(cleanName[0]) + cleanName.Substring(1) : "Slider");
                default: return cleanName.Length > 0 ? char.ToLower(cleanName[0]) + cleanName.Substring(1) : "component";
            }
        }

        /// <summary>
        /// 获取有意义的父节点名称 - 修改为只获取最后一个有意义的父节点
        /// </summary>
        private string GetMeaningfulParentName(string path) {
            string[] pathParts = path.Split('/');
            if (pathParts.Length <= 1) return "";

            // 查找最后一个有意义的父节点
            for (int i = pathParts.Length - 2; i >= 0; i--) {
                string parentName = pathParts[i];

                // // 跳过一些通用的容器名称
                // if (parentName == "Root" || parentName == "Content" ||
                //     parentName == "Panel" || parentName == "Container") {
                //     continue;
                // }

                // 清理并返回有意义的父节点名称
                string cleanParent = CleanObjectName(parentName);
                if (!string.IsNullOrEmpty(cleanParent)) {
                    return char.ToUpper(cleanParent[0]) + cleanParent.Substring(1);
                }
            }

            return "";
        }
        /// <summary>
        /// 清理对象名称，移除特殊字符并处理空格
        /// </summary>
        private string CleanObjectName(string objectName) {
            // 移除括号及其内容
            int bracketIndex = objectName.IndexOf(" (");
            if (bracketIndex > 0) {
                objectName = objectName.Substring(0, bracketIndex);
            }

            // 移除其他特殊字符并保留字母、数字和空格
            string cleaned = new string(objectName.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());

            // 处理空格，转换为驼峰命名法
            string[] words = cleaned.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0) return "element";

            string result = words[0];
            for (int i = 1; i < words.Length; i++) {
                if (words[i].Length > 0) {
                    result += char.ToUpper(words[i][0]) + (words[i].Length > 1 ? words[i].Substring(1) : "");
                }
            }

            return result;
        }

        /// <summary>
        /// 清理路径，用于生成唯一键
        /// </summary>
        private string CleanPath(string path) {
            // 返回不带特殊字符的路径，用于字典键
            return new string(path.Where(c => char.IsLetterOrDigit(c) || c == '/').ToArray());
        }

        /// <summary>
        /// 检查变量名是否与Unity保留名称冲突，如果冲突则进行修改
        /// </summary>
        /// <param name="varName">原始变量名</param>
        /// <returns>处理后的变量名</returns>
        private string HandleReservedNames(string varName) {
            // Unity常见的保留名称列表
            HashSet<string> reservedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
        "gameObject", "transform", "name", "tag", "enabled",
        "activeInHierarchy", "activeSelf", "isActiveAndEnabled",
        "hideFlags", "useGUILayout", "runInEditMode"
    };

            // 检查是否与保留名称冲突
            if (reservedNames.Contains(varName)) {
                // 加上"Component"后缀以避免冲突
                return varName + "Component";
            }

            return varName;
        }

        // <summary>
        /// 存储UI组件信息的辅助类
        /// </summary>
        private class UIComponentInfo {
            /// <summary>
            /// 组件类型（如Button, Text等）
            /// </summary>
            public string ComponentType { get; set; }

            /// <summary>
            /// 组件在预制体中的路径
            /// </summary>
            public string Path { get; set; }

            /// <summary>
            /// 变量名
            /// </summary>
            public string VariableName { get; set; }
        }

        /// <summary>
        /// 临时存储组件信息用于排序的辅助类
        /// </summary>
        private class ComponentInfo {
            public Component Component { get; set; }
            public string Type { get; set; }
            public int Priority { get; set; }
            public string Path { get; set; }
        }
    }
}