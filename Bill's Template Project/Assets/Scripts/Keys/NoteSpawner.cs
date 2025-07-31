using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : Singleton<NoteSpawner>
{
    public GameObject notePrefab;
    public GameObject noteHoldPrefab;
    public Transform[] spawnPoints;//move prefabs in NoteManager to check judgement line position
    // public int spawnInterval = 1;//just for test, delete later
    // private float timer = 0f;
    // 音节的移动总长度
    private float totalDistance = 7f; // 假设音节从上到下移动的总距离为7单位

    // void Update()
    // {
    //     timer += Time.deltaTime;
    //     if (timer >= spawnInterval)
    //     {
    //         SpawnNote();
    //         timer = 0f;
    //     }
    // }

    // private void SpawnNote()
    // {
    //     int laneNum = Random.Range(0, spawnPoints.Length);

    //     //测试：有50%的概率可以生成长按键/短按键
    //     bool spawnHold = Random.value < 0.5f;
    //     GameObject prefabToSpawn = spawnHold ? noteHoldPrefab : notePrefab;

    //     GameObject note = Instantiate(prefabToSpawn, spawnPoints[laneNum].position, Quaternion.identity);
    //     note.AddComponent<NoteMover>();
    // }

    public void SpawnNote(SyllableDetail syllableDetail)
    {
        // 根据音节细节生成音符
        int laneNum = syllableDetail.positionIndex; // 假设 positionIndex 从 0 开始

        if (laneNum < 0 || laneNum >= spawnPoints.Length)
        {
            Debug.LogWarning("Invalid lane number: " + laneNum);
            return;
        }

        GameObject note = Instantiate(notePrefab, spawnPoints[laneNum].position, Quaternion.identity);
        note.AddComponent<NoteMover>();

        // 可以在这里设置音符的其他属性，比如持续时间等
        float duration = syllableDetail.duration;
        NoteMover noteMover = note.GetComponent<NoteMover>();
        float speed = totalDistance / duration; // 计算音符的移动速度
        noteMover.speed = speed;
    }

}
