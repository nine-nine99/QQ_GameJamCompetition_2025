using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : Singleton<NoteSpawner>
{
    public GameObject notePrefab;
    public GameObject noteHoldPrefab;
    public List<Transform> spawnPoints;
    
    // 音节的移动总长
    private float totalDistance = 7f; // 假设音节从上到下移动的总距离为7单位
    public void InitNoteSpawn(List<Transform> transforms)
    {
        spawnPoints = new List<Transform>(transforms);
        notePrefab = Resources.Load("Prefab/Note/TestNote") as GameObject;
        noteHoldPrefab = Resources.Load("Prefab/Note/TestKeyHold") as GameObject;
    }

    public void SpawnNote(SyllableDetail data)
    {
        // 根据音节细节生成音符
        int laneNum = data.positionIndex; // 假设 positionIndex 从 0 开始

        if (laneNum < 0 || laneNum >= spawnPoints.Count)
        {
            Debug.LogWarning("Invalid lane number: " + laneNum);
            return;
        }
        GameObject note;
        if (data.syllableType == SyllableType.Hold)
        {
            note = ObjectPool.Instance.Get("Note", "TestKeyHold");
        }
        else
        {
            note = ObjectPool.Instance.Get("Note", "TestKeyTap");
        }

        note.AddComponent<NoteMover>();
        note.transform.position = spawnPoints[laneNum].position;

        // 可以在这里设置音符的其他属性，比如持续时间等
        float duration = data.duration;
        NoteMover noteMover = note.GetComponent<NoteMover>();
        float speed = totalDistance / duration; // 计算音符的移动速度
        noteMover.speed = speed;
        noteMover.underJudgementLine = spawnPoints[laneNum].position.y - 7 - 4;
    }

}
