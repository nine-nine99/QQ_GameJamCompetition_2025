using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;
    public Transform[] spawnPoints;//move prefabs in NoteManager to check judgement line position
    public int spawnInterval = 1;//just for test, delete later

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnNote();
            timer = 0f;
        }
    }

    private void SpawnNote()
    {
        int laneNum = Random.Range(0, spawnPoints.Length);

        GameObject note = Instantiate(notePrefab, spawnPoints[laneNum].position, Quaternion.identity);
        note.AddComponent<NoteMover>();
    }

}
