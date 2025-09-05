using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : SingletonMonoBehavior<BGMController>
{
    public AudioSource bgm => GetComponent<AudioSource>();
    public bool isBegin = false;

    private void OnEnable()
    {
        Send.RegisterMsg(SendType.MusicBattleEnd, OnMusicBattleEnd);
    }
    private void OnDisable()
    {
        Send.UnregisterMsg(SendType.MusicBattleEnd, OnMusicBattleEnd);

    }

    private void OnMusicBattleEnd(params object[] objects)
    {
        isBegin = false;
        EndBGMBattle();
    }

    // TODO:暂时的
    // 游戏具体音游战斗开始
    public void StartBGMBattle()
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
            isBegin = true;
        }
        else
        {
            // 已经在播放时
        }
    }

    // 游戏音游战斗结束
    public void EndBGMBattle()
    {
        if (bgm.isPlaying)
        {
            bgm.Stop();
            isBegin = false;
        }
    }
}
