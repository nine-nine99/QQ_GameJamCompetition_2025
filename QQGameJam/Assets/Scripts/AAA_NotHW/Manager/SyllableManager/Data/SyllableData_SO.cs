using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SyllableData", menuName = "ScriptableObjects/SyllableData")]
public class SyllableData_SO : ScriptableObject
{
    public AudioClip audioClip;
    public List<SyllableDetail> datas; // 音节列表
}

[System.Serializable]
public class SyllableDetail
{
    public int index;
    public int positionIndex; // 音节位置(0, 1, 2, 3, 4)
    public float showTime_1; // 假如是长按音符则就是音符第一个点出现时间
    public float endTime_1; // 假如是长按音符则就是音符第一个点到达判定线的时间
    public float showTime_2; // 假如是长按音符则就是音符尾巴，第二个点的出现时间
    public float endTime_2; // 假如是长按音符则就是音符尾巴，第二个点到达判定线的时间
    public SyllableType syllableType; // 音节类型
    
    // 长按音符专用属性
    public float GetHoldDuration()
    {
        if (syllableType == SyllableType.Hold)
        {
            return endTime_2 - endTime_1; // 长按持续时间
        }
        return 0f;
    }
    
    public float GetShowDuration()
    {
        if (syllableType == SyllableType.Hold)
        {
            return showTime_2 - showTime_1; // 显示持续时间
        }
        return 0f;
    }
}