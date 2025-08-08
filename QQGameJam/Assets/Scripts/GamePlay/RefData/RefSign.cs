using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefSign : RefBase {

    public static Dictionary<int, RefSign> cacheMap = new Dictionary<int, RefSign>();

    public int Day;
    public RewardType RewardType;
    public int RewardNum;

    public override string GetFirstKeyName() {
        return "Day";
    }

    public override void LoadByLine(Dictionary<string, string> _value, int _line) {
        base.LoadByLine(_value, _line);
        Day = GetInt("Day");
        RewardType = (RewardType)GetEnum("RewardType", typeof(RewardType));
        RewardNum = GetInt("RewardNum");
    }

    public static RefSign GetRef(int day) {
        RefSign data = null;
        if (cacheMap.TryGetValue(day, out data)) {
            return data;
        }

        if (data == null) {
            Debug.LogError("error RefSign key:" + day);
        }
        return data;
    }
}
