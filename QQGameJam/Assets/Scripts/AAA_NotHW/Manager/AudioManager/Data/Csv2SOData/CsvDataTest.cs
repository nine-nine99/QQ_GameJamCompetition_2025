using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class CsvDataTest : MonoBehaviour
{
    [SerializeField] private TextAsset enemyCSVFile;
    // [SerializeField] private List<EnemyData> enemyDatas;
    [SerializeField] private EnemyData_SO enemyData_so;
    private void Start()
    {
        Csv2EnemyData();
    }

    private void Csv2EnemyData()
    {
        enemyData_so.data.Clear();
        // 按换行符分隔成行，移除为空的行
        string[] lines = enemyCSVFile.text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        
        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(",", StringSplitOptions.RemoveEmptyEntries);

            EnemyData data = new EnemyData
            {
                ID = int.Parse(fields[0].Trim()), // Trim移除空格
                Name = fields[1].Trim(),
                Health = int.Parse(fields[2].Trim()),
                Attack = int.Parse(fields[3].Trim())
            };
            enemyData_so.data.Add(data);
        }
        EditorUtility.SetDirty(enemyData_so);
    }
}


[Serializable]
public class EnemyData {
    public int ID;
    public string Name;
    public int Health;
    public int Attack;
}