using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyllableManager : SingletonMonoBehavior<SyllableManager>
{
    public SyllableData_SO syllableData; // 音节数据
    private float arrivalTime = 0;
    private float duration = 0;
    private float actualTime = 0;
    private int index = 0;
    private bool isPlaying = false;
    private float currentTime => BGMListener.Instance.GetCurrentTime();
    private SyllableDetail currentDetail = null;


    void Update()
    {
        if (isPlaying == true)
        {
            SongNodeStartIni();
        }
    }
    // 当歌曲开始播放的时候的函数
    public void OnMusicStart()
    {
        // 在这里处理音节的播放
        if (syllableData == null || syllableData.syllableDetails == null || syllableData.syllableDetails.Count == 0)
        {
            return;
        }
        isPlaying = true;
    }
    public void SongNodeStartIni()
    {
        if (index >= syllableData.syllableDetails.Count)
        {
            isPlaying = false; // 如果索引超出范围，退出循环
            return;
        }
        if (currentDetail == null)
        {
            currentDetail = syllableData.syllableDetails[index];
            if (currentDetail == null)
            {
                isPlaying = false;
                Debug.Log("出现错误");
                return;
            }
            // 处理音节的到达时间和持续时间
            // 这里可以添加更多的逻辑来处理音节的播放
            arrivalTime = currentDetail.arrivalTime;
            duration = currentDetail.duration;
            // 实际生成时间
            actualTime = arrivalTime - duration;
        }

        if (currentTime >= actualTime)
        {
            // 在这里处理音节的播放逻辑
            // 触发生成音节事件
            NoteSpawner.Instance.SpawnNote(currentDetail);

            index++;
            currentDetail = null;
        }
        if (currentTime >= BGMListener.Instance.GetTotalLength())
        {
            currentDetail = null;
            // 乐曲播放结束了
        }
    }
}
