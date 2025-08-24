using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HPBarControl : MonoBehaviour
{
    private HPModel hPModel => transform.GetComponent<HPModel>();
    public Transform topBar;

    [Header("HP条设置")]
    [SerializeField] private float animationDuration = 0.3f; // 动画持续时间
    [SerializeField] private Ease easeType = Ease.OutQuad; // 缓动类型
    [SerializeField] private bool useSmoothing = true; // 是否使用平滑过渡

    [Header("2D HP条特有设置")]
    [SerializeField] private bool is2DSprite = true; // 是否为2D Sprite
    [SerializeField] private Vector3 originalPosition; // 记录原始位置
    [SerializeField] private bool maintainAspectRatio = true; // 是否保持宽高比

    private Vector3 originalScale; // 记录原始缩放
    private float originalHeight; // 记录原始高度
    private float originalWidth; // 记录原始宽度
    private SpriteRenderer spriteRenderer; // 2D Sprite 渲染器
    private Tween currentTween; // 当前动画引用
    private Tween positionTween; // 位置动画引用

    private void Awake()
    {
        InitializeHPBar();
    }

    private void Start()
    {
        // 初始化HP条显示
        UpdateHPBar();
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        // 停止所有动画
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
        if (positionTween != null && positionTween.IsActive())
        {
            positionTween.Kill();
        }
    }

    /// <summary>
    /// 初始化HP条 - 针对2D对象优化
    /// </summary>
    private void InitializeHPBar()
    {
        if (topBar == null)
        {
            Debug.LogError("topBar 未设置！");
            return;
        }

        // 记录原始缩放和位置
        originalScale = topBar.localScale;
        originalPosition = topBar.localPosition;

        // 检查是否为2D Sprite
        spriteRenderer = topBar.GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            is2DSprite = true;
            // 获取Sprite的实际尺寸
            Bounds spriteBounds = spriteRenderer.bounds;
            originalHeight = spriteBounds.size.y;
            originalWidth = spriteBounds.size.x;
            
            Debug.Log($"2D Sprite HP条初始化: 高度={originalHeight}, 宽度={originalWidth}");
        }
        else
        {
            // 如果不是Sprite，尝试使用Renderer
            var renderer = topBar.GetComponent<Renderer>();
            if (renderer != null)
            {
                originalHeight = renderer.bounds.size.y;
                originalWidth = renderer.bounds.size.x;
            }
            else
            {
                // 备用：使用Transform的scale作为参考
                originalHeight = originalScale.y;
                originalWidth = originalScale.x;
            }
        }
    }

    /// <summary>
    /// 更新HP条显示
    /// </summary>
    public void UpdateHPBar()
    {
        if (hPModel == null || topBar == null)
        {
            Debug.LogWarning("HPModel 或 topBar 为空，无法更新HP条");
            return;
        }

        // 计算HP百分比
        float hpPercentage = Mathf.Clamp01((float)hPModel.HP / hPModel.MaxHP);

        // 根据是否使用平滑过渡来更新
        if (useSmoothing)
        {
            Update2DHPBarSmooth(hpPercentage);
        }
        else
        {
            Update2DHPBarInstant(hpPercentage);
        }
    }

    /// <summary>
    /// 2D HP条平滑更新
    /// </summary>
    private void Update2DHPBarSmooth(float targetPercentage)
    {
        // 停止之前的动画
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
        if (positionTween != null && positionTween.IsActive())
        {
            positionTween.Kill();
        }

        // 计算目标缩放
        Vector3 targetScale = CalculateTargetScale(targetPercentage);
        
        // 计算目标位置（使HP条从顶部开始减少）
        Vector3 targetPosition = CalculateTargetPosition(targetPercentage);
        Debug.Log("11");
        // 缩放动画
        currentTween = topBar.DOScale(targetScale, animationDuration)
            .SetEase(easeType)
            .OnComplete(() => currentTween = null);

        // 位置动画
        positionTween = topBar.DOLocalMove(targetPosition, animationDuration)
            .SetEase(easeType)
            .OnComplete(() => positionTween = null);
    }

    /// <summary>
    /// 2D HP条立即更新
    /// </summary>
    private void Update2DHPBarInstant(float targetPercentage)
    {
        // 直接设置缩放和位置
        topBar.localScale = CalculateTargetScale(targetPercentage);
        topBar.localPosition = CalculateTargetPosition(targetPercentage);
    }

    /// <summary>
    /// 计算目标缩放
    /// </summary>
    private Vector3 CalculateTargetScale(float hpPercentage)
    {
        Vector3 targetScale = originalScale;
        
        if (maintainAspectRatio)
        {
            // 保持宽高比，只缩放Y轴
            targetScale.y = originalScale.y * hpPercentage;
        }
        else
        {
            // 可以分别控制X和Y轴
            targetScale.y = originalScale.y * hpPercentage;
            // targetScale.x = originalScale.x; // 保持宽度不变
        }
        
        return targetScale;
    }

    /// <summary>
    /// 计算目标位置（使HP条从顶部开始减少）
    /// </summary>
    private Vector3 CalculateTargetPosition(float hpPercentage)
    {
        Vector3 targetPosition = originalPosition;
        
        // 计算Y轴偏移量，使HP条从顶部开始减少
        float heightDifference = originalHeight * (1f - hpPercentage);
        targetPosition.y = originalPosition.y - (heightDifference * 0.5f);
        
        return targetPosition;
    }

    /// <summary>
    /// 设置HP条颜色（针对2D Sprite优化）
    /// </summary>
    public void UpdateHPBarColor()
    {
        if (hPModel == null || topBar == null) return;

        float hpPercentage = (float)hPModel.HP / hPModel.MaxHP;
        Color targetColor = GetHPColor(hpPercentage);

        // 应用颜色
        if (spriteRenderer != null)
        {
            // 2D Sprite
            if (useSmoothing)
            {
                spriteRenderer.DOColor(targetColor, animationDuration * 0.5f);
            }
            else
            {
                spriteRenderer.color = targetColor;
            }
        }
        else
        {
            // 3D对象
            var renderer = topBar.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (useSmoothing)
                {
                    renderer.material.DOColor(targetColor, animationDuration * 0.5f);
                }
                else
                {
                    renderer.material.color = targetColor;
                }
            }
        }
    }

    /// <summary>
    /// 根据HP百分比获取颜色
    /// </summary>
    private Color GetHPColor(float hpPercentage)
    {
        if (hpPercentage > 0.6f)
        {
            return Color.green; // 高HP：绿色
        }
        else if (hpPercentage > 0.3f)
        {
            return Color.yellow; // 中HP：黄色
        }
        else
        {
            return Color.red; // 低HP：红色
        }
    }

    /// <summary>
    /// 2D专用的受伤特效
    /// </summary>
    public void PlayDamageEffect()
    {
        if (topBar == null) return;

        // 2D震动效果（在2D平面内震动）
        Vector3 shakeDirection = new Vector3(0.05f, 0.05f, 0f); // 只在X-Y平面震动
        topBar.DOShakePosition(0.2f, shakeDirection, 10, 90, false, true)
            .SetEase(Ease.OutBounce);

        // 2D闪烁效果
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.DOColor(Color.white, 0.1f)
                .OnComplete(() => spriteRenderer.DOColor(originalColor, 0.1f));
        }
        else
        {
            var renderer = topBar.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color originalColor = renderer.material.color;
                renderer.material.DOColor(Color.white, 0.1f)
                    .OnComplete(() => renderer.material.DOColor(originalColor, 0.1f));
            }
        }

        // 可选：添加缩放弹跳效果
        Vector3 originalScale = topBar.localScale;
        topBar.DOScale(originalScale * 1.1f, 0.1f)
            .OnComplete(() => topBar.DOScale(originalScale, 0.1f).SetEase(Ease.OutBack));
    }


    /// <summary>
    /// 重置HP条到初始状态
    /// </summary>
    public void ResetHPBar()
    {
        if (topBar == null) return;

        // 停止所有动画
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
        if (positionTween != null && positionTween.IsActive())
        {
            positionTween.Kill();
        }

        // 重置到初始状态
        topBar.localScale = originalScale;
        topBar.localPosition = originalPosition;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = GetHPColor(1f); // 满血颜色
        }
    }

    /// <summary>
    /// 公共方法：外部调用更新HP条
    /// </summary>
    public void RefreshHPBar()
    {
        UpdateHPBar();
        UpdateHPBarColor();
    }



    // 如果没有事件系统，可以在Update中检测HP变化
    private int lastHP = -1;

    private void Update()
    {
        if (hPModel != null && hPModel.HP != lastHP)
        {
            int previousHP = lastHP;
            lastHP = hPModel.HP;
            RefreshHPBar();

            // 如果HP减少，播放受伤特效
            if (previousHP != -1 && hPModel.HP < previousHP)
            {
                PlayDamageEffect();
            }
        }
    }

    #region 调试相关
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;

    private void OnGUI()
    {
        if (showDebugInfo && hPModel != null)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 150));
            GUILayout.Label($"当前HP: {hPModel.HP}");
            GUILayout.Label($"最大HP: {hPModel.MaxHP}");
            GUILayout.Label($"HP百分比: {(float)hPModel.HP / hPModel.MaxHP:P1}");
            GUILayout.Label($"2D Sprite: {is2DSprite}");
            GUILayout.Label($"原始尺寸: {originalWidth:F2} x {originalHeight:F2}");

            if (GUILayout.Button("测试扣血"))
            {
                if (hPModel.HP > 10)
                {
                    // 假设HPModel有public字段可以直接修改
                    // hPModel.HP -= 10;
                }
            }

            if (GUILayout.Button("重置HP条"))
            {
                ResetHPBar();
            }

            GUILayout.EndArea();
        }
    }

    // 在Scene视图中显示HP条的边界框（调试用）
    private void OnDrawGizmos()
    {
        if (topBar != null && showDebugInfo)
        {
            // 绘制HP条的边界
            Gizmos.color = Color.yellow;
            if (spriteRenderer != null)
            {
                Gizmos.DrawWireCube(topBar.position, spriteRenderer.bounds.size);
            }
            
            // 绘制原始位置
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + originalPosition, 0.1f);
        }
    }
    #endregion
}
