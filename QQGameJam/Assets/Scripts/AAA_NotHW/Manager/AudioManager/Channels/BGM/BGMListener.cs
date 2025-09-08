using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 当前曲子的数据
public class BGMListener : SingletonMonoBehavior<BGMListener>
{
    private AudioSource audioSource => transform.GetComponent<AudioSource>();

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
        return !audioSource.isPlaying && audioSource.time >= audioSource.clip.length && BGMController.Instance.bgmBattleStart == true;
    }
}
