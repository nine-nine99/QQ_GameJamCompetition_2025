using System;
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
            // NOTE:这里是音乐战斗结束的地方
            Send.SendMsg(SendType.MusicBattleEnd, false);
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
        return !audioSource.isPlaying && audioSource.time >= audioSource.clip.length && BGMController.Instance.isBegin == true;
    }
}
