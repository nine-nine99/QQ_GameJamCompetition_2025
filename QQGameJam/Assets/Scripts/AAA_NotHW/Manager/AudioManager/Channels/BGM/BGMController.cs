using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : SingletonMonoBehavior<BGMController>
{
    public AudioSource bgm => GetComponent<AudioSource>();
    public bool bgmBattleStart = false;

    private void OnEnable()
    {
        Send.RegisterMsg(SendType.MusicBattleEnd, OnMusicBattleEnd);
    }
    private void OnDisable()
    {
        Send.UnregisterMsg(SendType.MusicBattleEnd, OnMusicBattleEnd);

    }

    void Update()
    {
        if (bgmBattleStart == true)
        {
            // 监听战斗是否结束
            if (BGMListener.Instance.IsFinished())
            {
                // NOTE:这里是音乐战斗结束的地方，成功状态下
                Send.SendMsg(SendType.MusicBattleEnd, false);
                // OnMusicBattleEnd();
            }
        }
    }
    // 游戏具体音游战斗开始
    public void Start_BGMBattle(int ID)
    {
        // 开始播放
        InitBGMBattle_Data(ID);

        if (!bgm.isPlaying)
        {
            // 开始播放
            bgm.Play();
            bgmBattleStart = true;
        }
        else
        {
            // 已经在播放时
        }
    }
    private void OnMusicBattleEnd(params object[] objects)
    {
        End_BGMBattle();
    }

    // 游戏音游战斗结束
    public void End_BGMBattle()
    {
        if (bgm.isPlaying)
        {
            bgm.Stop();
            bgmBattleStart = false;
        }
    }

    /// <summary>
    /// 当前谱面总数据初始化
    /// </summary>
    /// <param name="SyllableData_ID">谱面数据ID</param>
    public void InitBGMBattle_Data(int SyllableData_ID)
    {
        bgmBattleStart = true;

        // 通过ID获取当前的铺面数据
        SyllableData_SO syllableData_SO = RefSyllableDatas.GetSyllableData_SOByID(SyllableData_ID);
        BattleSceneData_SO battleSceneData_SO = RefSyllableDatas.GetBattleSceneData_SOByID(SyllableData_ID);

        Debug.Log("Current SyllableData_ID =  " + SyllableData_ID);

        // 初始化音节乐谱脚本
        TODO: SyllableManager.Instance.BGMBattle_InitSyllableData(syllableData_SO, battleSceneData_SO);

        // 挂载clip
        bgm.clip = SyllableManager.Instance.mSyllableData.audioClip;
    }
}
