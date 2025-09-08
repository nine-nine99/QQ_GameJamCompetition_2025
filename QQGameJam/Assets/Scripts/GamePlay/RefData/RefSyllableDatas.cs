using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefSyllableDatas : RefBase
{
    public static Dictionary<int, RefSyllableDatas> cacheMap = new Dictionary<int, RefSyllableDatas>();
    public int ID;  // ID
    public string Path; // 谱面SO文件地址
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
    public static RefSyllableDatas GetRef(int ID)
    {
        RefSyllableDatas data = null;
        if (cacheMap.TryGetValue(ID, out data))
        {
            return data;
        }

        if (data == null)
        {
            Debug.LogError("error RefSyllableDatas key:" + ID);
        }
        return data;
    }

    public static SyllableData_SO GetSyllableData_SOByID(int ID)
    {
        // 通过ID获取RefImage数据
        RefSyllableDatas refData = GetRef(ID);
        if (refData == null)
        {
            Debug.LogError($"RefImage with ID {ID} not found!");
            return null;
        }

        // 从Resources文件夹加载图片资源
        string resourcePath = refData.Path.Replace("Resources/", ""); // 移除"Resources/"前缀
        SyllableData_SO syllableData_SO = Resources.Load<SyllableData_SO>(resourcePath);

        if (syllableData_SO == null)
        {
            Debug.LogError($"Failed to load SyllableData_SO from path: {refData.Path}");
            return null;
        }
        Debug.Log($"Successfully loaded SyllableData_SO: {syllableData_SO.name} from path: {refData.Path}");
        return syllableData_SO;
    }
}
