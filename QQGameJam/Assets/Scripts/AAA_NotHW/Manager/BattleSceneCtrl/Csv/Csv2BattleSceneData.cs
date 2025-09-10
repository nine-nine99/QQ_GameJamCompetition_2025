using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Csv2BattleSceneData : MonoBehaviour
{
    [SerializeField] private TextAsset CSVFile;
    // [SerializeField] private List<EnemyData> enemyDatas;
    [SerializeField] private BattleSceneData_SO battleSceneData;
    private void Start()
    {
        Csv2SOData();
    }

    private void Csv2SOData()
    {
        battleSceneData.battleSceneDetails.Clear();
        // 按换行符分隔成行，移除为空的行
        string[] lines = CSVFile.text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(",", StringSplitOptions.RemoveEmptyEntries);

            BattleSceneDetail data = new BattleSceneDetail
            {
                // ID = int.Parse(fields[0].Trim()), // Trim移除空格
                // Name = fields[1].Trim(),
                // Health = int.Parse(fields[2].Trim()),
                // Attack = int.Parse(fields[3].Trim())
                index = int.Parse(fields[0].Trim()),
                BGID = int.Parse(fields[1].Trim()),
                AniID = int.Parse(fields[2].Trim()),
                showTime = float.Parse(fields[3].Trim()),
            };
            battleSceneData.battleSceneDetails.Add(data);
        }
        EditorUtility.SetDirty(battleSceneData);
    }
}
