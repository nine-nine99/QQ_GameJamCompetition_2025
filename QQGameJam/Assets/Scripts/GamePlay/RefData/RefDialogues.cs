using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefDialogues : RefBase
{
    public static Dictionary<int, RefDialogues> cacheMap = new Dictionary<int, RefDialogues>();
    public int ID;  // 图片ID
    public string Path; // 图片路径
    public string Desc; // 描述

    public override string GetFirstKeyName()
    {
        return "ID";
    }
    public override void LoadByLine(Dictionary<string, string> _value, int _line)
    {
        base.LoadByLine(_value, _line);
        ID = GetInt("ID");
        Path = GetString("Path");
        Desc = GetString("Desc");
    }

    public static RefDialogues GetRef(int ID)
    {
        RefDialogues data = null;
        if (cacheMap.TryGetValue(ID, out data))
        {
            return data;
        }

        if (data == null)
        {
            Debug.LogError("error RefDialogues key:" + ID);
        }
        return data;
    }

    // 获取对应的SO文件
    /// <summary>
    /// 通过ID获取Sprite资源
    /// </summary>
    /// <param name="ID">图片ID</param>
    /// <returns>对应的Sprite资源，如果未找到则返回null</returns>
    public static DialogueData_SO GetDialogueData_SOByID(int ID)
    {
        // 通过ID获取RefImage数据
        RefDialogues refData = GetRef(ID);
        if (refData == null)
        {
            Debug.LogError($"RefImage with ID {ID} not found!");
            return null;
        }

        // 从Resources文件夹加载图片资源
        string resourcePath = refData.Path.Replace("Resources/", ""); // 移除"Resources/"前缀
        DialogueData_SO dialogueData_SO = Resources.Load<DialogueData_SO>(resourcePath);

        if (dialogueData_SO == null)
        {
            Debug.LogError($"Failed to load DialogueData_SO from path: {refData.Path}");
            return null;
        }
        Debug.Log($"Successfully loaded DialogueData_SO: {dialogueData_SO.name} from path: {refData.Path}");
        return dialogueData_SO;
    }
}
