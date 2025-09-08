using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyllableManager : SingletonMonoBehavior<SyllableManager>
{
    public SyllableData_SO mSyllableData; // 音节数据
    private float arrivalTime = 0;
    private float duration = 0;
    private float actualTime = 0;
    private int index = 0;
    private float currentTime => BGMListener.Instance.GetCurrentTime();
    private SyllableDetail currentDetail;   // 当前音节数据

    void Update()
    {
        GenerateNode();
    }

    // 当歌曲开始播放初始化谱面数据
    public void BGMBattle_InitSyllableData(SyllableData_SO CurData)
    {
        index = 0;

        mSyllableData = CurData;

        currentDetail = null;
    }

    private void GenerateNode()
    {
        if (mSyllableData == null) return;
        if (index >= mSyllableData.datas.Count) return;

        // 战斗开始时初始化音节管理器
        if (currentDetail == null)
        {
            currentDetail = mSyllableData.datas[index];

            if (currentDetail == null)
            {
                Debug.Log("出现错误");
                return;
            }

            // 处理音节的到达时间和持续时间
            arrivalTime = currentDetail.arrivalTime;
            duration = currentDetail.duration;
            // 实际生成时间
            actualTime = arrivalTime - duration;
        }

        // 到达具体音节时间时生成音节
        if (currentTime >= actualTime)
        {
            // 在这里处理音节的播放逻辑
            // 触发生成音节事件
            NoteSpawner.Instance.SpawnNote(currentDetail);

            index++;

            currentDetail = null;
        }

        // 当音节生成结束时
        if (index >= mSyllableData.datas.Count || currentTime >= BGMListener.Instance.GetTotalLength())
        {
            currentDetail = null;
            return;
        }
    }
}
