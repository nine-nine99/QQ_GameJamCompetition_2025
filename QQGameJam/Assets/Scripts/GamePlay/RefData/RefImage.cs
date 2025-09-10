using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 获取所有需要多次调用的图片
/// </summary>
public class RefImage : RefBase
{
    public static Dictionary<int, RefImage> cacheMap = new Dictionary<int, RefImage>();
    public int ID;  // 图片ID
    public string Path; // 图片路径

    public override string GetFirstKeyName()
    {
        return "ID";
    }

    public override void LoadByLine(Dictionary<string, string> _value, int _line)
    {
        base.LoadByLine(_value, _line);
        ID = GetInt("ID");
        Path = GetString("Path");
    }

    public static RefImage GetRef(int ID)
    {
        RefImage data = null;
        if (cacheMap.TryGetValue(ID, out data))
        {
            return data;
        }

        if (data == null)
        {
            Debug.LogError("error RefImage key:" + ID);
        }
        return data;
    }

    /// <summary>
    /// 通过ID获取Sprite资源
    /// </summary>
    /// <param name="ID">图片ID</param>
    /// <returns>对应的Sprite资源，如果未找到则返回null</returns>
    public static Sprite GetSpriteByID(int ID)
    {
        // 通过ID获取RefImage数据
        RefImage refData = GetRef(ID);
        if (refData == null)
        {
            Debug.LogError($"RefImage with ID {ID} not found!");
            return null;
        }

        // 从Resources文件夹加载图片资源
        string resourcePath = refData.Path.Replace("Resources/", ""); // 移除"Resources/"前缀
        Sprite sprite = Resources.Load<Sprite>(resourcePath);

        if (sprite == null)
        {
            Debug.LogError($"Failed to load sprite from path: {refData.Path}");
            return null;
        }
        // Debug.Log($"Successfully loaded sprite: {sprite.name} from path: {refData.Path}");
        return sprite;
    }
}
