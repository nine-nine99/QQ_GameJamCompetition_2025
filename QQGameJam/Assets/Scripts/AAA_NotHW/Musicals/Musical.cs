using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // 添加 DOTween 命名空间

public class Musical : MonoBehaviour
{
    public GameObject cue;
    [Header("将要进入的关卡的key")]
    public int key;

    [Header("摇晃设置")]
    [SerializeField] private float shakeIntensity = 0.1f;    // 摇晃强度
    [SerializeField] private float shakeDuration = 0.5f;     // 单次摇晃持续时间
    [SerializeField] private int shakeVibrato = 10;          // 摇晃次数
    [SerializeField] private float shakeRandomness = 90f;    // 摇晃随机性

    private bool isIn = false;
    private Vector3 originalPosition; // 记录原始位置
    private Tween shakeTween; // 摇晃动画引用

    private void OnEnable()
    {
        Init();
    }
    private void OnDisable()
    {
        Clear();
    }

    void Start()
    {
        // 记录 cue 的原始位置
        if (cue != null)
        {
            originalPosition = cue.transform.position;
            cue.SetActive(false); // 初始时隐藏 cue
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isIn)
        {
            isIn = true;
            StartCueShaking(); // 开始摇晃
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isIn = false;
            StopCueShaking(); // 停止摇晃
            cue.SetActive(false);
        }
    }

    private void Update()
    {
        if (isIn)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("进入音乐关卡: " + key);
                // 进入关卡时停止摇晃
                StopCueShaking();
                // 发送消息到游戏管理器
                Send.SendMsg(SendType.Into_InsideWorld, key);
            }
        }
        else
        {

        }
    }

    private void Init()
    {
        StopCueShaking(); // 清理时停止摇晃
        // cue.SetActive(false); // 初始化时隐藏 cue
        isIn = false; // 重置状态F
    }
    private void Clear()
    {
        StopCueShaking(); // 清理时停止摇晃
        cue.SetActive(false); // 确保 cue 被隐藏
        isIn = false; // 重置状态
    }


    /// <summary>
    /// 开始 cue 摇晃动画
    /// </summary>
    private void StartCueShaking()
    {
        if (cue == null) return;
        Debug.Log("开始摇晃动画");
        cue.SetActive(true);

        // 停止之前的摇晃动画
        StopCueShaking();

        // 2D 模式的摇晃 - 只在 X 和 Y 轴摇晃
        Vector3 shake2D = new Vector3(shakeIntensity, shakeIntensity, 0f); // Z轴设为0
        cue.transform.position = originalPosition; // 重置位置到原始位置
        shakeTween = cue.transform
            .DOShakePosition(shakeDuration, shake2D, shakeVibrato, shakeRandomness, false, true) // snapping设为true适合2D
            .SetLoops(-1, LoopType.Restart) // 无限循环
            .SetEase(Ease.InOutSine);
    }

    /// <summary>
    /// 停止 cue 摇晃动画
    /// </summary>
    private void StopCueShaking()
    {
        if (shakeTween != null)
        {
            shakeTween.Kill(); // 停止动画
            shakeTween = null;
        }

        // 平滑回到原始位置
        if (cue != null)
        {
            cue.transform.DOMove(originalPosition, 0.3f).SetEase(Ease.OutBack);
        }
    }

}
