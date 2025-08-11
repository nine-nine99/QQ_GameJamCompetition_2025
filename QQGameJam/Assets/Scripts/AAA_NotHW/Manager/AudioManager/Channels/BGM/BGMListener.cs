using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMListener : SingletonMonoBehavior<BGMListener>
{
    private AudioSource audioSource => transform.GetComponent<AudioSource>();
    void Update()
    {
        if (audioSource.isPlaying)
        {
            // 输出当前播放时间和音频总长度
            // Debug.Log($"Current Time: {audioSource.time} / Total Length: {audioSource.clip.length}");
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
