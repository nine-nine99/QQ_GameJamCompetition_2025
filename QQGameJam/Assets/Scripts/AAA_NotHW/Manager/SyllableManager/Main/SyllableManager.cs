using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyllableManager : SingletonMonoBehavior<SyllableManager>
{
    public SyllableData_SO mSyllableData; // 音节数据
    private SyllableDetail mSyllableDetail;   // 当前音节数据
    private float ActualTime_Syllable = 0;
    private int index_Syllable = 0;

    public BattleSceneData_SO mBattleSceneData; // 演出数据
    private BattleSceneDetail mBattleSceneDetail; // 演出数据
    private float ActualTime_BattleScene = 0;
    private int index_BattleScene = 0;


    private float currentTime => BGMListener.Instance.GetCurrentTime();

    void Update()
    {
        // 开始战斗时
        // 播放动画
        // BattleSceneCtrl.Instance.PlayerClip(null);
        PlayeAni();
        // 生成音节
        GenerateNode();
    }

    private void PlayeAni()
    {
        // 提前检查是否已完成或数据无效
        if (mBattleSceneData == null || index_BattleScene >= mBattleSceneData.battleSceneDetails.Count || currentTime >= BGMListener.Instance.GetTotalLength())
        {
            mBattleSceneDetail = null;
            return;
        }

        // 初始化当前音节数据
        if (mBattleSceneDetail == null)
        {
            mBattleSceneDetail = mBattleSceneData.battleSceneDetails[index_BattleScene];
            if (mBattleSceneDetail == null)
            {
                Debug.LogError($"音节数据错误: index {index_BattleScene}");
                index_BattleScene++; // 跳过错误数据
                return;
            }
            ActualTime_BattleScene = mBattleSceneDetail.showTime;
        }

        // 检查是否到达生成时间
        if (currentTime >= ActualTime_BattleScene)
        {
            // NoteSpawner.Instance.SpawnNote(mSyllableDetail);
            AnimationClip clip = RefAnimation.GetAnimationByID(mBattleSceneDetail.AniID);
            // 播放动画
            BattleSceneCtrl.Instance.PlayerClip(clip);

            // 准备下一个音节
            index_BattleScene++;
            mBattleSceneDetail = null;
        }
    }

    private void GenerateNode()
    {
        // 提前检查是否已完成或数据无效
        if (mSyllableData == null || index_Syllable >= mSyllableData.datas.Count || currentTime >= BGMListener.Instance.GetTotalLength())
        {
            mSyllableDetail = null;
            return;
        }

        // 初始化当前音节数据
        if (mSyllableDetail == null)
        {
            mSyllableDetail = mSyllableData.datas[index_Syllable];
            if (mSyllableDetail == null)
            {
                Debug.LogError($"音节数据错误: index {index_Syllable}");
                index_Syllable++; // 跳过错误数据
                return;
            }
            ActualTime_Syllable = mSyllableDetail.showTime_1;
        }

        // 检查是否到达生成时间
        if (currentTime >= ActualTime_Syllable)
        {
            NoteSpawner.Instance.SpawnNote(mSyllableDetail);

            // 准备下一个音节
            index_Syllable++;
            mSyllableDetail = null;
        }
    }
    // 当歌曲开始播放初始化谱面数据
    public void BGMBattle_InitSyllableData(SyllableData_SO syllableData_, BattleSceneData_SO battleSceneData_)
    {
        index_Syllable = 0;
        index_BattleScene = 0;

        mSyllableData = syllableData_;
        mSyllableDetail = null;

        mBattleSceneData = battleSceneData_;
        mBattleSceneDetail = null;
    }
}
