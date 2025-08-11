using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Csv2SyllableData : MonoBehaviour
{
    [SerializeField] private TextAsset CSVFile;
    // [SerializeField] private List<EnemyData> enemyDatas;
    [SerializeField] private SyllableData_SO syllableData;
    private void Start()
    {
        Csv2SOData();
    }

    private void Csv2SOData()
    {
        syllableData.datas.Clear();
        // 按换行符分隔成行，移除为空的行
        string[] lines = CSVFile.text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(",", StringSplitOptions.RemoveEmptyEntries);

            SyllableDetail data = new SyllableDetail
            {
                // ID = int.Parse(fields[0].Trim()), // Trim移除空格
                // Name = fields[1].Trim(),
                // Health = int.Parse(fields[2].Trim()),
                // Attack = int.Parse(fields[3].Trim())
                index = int.Parse(fields[0].Trim()),
                arrivalTime = float.Parse(fields[1].Trim()),
                positionIndex = int.Parse(fields[2].Trim()),
                duration = float.Parse(fields[3].Trim()),
                syllableType = (SyllableType)Enum.Parse(typeof(SyllableType), fields[4].Trim())
            };
            syllableData.datas.Add(data);
        }
        EditorUtility.SetDirty(syllableData);
    }
}
