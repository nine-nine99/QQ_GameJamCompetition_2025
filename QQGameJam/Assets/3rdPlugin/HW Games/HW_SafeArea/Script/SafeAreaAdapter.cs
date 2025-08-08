using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SafeAreaPlugin
{
    /// <summary>
    /// 安全区域适配类型
    /// </summary>
    public enum SafeAreaType
    {
        FullScreen,     // 全屏适配
        TopOnly,        // 仅顶部适配
        BottomOnly,     // 仅底部适配
        SidesOnly,      // 仅左右适配
        Custom          // 自定义适配
    }

    /// <summary>
    /// UI分辨率适配模式
    /// </summary>
    public enum ResolutionAdaptMode
    {
        MatchWidthOrHeight,  // 匹配宽高
        MatchWidth,          // 匹配宽度
        MatchHeight,         // 匹配高度
        Expand,              // 扩展
        Shrink               // 收缩
    }

    /// <summary>
    /// 安全区域适配器
    /// </summary>
    [System.Serializable]
    public class SafeAreaAdapter : MonoBehaviour
    {
        [Header("安全区域设置")]
        public SafeAreaType safeAreaType = SafeAreaType.FullScreen;
        public bool adaptOnStart = true;
        public bool adaptOnOrientationChange = true;

        [Header("自定义边距 (仅在Custom模式下生效)")]
        public bool useTop = true;
        public bool useBottom = true;
        public bool useLeft = true;
        public bool useRight = true;

        [Header("最小边距限制")]
        public float minTopMargin = 0f;
        public float minBottomMargin = 0f;
        public float minLeftMargin = 0f;
        public float minRightMargin = 0f;

        [Header("调试信息")]
        public bool showDebugInfo = false;

        private RectTransform rectTransform;
        private Canvas rootCanvas;
        private Rect lastSafeArea;
        private ScreenOrientation lastOrientation;

        void Start()
        {
            Initialize();
            if (adaptOnStart)
            {
                ApplySafeArea();
            }
        }

        void Update()
        {
            if (adaptOnOrientationChange && HasOrientationChanged())
            {
                ApplySafeArea();
            }
        }

        void Initialize()
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Debug.LogError("SafeAreaAdapter需要RectTransform组件!");
                return;
            }

            // 找到根Canvas
            Canvas[] canvases = GetComponentsInParent<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay ||
                    canvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    rootCanvas = canvas;
                    break;
                }
            }

            lastOrientation = Screen.orientation;
        }

        bool HasOrientationChanged()
        {
            if (Screen.orientation != lastOrientation)
            {
                lastOrientation = Screen.orientation;
                return true;
            }
            return false;
        }

        public void ApplySafeArea()
        {
            if (rectTransform == null) return;

            Rect safeArea = GetSafeArea();

            // 如果安全区域没有变化，不需要重新适配
            if (safeArea == lastSafeArea) return;

            lastSafeArea = safeArea;

            // 获取屏幕尺寸
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);

            // 计算安全区域的边距
            Vector4 margins = CalculateMargins(safeArea, screenSize);

            // 应用边距
            ApplyMargins(margins, screenSize);

            if (showDebugInfo)
            {
                Debug.Log($"SafeArea Applied: {safeArea}, Margins: {margins}");
            }
        }

        Rect GetSafeArea()
        {
            return Screen.safeArea;
        }

        Vector4 CalculateMargins(Rect safeArea, Vector2 screenSize)
        {
            // 计算各边的边距 (左, 下, 右, 上)
            float left = safeArea.x;
            float bottom = safeArea.y;
            float right = screenSize.x - (safeArea.x + safeArea.width);
            float top = screenSize.y - (safeArea.y + safeArea.height);

            // 根据适配类型调整边距
            switch (safeAreaType)
            {
                case SafeAreaType.TopOnly:
                    left = bottom = right = 0;
                    break;
                case SafeAreaType.BottomOnly:
                    left = top = right = 0;
                    break;
                case SafeAreaType.SidesOnly:
                    top = bottom = 0;
                    break;
                case SafeAreaType.Custom:
                    if (!useLeft) left = 0;
                    if (!useBottom) bottom = 0;
                    if (!useRight) right = 0;
                    if (!useTop) top = 0;
                    break;
            }

            // 应用最小边距限制
            left = Mathf.Max(left, minLeftMargin);
            bottom = Mathf.Max(bottom, minBottomMargin);
            right = Mathf.Max(right, minRightMargin);
            top = Mathf.Max(top, minTopMargin);

            return new Vector4(left, bottom, right, top);
        }

        void ApplyMargins(Vector4 margins, Vector2 screenSize)
        {
            // 转换为相对坐标 (0-1)
            Vector4 relativeMargins = new Vector4(
                margins.x / screenSize.x,  // left
                margins.y / screenSize.y,  // bottom  
                margins.z / screenSize.x,  // right
                margins.w / screenSize.y   // top
            );

            // 设置锚点和偏移
            rectTransform.anchorMin = new Vector2(relativeMargins.x, relativeMargins.y);
            rectTransform.anchorMax = new Vector2(1f - relativeMargins.z, 1f - relativeMargins.w);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        // 编辑器下测试用
        [ContextMenu("应用安全区域")]
        void TestApplySafeArea()
        {
            Initialize();
            ApplySafeArea();
        }
    }

    /// <summary>
    /// UI分辨率适配器
    /// </summary>
    [System.Serializable]
    public class UIResolutionAdapter : MonoBehaviour
    {
        [Header("参考分辨率")]
        public Vector2 referenceResolution = new Vector2(1920, 1080);

        [Header("适配模式")]
        public ResolutionAdaptMode adaptMode = ResolutionAdaptMode.MatchWidthOrHeight;

        [Header("匹配权重 (仅MatchWidthOrHeight模式)")]
        [Range(0, 1)]
        public float matchWidthOrHeight = 0.5f;

        [Header("缩放限制")]
        public float minScale = 0.5f;
        public float maxScale = 2.0f;

        [Header("自动适配")]
        public bool autoAdaptOnResolutionChange = true;

        private CanvasScaler canvasScaler;
        private Vector2 lastScreenSize;

        void Start()
        {
            Initialize();
            ApplyResolutionAdaptation();
        }

        void Update()
        {
            if (autoAdaptOnResolutionChange && HasResolutionChanged())
            {
                ApplyResolutionAdaptation();
            }
        }

        void Initialize()
        {
            canvasScaler = GetComponent<CanvasScaler>();
            if (canvasScaler == null)
            {
                canvasScaler = gameObject.AddComponent<CanvasScaler>();
            }

            lastScreenSize = new Vector2(Screen.width, Screen.height);
        }

        bool HasResolutionChanged()
        {
            Vector2 currentSize = new Vector2(Screen.width, Screen.height);
            if (currentSize != lastScreenSize)
            {
                lastScreenSize = currentSize;
                return true;
            }
            return false;
        }

        public void ApplyResolutionAdaptation()
        {
            if (canvasScaler == null) return;

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = referenceResolution;

            switch (adaptMode)
            {
                case ResolutionAdaptMode.MatchWidthOrHeight:
                    canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
                    break;

                case ResolutionAdaptMode.MatchWidth:
                    canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    canvasScaler.matchWidthOrHeight = 0f;
                    break;

                case ResolutionAdaptMode.MatchHeight:
                    canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    canvasScaler.matchWidthOrHeight = 1f;
                    break;

                case ResolutionAdaptMode.Expand:
                    canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                    break;

                case ResolutionAdaptMode.Shrink:
                    canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
                    break;
            }

            // 应用缩放限制
            float currentScale = CalculateCurrentScale();
            if (currentScale < minScale || currentScale > maxScale)
            {
                float clampedScale = Mathf.Clamp(currentScale, minScale, maxScale);
                AdjustForScaleLimit(clampedScale);
            }
        }

        float CalculateCurrentScale()
        {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            float widthScale = screenSize.x / referenceResolution.x;
            float heightScale = screenSize.y / referenceResolution.y;

            switch (adaptMode)
            {
                case ResolutionAdaptMode.MatchWidth:
                    return widthScale;
                case ResolutionAdaptMode.MatchHeight:
                    return heightScale;
                case ResolutionAdaptMode.Expand:
                    return Mathf.Max(widthScale, heightScale);
                case ResolutionAdaptMode.Shrink:
                    return Mathf.Min(widthScale, heightScale);
                default:
                    return Mathf.Lerp(widthScale, heightScale, matchWidthOrHeight);
            }
        }

        void AdjustForScaleLimit(float targetScale)
        {
            // 当缩放超出限制时，调整参考分辨率
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            Vector2 adjustedReferenceResolution = screenSize / targetScale;
            canvasScaler.referenceResolution = adjustedReferenceResolution;
        }

        [ContextMenu("应用分辨率适配")]
        void TestApplyResolutionAdaptation()
        {
            Initialize();
            ApplyResolutionAdaptation();
        }
    }

    /// <summary>
    /// 综合适配管理器
    /// </summary>
    public class UniversalUIAdapter : MonoBehaviour
    {
        [Header("组件引用")]
        public SafeAreaAdapter safeAreaAdapter;
        public UIResolutionAdapter resolutionAdapter;

        [Header("全局设置")]
        public bool adaptOnAwake = true;
        public bool adaptOnApplicationFocus = true;

        void Awake()
        {
            if (adaptOnAwake)
            {
                InitializeAdapters();
            }
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (adaptOnApplicationFocus && hasFocus)
            {
                ApplyAllAdaptations();
            }
        }

        void InitializeAdapters()
        {
            if (safeAreaAdapter == null)
                safeAreaAdapter = GetComponent<SafeAreaAdapter>();
            if (resolutionAdapter == null)
                resolutionAdapter = GetComponent<UIResolutionAdapter>();
        }

        public void ApplyAllAdaptations()
        {
            if (resolutionAdapter != null)
                resolutionAdapter.ApplyResolutionAdaptation();

            if (safeAreaAdapter != null)
                safeAreaAdapter.ApplySafeArea();
        }

        [ContextMenu("应用所有适配")]
        void TestApplyAllAdaptations()
        {
            InitializeAdapters();
            ApplyAllAdaptations();
        }
    }

    /// <summary>
    /// 安全区域边距组件 - 用于特定UI元素的边距调整
    /// </summary>
    public class SafeAreaMargin : MonoBehaviour
    {
        [Header("边距设置")]
        public bool useTopMargin = true;
        public bool useBottomMargin = true;
        public bool useLeftMargin = true;
        public bool useRightMargin = true;

        [Header("额外偏移")]
        public Vector4 additionalOffset = Vector4.zero; // 左,下,右,上

        private RectTransform rectTransform;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            ApplyMargin();
        }

        public void ApplyMargin()
        {
            if (rectTransform == null) return;

            Rect safeArea = Screen.safeArea;
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);

            // 计算边距
            float leftMargin = useLeftMargin ? safeArea.x + additionalOffset.x : additionalOffset.x;
            float bottomMargin = useBottomMargin ? safeArea.y + additionalOffset.y : additionalOffset.y;
            float rightMargin = useRightMargin ? (screenSize.x - safeArea.xMax) + additionalOffset.z : additionalOffset.z;
            float topMargin = useTopMargin ? (screenSize.y - safeArea.yMax) + additionalOffset.w : additionalOffset.w;

            // 应用边距
            rectTransform.offsetMin = new Vector2(leftMargin, bottomMargin);
            rectTransform.offsetMax = new Vector2(-rightMargin, -topMargin);
        }
    }
}