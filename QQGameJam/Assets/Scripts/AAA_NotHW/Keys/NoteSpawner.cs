using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NoteSpawner : Singleton<NoteSpawner>
{
    public List<Transform> spawnPoints;
    public List<Transform> targetPoints;
    private float Distance => spawnPoints[0].transform.position.y - targetPoints[0].transform.position.y;
    // 音节的移动总长
    private float totalDistance => spawnPoints[0].transform.position.y - targetPoints[0].transform.position.y; // 假设音节从上到下移动的总距离为7单位
    public void InitNoteSpawn(List<Transform> spawnPoints, List<Transform> targetPoints)
    {
        this.spawnPoints = new List<Transform>(spawnPoints);
        this.targetPoints = new List<Transform>(targetPoints);

        // notePrefab = Resources.Load("Prefab/Note/NormalNote") as GameObject;
        // noteHoldPrefab = Resources.Load("Prefab/Note/LongNote") as GameObject;
    }

    public void SpawnNote(SyllableDetail data)
    {
        // 根据音节细节生成音符
        int index = data.positionIndex - 1; // 假设 positionIndex 从 0 开始

        if (index < 0 || index >= spawnPoints.Count)
        {
            Debug.LogWarning("Invalid lane number: " + index);
            return;
        }
        GameObject note;

        if (data.syllableType == SyllableType.Hold)
        {
            note = ObjectPool.Instance.Get("Note", "LongNote");
            LongNote longPressNote = note.GetComponent<LongNote>();
            note.transform.position = spawnPoints[index].position;

            longPressNote.Init(data, Distance, -totalDistance);
            // Debug.LogWarning("totalDistance = " + totalDistance + "; " + (spawnPoints[index].position.y - 7 - 4));
        }
        else
        {
            note = ObjectPool.Instance.Get("Note", "NormalNote");
            // note.AddComponent<NormalNote>();
            note.transform.position = spawnPoints[index].position;
            NormalNote normalNote = note.GetComponent<NormalNote>();
            
            normalNote.Init(data, Distance, -totalDistance);
        }
    }

}
