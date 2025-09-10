using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleSceneData", menuName = "ScriptableObjects/BattleSceneData")]
public class BattleSceneData_SO : ScriptableObject
{
    // 场景细节数据列表
    public List<BattleSceneDetail> battleSceneDetails;
}

[System.Serializable]
public class BattleSceneDetail
{
    public int index;   // 场景细节数据序号
    // 背景ID
    public int BGID;
    // 动画ID
    public int AniID;
    // 出现时间
    public float showTime;
}
