using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : Singleton<BGMController>
{
    public AudioSource bgm => GetComponent<AudioSource>();
    private void OnEnable()
    {
        EventHandler.StartBGMButtonPressedEvent += StartBGM;
    }
    private void OnDisable()
    {
        EventHandler.StartBGMButtonPressedEvent -= StartBGM;
    }

    public void StartBGM()
    {
        // 开始播放
        if (bgm.clip == null)
        {
            Debug.LogWarning("BGM Clip 为空");
            return;
        }
        if (!bgm.isPlaying)
        {
            // 挂载clip
            bgm.clip = SyllableManager.Instance.syllableData.audioClip;
            // 启动音节乐谱脚本
            SyllableManager.Instance.OnMusicStart();
            // 开始播放
            bgm.Play();
        }
        else
        {
            // 已经在播放时
        }
    }
}
