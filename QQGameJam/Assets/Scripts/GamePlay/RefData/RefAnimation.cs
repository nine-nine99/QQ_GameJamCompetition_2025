using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefAnimation : RefBase
{
    public static Dictionary<int, RefAnimation> cacheMap = new Dictionary<int, RefAnimation>();
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

    public static RefAnimation GetRef(int ID)
    {
        RefAnimation data = null;
        if (cacheMap.TryGetValue(ID, out data))
        {
            return data;
        }

        if (data == null)
        {
            Debug.LogError("error RefAnimation key:" + ID);
        }
        return data;
    }

    /// <summary>
    /// 通过ID获取Animation资源
    /// </summary>
    /// <param name="ID">图片ID</param>
    /// <returns>对应的Sprite资源，如果未找到则返回null</returns>
    public static AnimationClip GetAnimationByID(int ID)
    {
        // 通过ID获取RefImage数据
        RefAnimation refData = GetRef(ID);
        if (refData == null)
        {
            Debug.LogError($"RefImage with ID {ID} not found!");
            return null;
        }

        // 从Resources文件夹加载图片资源
        string resourcePath = refData.Path.Replace("Resources/", ""); // 移除"Resources/"前缀
        AnimationClip clip = Resources.Load<AnimationClip>(resourcePath);

        if (clip == null)
        {
            Debug.LogError($"Failed to load sprite from path: {refData.Path}");
            return null;
        }
        // Debug.Log($"Successfully loaded sprite: {sprite.name} from path: {refData.Path}");
        return clip;
    }
}
