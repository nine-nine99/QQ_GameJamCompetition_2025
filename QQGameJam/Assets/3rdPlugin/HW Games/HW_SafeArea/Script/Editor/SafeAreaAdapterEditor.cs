#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using SafeAreaPlugin;

namespace SafeAreaPlugin.Editor {
    /// <summary>
    /// 安全区域适配器编辑器
    /// </summary>
    [CustomEditor(typeof(SafeAreaAdapter))]
    public class SafeAreaAdapterEditor : UnityEditor.Editor {
        private SafeAreaAdapter adapter;

        void OnEnable() {
            adapter = target as SafeAreaAdapter;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("安全区域适配器", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 基本设置
            EditorGUILayout.PropertyField(serializedObject.FindProperty("safeAreaType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("adaptOnStart"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("adaptOnOrientationChange"));

            EditorGUILayout.Space();

            // 自定义模式设置
            if (adapter.safeAreaType == SafeAreaType.Custom) {
                EditorGUILayout.LabelField("自定义适配设置", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useTop"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useBottom"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useLeft"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useRight"));
                EditorGUILayout.Space();
            }

            // 最小边距设置
            EditorGUILayout.LabelField("最小边距限制", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minTopMargin"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minBottomMargin"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minLeftMargin"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minRightMargin"));

            EditorGUILayout.Space();

            // 调试设置
            EditorGUILayout.PropertyField(serializedObject.FindProperty("showDebugInfo"));

            EditorGUILayout.Space();

            // 实时信息显示
            if (Application.isPlaying) {
                EditorGUILayout.LabelField("实时信息", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"屏幕尺寸: {Screen.width} x {Screen.height}");
                EditorGUILayout.LabelField($"安全区域: {Screen.safeArea}");
                EditorGUILayout.LabelField($"当前方向: {Screen.orientation}");

                EditorGUILayout.Space();

                if (GUILayout.Button("手动应用安全区域")) {
                    adapter.ApplySafeArea();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    /// <summary>
    /// UI分辨率适配器编辑器
    /// </summary>
    [CustomEditor(typeof(UIResolutionAdapter))]
    public class UIResolutionAdapterEditor : UnityEditor.Editor {
        private UIResolutionAdapter adapter;

        void OnEnable() {
            adapter = target as UIResolutionAdapter;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("UI分辨率适配器", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 参考分辨率
            EditorGUILayout.PropertyField(serializedObject.FindProperty("referenceResolution"));

            // 适配模式
            EditorGUILayout.PropertyField(serializedObject.FindProperty("adaptMode"));

            // 匹配权重（仅在MatchWidthOrHeight模式下显示）
            if (adapter.adaptMode == ResolutionAdaptMode.MatchWidthOrHeight) {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("matchWidthOrHeight"));

                // 显示权重说明
                EditorGUILayout.HelpBox(
                    "匹配权重说明:\n" +
                    "0 = 完全匹配宽度\n" +
                    "1 = 完全匹配高度\n" +
                    "0.5 = 平衡匹配",
                    MessageType.Info);
            }

            EditorGUILayout.Space();

            // 缩放限制
            EditorGUILayout.LabelField("缩放限制", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minScale"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxScale"));

            EditorGUILayout.Space();

            // 自动适配
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoAdaptOnResolutionChange"));

            EditorGUILayout.Space();

            // 实时信息
            if (Application.isPlaying) {
                EditorGUILayout.LabelField("实时信息", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"当前分辨率: {Screen.width} x {Screen.height}");
                EditorGUILayout.LabelField($"参考分辨率: {adapter.referenceResolution}");

                // 计算当前缩放比例
                float widthScale = (float)Screen.width / adapter.referenceResolution.x;
                float heightScale = (float)Screen.height / adapter.referenceResolution.y;
                EditorGUILayout.LabelField($"宽度缩放: {widthScale:F2}");
                EditorGUILayout.LabelField($"高度缩放: {heightScale:F2}");

                EditorGUILayout.Space();

                if (GUILayout.Button("手动应用分辨率适配")) {
                    adapter.ApplyResolutionAdaptation();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    /// <summary>
    /// 安全区域适配菜单
    /// </summary>
    public static class SafeAreaAdaptMenu {
        [MenuItem("GameObject/UI/Safe Area Adapter", false, 10)]
        public static void CreateSafeAreaAdapter() {
            GameObject go = new GameObject("SafeAreaAdapter");
            go.AddComponent<RectTransform>();
            go.AddComponent<SafeAreaAdapter>();

            // 设置为当前选中Canvas的子对象
            if (Selection.activeGameObject != null) {
                Canvas canvas = Selection.activeGameObject.GetComponentInParent<Canvas>();
                if (canvas != null) {
                    go.transform.SetParent(canvas.transform, false);
                }
            }

            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/UI/Resolution Adapter", false, 11)]
        public static void CreateResolutionAdapter() {
            GameObject go = Selection.activeGameObject;
            if (go == null) {
                Debug.LogWarning("请先选择一个Canvas对象");
                return;
            }

            Canvas canvas = go.GetComponent<Canvas>();
            if (canvas == null) {
                Debug.LogWarning("选择的对象不是Canvas");
                return;
            }

            if (go.GetComponent<UIResolutionAdapter>() == null) {
                go.AddComponent<UIResolutionAdapter>();
                Debug.Log("已为Canvas添加UI分辨率适配器");
            }
            else {
                Debug.Log("Canvas已经有UI分辨率适配器了");
            }
        }

        [MenuItem("GameObject/UI/Universal UI Adapter", false, 12)]
        public static void CreateUniversalAdapter() {
            GameObject go = Selection.activeGameObject;
            if (go == null) {
                Debug.LogWarning("请先选择一个Canvas对象");
                return;
            }

            Canvas canvas = go.GetComponent<Canvas>();
            if (canvas == null) {
                Debug.LogWarning("选择的对象不是Canvas");
                return;
            }

            // 添加综合适配器
            UniversalUIAdapter universalAdapter = go.GetComponent<UniversalUIAdapter>();
            if (universalAdapter == null) {
                universalAdapter = go.AddComponent<UniversalUIAdapter>();
            }

            // 添加分辨率适配器
            UIResolutionAdapter resolutionAdapter = go.GetComponent<UIResolutionAdapter>();
            if (resolutionAdapter == null) {
                resolutionAdapter = go.AddComponent<UIResolutionAdapter>();
            }
            universalAdapter.resolutionAdapter = resolutionAdapter;

            // 创建安全区域适配对象
            GameObject safeAreaGO = new GameObject("SafeAreaPanel");
            safeAreaGO.transform.SetParent(canvas.transform, false);

            RectTransform safeAreaRect = safeAreaGO.AddComponent<RectTransform>();
            safeAreaRect.anchorMin = Vector2.zero;
            safeAreaRect.anchorMax = Vector2.one;
            safeAreaRect.offsetMin = Vector2.zero;
            safeAreaRect.offsetMax = Vector2.zero;

            SafeAreaAdapter safeAreaAdapter = safeAreaGO.AddComponent<SafeAreaAdapter>();
            universalAdapter.safeAreaAdapter = safeAreaAdapter;

            Debug.Log("已创建通用UI适配器设置");
            Selection.activeGameObject = safeAreaGO;
        }
    }

    /// <summary>
    /// 常用分辨率预设窗口
    /// </summary>
    public class ResolutionPresetWindow : EditorWindow {
        private UIResolutionAdapter targetAdapter;

        [MenuItem("Window/Safe Area Plugin/Resolution Presets")]
        public static void ShowWindow() {
            GetWindow<ResolutionPresetWindow>("分辨率预设");
        }

        void OnGUI() {
            EditorGUILayout.LabelField("分辨率预设", EditorStyles.boldLabel);

            targetAdapter = EditorGUILayout.ObjectField("目标适配器", targetAdapter, typeof(UIResolutionAdapter), true) as UIResolutionAdapter;

            if (targetAdapter == null) {
                EditorGUILayout.HelpBox("请选择一个UI分辨率适配器", MessageType.Warning);
                return;
            }

            EditorGUILayout.Space();

            // 常用分辨率预设
            if (GUILayout.Button("手机竖屏 (1080x1920)"))
                SetResolution(new Vector2(1080, 1920));
            if (GUILayout.Button("手机横屏 (1920x1080)"))
                SetResolution(new Vector2(1920, 1080));
            if (GUILayout.Button("平板 (1536x2048)"))
                SetResolution(new Vector2(1536, 2048));
            if (GUILayout.Button("PC标准 (1920x1080)"))
                SetResolution(new Vector2(1920, 1080));
            if (GUILayout.Button("4K (3840x2160)"))
                SetResolution(new Vector2(3840, 2160));

            EditorGUILayout.Space();

            // iPhone刘海屏预设
            EditorGUILayout.LabelField("iPhone刘海屏预设", EditorStyles.boldLabel);
            if (GUILayout.Button("iPhone X/XS (1125x2436)"))
                SetResolution(new Vector2(1125, 2436));
            if (GUILayout.Button("iPhone XR/11 (828x1792)"))
                SetResolution(new Vector2(828, 1792));
            if (GUILayout.Button("iPhone 12/13 (1170x2532)"))
                SetResolution(new Vector2(1170, 2532));
            if (GUILayout.Button("iPhone 12/13 Pro Max (1284x2778)"))
                SetResolution(new Vector2(1284, 2778));
        }

        void SetResolution(Vector2 resolution) {
            if (targetAdapter != null) {
                Undo.RecordObject(targetAdapter, "Set Resolution Preset");
                targetAdapter.referenceResolution = resolution;
                targetAdapter.ApplyResolutionAdaptation();
                EditorUtility.SetDirty(targetAdapter);
            }
        }
    }
}
#endif