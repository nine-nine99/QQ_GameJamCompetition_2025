using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongPressNote : MonoBehaviour
{
    [Header("长音符组件")]
    public Transform headNote; // 音符头部
    public Transform tailNote; // 音符尾部
    public Transform noteBody; // 音符连接线/身体
    public LineRenderer bodyLineRenderer; // 身体线条渲染器

    [Header("长音符状态")]
    public bool isPressed = false; // 是否正在被按住
    public bool isCompleted = false; // 是否完成
    public bool hasStarted = false; // 是否已开始判定

    [Header("LineRenderer设置")]
    public Material normalMaterial; // 正常状态材质
    public Material holdingMaterial; // 按住状态材质
    public float lineWidth = 0.1f; // 线条宽度

    private SyllableDetail syllableDetail;
    private float currentTime;
    private float moveSpeed = 5f; // 移动速度
    private float judgeLineY = 0f; // 判定线Y坐标
    private Vector3 startPosition;
    private Vector3 endPosition;

    // 长音符判定相关
    private bool headHit = false; // 头部是否击中
    private bool bodyHolding = false; // 是否正在按住身体
    private float holdStartTime; // 开始按住的时间
    private float totalHoldTime; // 总按住时间

    // LineRenderer 颜色控制
    private Color normalColor = Color.white;
    private Color holdingColor = Color.green;
    private Color failColor = Color.red;

    void Update()
    {
        if (isCompleted) return;

        currentTime += Time.deltaTime;

        // 移动音符
        MoveNote();

        // 检测输入
        CheckInput();

        // 检查是否完成或失败
        CheckCompletion();

        // 更新LineRenderer
        UpdateLineRenderer();
    }

    public void Initialize(SyllableDetail detail, float judgeLine, Transform startTransform, Transform endTransform)
    {
        syllableDetail = detail;
        // moveSpeed = speed;
        judgeLineY = judgeLine;

        // 如果传入了起始/结束 Transform，优先使用它们的 worldPosition
        if (startTransform != null && endTransform != null)
        {
            startPosition = startTransform.position;
            endPosition = endTransform.position;
            float distance = endPosition.y - startPosition.y;
            moveSpeed = distance / (syllableDetail.endTime_1 - syllableDetail.showTime_1);
        }
        else
        {
            // 回退到原有基于时间计算的位置（保持兼容）
            float distance = moveSpeed * (syllableDetail.endTime_1 - syllableDetail.showTime_1);
            startPosition = new Vector3(syllableDetail.positionIndex * 2f, judgeLineY + distance, 0);
            endPosition = new Vector3(syllableDetail.positionIndex * 2f, judgeLineY, 0);
        }

        transform.position = startPosition;

        // 设置长音符长度和LineRenderer
        SetupLongNoteLength();
        InitializeLineRenderer();
    }

    void InitializeLineRenderer()
    {
        if (bodyLineRenderer == null) return;

        // 设置LineRenderer基本属性
        bodyLineRenderer.positionCount = 2;
        bodyLineRenderer.startWidth = lineWidth;
        bodyLineRenderer.endWidth = lineWidth;
        bodyLineRenderer.material = normalMaterial != null ? normalMaterial : bodyLineRenderer.material;
        SetLineRendererColor(normalColor);
        bodyLineRenderer.sortingOrder = 1; // 确保在其他sprite之上
        bodyLineRenderer.useWorldSpace = false; // 使用本地坐标
    }

    // 辅助方法：设置LineRenderer颜色
    void SetLineRendererColor(Color color)
    {
        if (bodyLineRenderer == null) return;

        bodyLineRenderer.startColor = color;
        bodyLineRenderer.endColor = color;
    }

    void SetupLongNoteLength()
    {
        if (syllableDetail.syllableType != SyllableType.Hold) return;

        // 计算长音符的长度
        float holdDuration = syllableDetail.GetHoldDuration();
        float noteLength = holdDuration * moveSpeed;

        // 设置尾部位置
        if (tailNote != null)
        {
            tailNote.localPosition = new Vector3(0, -noteLength, 0);
        }

        // 更新LineRenderer的点位置
        UpdateLineRenderer();
    }

    void UpdateLineRenderer()
    {
        if (bodyLineRenderer == null || headNote == null || tailNote == null) return;

        // 设置LineRenderer的起点和终点
        Vector3 startPoint = headNote.localPosition;
        Vector3 endPoint = tailNote.localPosition;

        bodyLineRenderer.SetPosition(0, startPoint);
        bodyLineRenderer.SetPosition(1, endPoint);
    }



    void MoveNote()
    {
        // 计算当前应该在的位置
        float progress = (currentTime - syllableDetail.showTime_1) / (syllableDetail.endTime_1 - syllableDetail.showTime_1);
        progress = Mathf.Clamp01(progress);

        Vector3 currentPos = Vector3.Lerp(startPosition, endPosition, progress);
        transform.position = currentPos;
    }

    void CheckInput()
    {
        // 检查是否在正确的轨道上按下
        KeyCode curKeyCode = GetKeyForPosition(syllableDetail.positionIndex);
        bool isInputPressed = Input.GetKey(curKeyCode);
        bool wasInputPressed = Input.GetKeyDown(curKeyCode);
        bool inputReleased = Input.GetKeyUp(curKeyCode);

        // 头部判定
        if (!headHit && wasInputPressed)
        {
            float headDistance = Vector3.Distance(headNote.position, new Vector3(headNote.position.x, judgeLineY, headNote.position.z));
            if (headDistance < 0.5f) // 判定范围
            {
                headHit = true;
                hasStarted = true;
                holdStartTime = currentTime;
                OnHeadHit();
            }
        }

        // 身体按住判定
        if (headHit && !isCompleted)
        {
            if (isInputPressed)
            {
                if (!bodyHolding)
                {
                    bodyHolding = true;
                    OnBodyHoldStart();
                }

                // 更新按住时间
                totalHoldTime += Time.deltaTime;
                UpdateHoldEffect();
            }
            else
            {
                if (bodyHolding)
                {
                    bodyHolding = false;
                    OnBodyHoldEnd();
                }
            }
        }

        // 检查提前释放
        if (headHit && inputReleased && !isCompleted)
        {
            CheckEarlyRelease();
        }
    }

    void CheckCompletion()
    {
        if (isCompleted) return;

        // 检查尾部是否通过判定线
        if (tailNote != null)
        {
            if (tailNote.position.y <= judgeLineY)
            {
                CompleteNote();
            }
        }

        // 检查是否超时失败
        if (currentTime > syllableDetail.endTime_2 + 1f) // 给1秒容错时间
        {
            FailNote();
        }
    }

    void CheckEarlyRelease()
    {
        float expectedHoldTime = syllableDetail.GetHoldDuration();
        if (totalHoldTime < expectedHoldTime * 0.8f) // 至少要按住80%的时间
        {
            FailNote();
        }
    }

    KeyCode GetKeyForPosition(int position)
    {
        // 根据位置返回对应的按键
        return SetMenuControl.Instance.GetcurrentKeys(position);
    }

    void OnHeadHit()
    {
        Debug.Log("长音符头部击中！");
        // 播放击中特效
        if (headNote != null)
        {
            // 添加击中特效
            PlayHitEffect(headNote.position);
        }
    }

    void OnBodyHoldStart()
    {
        Debug.Log("开始按住长音符身体");
        // 改变LineRenderer颜色和材质
        if (bodyLineRenderer != null)
        {
            SetLineRendererColor(holdingColor);
            if (holdingMaterial != null)
            {
                bodyLineRenderer.material = holdingMaterial;
            }
        }
    }

    void OnBodyHoldEnd()
    {
        Debug.Log("停止按住长音符身体");
        // 恢复LineRenderer颜色和材质
        if (bodyLineRenderer != null)
        {
            SetLineRendererColor(normalColor);
            if (normalMaterial != null)
            {
                bodyLineRenderer.material = normalMaterial;
            }
        }
    }

    void UpdateHoldEffect()
    {
        // 更新按住时的视觉效果
        if (bodyLineRenderer != null)
        {
            // 创建呼吸灯效果
            float alpha = 0.7f + 0.3f * Mathf.Sin(Time.time * 10f);
            Color color = holdingColor;
            color.a = alpha;
            SetLineRendererColor(color);

            // 可选：动态改变线条宽度
            float widthMultiplier = 1f + 0.2f * Mathf.Sin(Time.time * 8f);
            bodyLineRenderer.startWidth = lineWidth * widthMultiplier;
            bodyLineRenderer.endWidth = lineWidth * widthMultiplier;
        }
    }

    void CompleteNote()
    {
        isCompleted = true;
        Debug.Log("长音符完成！");

        // 计算得分
        CalculateScore();

        // 播放完成特效
        PlayCompleteEffect();

        // 销毁音符
        // Destroy(gameObject, 0.5f);
        ObjectPool.Instance.Recycle(gameObject);
    }

    void FailNote()
    {
        isCompleted = true;
        Debug.Log("长音符失败！");

        // 设置失败颜色
        if (bodyLineRenderer != null)
        {
            SetLineRendererColor(failColor);
        }

        // 播放失败特效
        PlayFailEffect();

        // 销毁音符
        // Destroy(gameObject, 0.5f);
        ObjectPool.Instance.Recycle(gameObject);
    }

    void CalculateScore()
    {
        float expectedHoldTime = syllableDetail.GetHoldDuration();
        float accuracy = Mathf.Clamp01(totalHoldTime / expectedHoldTime);

        int score = 0;
        string rating = "";

        if (accuracy >= 0.95f)
        {
            score = 1000;
            rating = "Perfect";
        }
        else if (accuracy >= 0.8f)
        {
            score = 800;
            rating = "Great";
        }
        else if (accuracy >= 0.6f)
        {
            score = 600;
            rating = "Good";
        }
        else
        {
            score = 300;
            rating = "Bad";
        }

        Debug.Log($"长音符评级: {rating}, 得分: {score}, 准确率: {accuracy:P}");

        // 这里可以调用分数管理器来记录分数
        // ScoreManager.Instance.AddScore(score);
    }

    void PlayHitEffect(Vector3 position)
    {
        // 播放击中粒子特效
        // ParticleSystem hitEffect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
    }

    void PlayCompleteEffect()
    {
        // 播放完成特效
        Debug.Log("播放长音符完成特效");

        // 可选：LineRenderer完成特效
        if (bodyLineRenderer != null)
        {
            StartCoroutine(CompleteEffectCoroutine());
        }
    }

    IEnumerator CompleteEffectCoroutine()
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Color startColor = bodyLineRenderer.startColor;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // 渐变到亮色
            Color currentColor = Color.Lerp(startColor, Color.cyan, progress);
            SetLineRendererColor(currentColor);

            yield return null;
        }
    }

    void PlayFailEffect()
    {
        // 播放失败特效
        Debug.Log("播放长音符失败特效");

        // 可选：LineRenderer失败特效
        if (bodyLineRenderer != null)
        {
            StartCoroutine(FailEffectCoroutine());
        }
    }

    IEnumerator FailEffectCoroutine()
    {
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 闪烁效果
            Color flashColor = elapsed % 0.1f < 0.05f ? failColor : Color.clear;
            SetLineRendererColor(flashColor);

            yield return null;
        }
    }

    // 公共方法：强制完成音符（用于调试）
    public void ForceComplete()
    {
        CompleteNote();
    }

    // 公共方法：获取当前状态
    public bool IsHeadHit() => headHit;
    public bool IsBodyHolding() => bodyHolding;
    public float GetHoldProgress() => totalHoldTime / syllableDetail.GetHoldDuration();

    // 公共方法：设置LineRenderer属性
    public void SetLineWidth(float width)
    {
        lineWidth = width;
        if (bodyLineRenderer != null)
        {
            bodyLineRenderer.startWidth = width;
            bodyLineRenderer.endWidth = width;
        }
    }

    public void SetLineMaterial(Material mat)
    {
        if (bodyLineRenderer != null && mat != null)
        {
            bodyLineRenderer.material = mat;
        }
    }
}
