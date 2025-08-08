using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefShop : RefBase {

    public static Dictionary<int, RefShop> cacheMap = new Dictionary<int, RefShop>();

    /// <summary>
    /// 物品ID
    /// </summary>
    public int ItemId;
    /// <summary>
    /// 解锁类型
    /// </summary>
    public UnLockType UnLockType;
    /// <summary>
    /// 参数
    /// </summary>
    public int Param;
    /// <summary>
    /// 描述
    /// </summary>
    public string Desc;

    public override string GetFirstKeyName() {
        return "ItemId";
    }

    public override void LoadByLine(Dictionary<string, string> _value, int _line) {
        base.LoadByLine(_value, _line);
        ItemId = GetInt("ItemId");
        UnLockType = (UnLockType)GetEnum("UnLockType", typeof(UnLockType));
        Param = GetInt("Param");
        Desc = GetString("Desc");
    }

    public static RefShop GetRef(int itemid) {
        RefShop data = null;
        if (cacheMap.TryGetValue(itemid, out data)) {
            return data;
        }

        if (data == null) {
            Debug.LogError("error RefShop key:" + itemid);
        }
        return data;
    }
}
