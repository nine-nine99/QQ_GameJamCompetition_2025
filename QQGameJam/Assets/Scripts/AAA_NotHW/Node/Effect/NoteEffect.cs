using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

public class NoteEffect : MonoBehaviour
{
    [Header("当前的动画器控制器")]
    public AnimancerComponent animancer;
    [Header("Idle动画")]
    public AnimationClip Idle;
    [Header("Effect动画")]
    public AnimationClip Effect;

    /// <summary>
    /// 播放特效动画，完成后自动切换到Idle动画
    /// </summary>
    /// <param name="onComplete">播放完动画后的效果</param>
    public void StartEffect(System.Action onComplete = null)
    {
        if (animancer == null || Effect == null || Idle == null)
        {
            Debug.LogError("缺少必要的动画组件或动画片段");
            return;
        }
        // Debug.LogWarning("XXXXXXXXX");
        // 播放Effect动画
        var effectState = animancer.Play(Effect);

        // 正确的事件设置方式
        if (effectState.Events(this, out var events))
        {
            events.OnEnd = () =>
            {
                // Effect动画结束后播放Idle动画
                animancer.Play(Idle);
                onComplete?.Invoke();
            };
        }
    }

    /// <summary>
    /// 播放特效动画的另一种实现方式（使用协程）
    /// </summary>
    public void PlayEffectAlternative()
    {
        if (animancer == null || Effect == null || Idle == null)
        {
            Debug.LogError("缺少必要的动画组件或动画片段");
            return;
        }

        StartCoroutine(PlayEffectCoroutine());
    }

    private IEnumerator PlayEffectCoroutine()
    {
        // 播放Effect动画
        var effectState = animancer.Play(Effect);

        // 等待动画播放完成
        yield return effectState;

        // Effect动画结束后播放Idle动画
        animancer.Play(Idle);
    }

    /// <summary>
    /// 直接播放Idle动画
    /// </summary>
    public void PlayIdle()
    {
        if (animancer == null || Idle == null)
        {
            Debug.LogError("缺少必要的动画组件或Idle动画");
            return;
        }

        animancer.Play(Idle);
    }

    /// <summary>
    /// 直接播放Effect动画
    /// </summary>
    public void PlayEffect()
    {
        if (animancer == null || Effect == null)
        {
            Debug.LogError("缺少必要的动画组件或Effect动画");
            return;
        }

        animancer.Play(Effect);
    }

    /// <summary>
    /// 停止所有动画
    /// </summary>
    public void StopAllAnimations()
    {
        if (animancer != null)
        {
            animancer.Stop();
        }
    }
}
