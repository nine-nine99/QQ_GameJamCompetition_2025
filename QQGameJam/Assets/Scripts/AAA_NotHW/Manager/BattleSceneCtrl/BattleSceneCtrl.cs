using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public class BattleSceneCtrl : SingletonMonoBehavior<BattleSceneCtrl>
{
    [Header("战斗场景boss动画器控制器")]
    public AnimancerComponent boss_Animancer;

    public void PlayerClip(AnimationClip clip, System.Action onComplete = null)
    {
        if (boss_Animancer == null || clip == null)
        {
            Debug.LogError("缺少必要的动画组件或动画片段");
            return;
        }
        // Debug.LogWarning("XXXXXXXXX");

        // 播放clip动画
        var curState = boss_Animancer.Play(clip);

        // 正确的事件设置方式
        if (curState.Events(this, out var events))
        {
            events.OnEnd = () =>
            {
                // boss_Animancer.Play(Idle);
                onComplete?.Invoke();
            };
        }
    }
}
