using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingControl : MonoBehaviour, IPointerClickHandler
{
    [Header("位置设置")]
    [SerializeField] private Vector2 initialPosition = new Vector2(0, 850);
    [SerializeField] private Vector2 haftPosition = new Vector2(0, 660);
    [SerializeField] private Vector2 endPosition = new Vector2(0, 0);

    [Header("检测设置")]
    [SerializeField] private float detectionDistance = 200f;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutQuart;

    [Header("设置图片")]
    public Image settingImage;

    private bool isMouseInDetectionZone = false;
    private bool hasMovedToHalf = false;
    private bool hasMovedToEnd = false;
    private Tween moveTween;
    private RectTransform rectTransform;

    private void Awake()
    {
        // 获取 RectTransform 组件
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("SettingControl 需要挂载在 UI 对象上（需要 RectTransform 组件）");
        }
    }

    private void Start()
    {
        // 设置初始位置 - 使用 anchoredPosition
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = initialPosition;
        }
    }

    private void Update()
    {
        CheckMousePosition();
    }

    /// <summary>
    /// 实现 IPointerClickHandler 接口
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 检查点击位置是否在 settingImage 区域内
        if (settingImage != null && IsPointOverImage(eventData.position, settingImage))
        {
            OnSettingImageClicked();
        }
        if (settingImage != null && !IsPointOverImage(eventData.position, settingImage))
        {
            OnOutImageClicked();
        }
    }

    /// <summary>
    /// 检测鼠标位置是否在屏幕上边界指定距离内
    /// </summary>
    private void CheckMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        if (!IsMouseInScreen(mousePosition))
        {
            if (isMouseInDetectionZone)
            {
                OnMouseExitDetectionZone();
            }
            return;
        }

        float distanceFromTop = Screen.height - mousePosition.y;
        bool inZone = distanceFromTop <= detectionDistance;

        if (inZone && !isMouseInDetectionZone)
        {
            OnMouseEnterDetectionZone();
        }
        else if (!inZone && isMouseInDetectionZone)
        {
            OnMouseExitDetectionZone();
        }
    }

    private bool IsMouseInScreen(Vector3 mousePosition)
    {
        return mousePosition.x >= 0 &&
               mousePosition.x <= Screen.width &&
               mousePosition.y >= 0 &&
               mousePosition.y <= Screen.height;
    }

    /// <summary>
    /// 设置图片外被点击
    /// </summary>
    public void OnOutImageClicked()
    {
        Debug.Log("图片外区域被点击！");

        // 添加点击反馈效果
        AddClickFeedback();


        if (hasMovedToEnd)
        {
            MoveToInitialPosition();

            hasMovedToEnd = false;
            hasMovedToHalf = false;
        }
    }

    private void OnMouseEnterDetectionZone()
    {
        isMouseInDetectionZone = true;

        if (!hasMovedToHalf && !hasMovedToEnd)
        {
            MoveToHaftPosition();
        }
    }

    private void OnMouseExitDetectionZone()
    {
        isMouseInDetectionZone = false;

        if (hasMovedToHalf && !hasMovedToEnd)
        {
            MoveToInitialPosition();
        }
    }

    /// <summary>
    /// 移动到半位置 - 使用 DOAnchorPos
    /// </summary>
    private void MoveToHaftPosition()
    {
        if (rectTransform == null) return;

        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }

        hasMovedToHalf = true;

        // 使用 DOAnchorPos 进行 UI 动画
        moveTween = rectTransform.DOAnchorPos(haftPosition, animationDuration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                Debug.Log("移动到半位置完成");
                moveTween = null;
            });
    }

    /// <summary>
    /// 返回到初始位置 - 使用 DOAnchorPos
    /// </summary>
    private void MoveToInitialPosition()
    {
        if (rectTransform == null) return;

        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }

        hasMovedToHalf = false;

        // 使用 DOAnchorPos 进行 UI 动画
        moveTween = rectTransform.DOAnchorPos(initialPosition, animationDuration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                Debug.Log("返回初始位置完成");
                moveTween = null;
            });
    }

    /// <summary>
    /// 公共方法：移动到指定位置
    /// </summary>
    public void MoveToPosition(Vector2 targetPosition, float duration = -1f)
    {
        if (rectTransform == null) return;
        if (duration < 0) duration = animationDuration;

        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }

        moveTween = rectTransform.DOAnchorPos(targetPosition, duration)
            .SetEase(easeType)
            .OnComplete(() => moveTween = null);
    }

    /// <summary>
    /// 立即设置位置
    /// </summary>
    public void SetPosition(Vector2 position)
    {
        if (rectTransform == null) return;

        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }

        rectTransform.anchoredPosition = position;
        hasMovedToHalf = Vector2.Distance(position, haftPosition) < 0.1f;
    }

    private void OnDisable()
    {
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }
    }



    /// <summary>
    /// 改进的点击检测方法 - 修复摄像机问题
    /// </summary>
    private bool IsPointOverImage(Vector2 screenPoint, Image image)
    {
        if (image == null) return false;
        
        RectTransform imageRect = image.rectTransform;
        Vector2 localPoint;

        // 获取正确的摄像机
        Canvas canvas = image.canvas;
        Camera targetCamera = null;
        
        // 根据 Canvas 渲染模式选择正确的摄像机
        switch (canvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                targetCamera = null; // Overlay 模式不需要摄像机
                break;
            case RenderMode.ScreenSpaceCamera:
                targetCamera = canvas.worldCamera; // 使用 Canvas 指定的摄像机
                break;
            case RenderMode.WorldSpace:
                targetCamera = canvas.worldCamera ?? Camera.main; // 优先使用指定摄像机，否则用主摄像机
                break;
        }

        // 坐标转换
        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            imageRect,
            screenPoint,
            targetCamera,
            out localPoint
        );

        if (!success)
        {
            Debug.LogWarning($"坐标转换失败: Canvas模式={canvas.renderMode}, 摄像机={targetCamera?.name}");
            return false;
        }

        // 检查是否在矩形范围内
        bool isInside = imageRect.rect.Contains(localPoint);
        
        // 调试信息（可以在调试时启用）
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) // 只在点击时输出调试信息
        {
            Debug.Log($"[点击检测] 屏幕坐标: {screenPoint}");
            Debug.Log($"[点击检测] 本地坐标: {localPoint}");
            Debug.Log($"[点击检测] 图片矩形: {imageRect.rect}");
            Debug.Log($"[点击检测] Canvas模式: {canvas.renderMode}");
            Debug.Log($"[点击检测] 使用摄像机: {targetCamera?.name ?? "null"}");
            Debug.Log($"[点击检测] 检测结果: {isInside}");
        }
        #endif
        
        return isInside;
    }

    /// <summary>
    /// 设置图片被点击时的处理
    /// </summary>
    private void OnSettingImageClicked()
    {
        Debug.Log("设置图片被点击！");

        // 添加点击反馈效果
        AddClickFeedback();

        // 根据当前状态执行不同操作
        if (hasMovedToHalf)
        {
            MoveToEndPosition();
        }
        else
        {
            if (!hasMovedToEnd)
            {
                MoveToHaftPosition();
            }
        }
    }

    /// <summary>
    /// 添加点击反馈效果
    /// </summary>
    private void AddClickFeedback()
    {
        if (hasMovedToEnd)  return; // 如果已经在完全显示位置，不再添加反馈
        if (settingImage != null)
        {
            // 缩放反馈效果
            settingImage.transform.DOScale(1.1f, 0.1f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    settingImage.transform.DOScale(1f, 0.1f).SetEase(Ease.InBack);
                });

            // 或者颜色反馈效果
            Color originalColor = settingImage.color;
            settingImage.DOColor(Color.gray, 0.1f)
                .OnComplete(() =>
                {
                    settingImage.DOColor(originalColor, 0.1f);
                });
        }
    }

    /// <summary>
    /// 移动到完全显示位置
    /// </summary>
    private void MoveToEndPosition()
    {
        if (rectTransform == null) return;

        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }

        hasMovedToEnd = true;

        moveTween = rectTransform.DOAnchorPos(endPosition, animationDuration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                Debug.Log("移动到完全显示位置完成");
                moveTween = null;
            });
    }

    // #region 调试相关
    // private void OnGUI()
    // {
    //     if (Application.isEditor && rectTransform != null)
    //     {
    //         GUILayout.BeginArea(new Rect(10, 10, 400, 150));
    //         GUILayout.Label($"鼠标位置: {Input.mousePosition}");
    //         GUILayout.Label($"距离顶部: {Screen.height - Input.mousePosition.y:F1}");
    //         GUILayout.Label($"在检测区域: {isMouseInDetectionZone}");
    //         GUILayout.Label($"在屏幕内: {IsMouseInScreen(Input.mousePosition)}");
    //         GUILayout.Label($"已移动到半位置: {hasMovedToHalf}");
    //         GUILayout.Label($"当前 anchoredPosition: {rectTransform.anchoredPosition}");
    //         GUILayout.EndArea();
    //     }
    // }
    // #endregion
}
