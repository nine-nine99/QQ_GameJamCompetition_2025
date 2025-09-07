using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Csv2DialogueData : MonoBehaviour
{
    [SerializeField] private TextAsset CSVFile;
    [SerializeField] private DialogueData_SO dialogueData;
    private void Start() {
        Csv2SOData();
    }

    private void Csv2SOData()
    {
        dialogueData.lineDetails.Clear();
        // 按换行符分隔成行，移除为空的行
        string[] lines = CSVFile.text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(",", StringSplitOptions.RemoveEmptyEntries);

            LineDetail data = new LineDetail
            {
                // ID = int.Parse(fields[0].Trim()), // Trim移除空格
                // Name = fields[1].Trim(),
                // Health = int.Parse(fields[2].Trim()),
                // Attack = int.Parse(fields[3].Trim())
                index = int.Parse(fields[0].Trim()),
                BGID = int.Parse(fields[1].Trim()),
                CharactorID_0 = int.Parse(fields[2].Trim()),
                CharactorID_1 = int.Parse(fields[3].Trim()),
                txt_name = fields[4].Trim(),
                txt_dialogue = fields[5].Trim()
            };
            dialogueData.lineDetails.Add(data);
        }
        EditorUtility.SetDirty(dialogueData);
    }
}
