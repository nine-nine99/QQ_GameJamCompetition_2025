using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMListener : SingletonMonoBehavior<BGMListener>
{
    private AudioSource audioSource => transform.GetComponent<AudioSource>();
    void Update()
    {
        if (IsFinished())
        {
            Debug.Log("is Finish");
            Send.SendMsg(SendType.MusicBattleEnd);
            // DialogueMgr.Instance.OpenDialogue(1);
            // GameStateMgr.Instance.SwitchState(GameState.Main);
        }
    }
    // 获取当前播放时间（秒）
    public float GetCurrentTime()
    {
        return audioSource.time;
    }

    // 获取音频总长度（秒）
    public float GetTotalLength()
    {
        return audioSource.clip != null ? audioSource.clip.length : 0f;
    }
    // 检查是否播放完毕
    public bool IsFinished()
    {
        return !audioSource.isPlaying && audioSource.time >= audioSource.clip.length;
    }
}
