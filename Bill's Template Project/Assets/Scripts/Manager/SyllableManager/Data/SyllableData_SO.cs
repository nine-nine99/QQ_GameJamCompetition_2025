using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SyllableData", menuName = "ScriptableObjects/SyllableData")]
public class SyllableData_SO : ScriptableObject
{
    public AudioClip audioClip;
    public List<SyllableDetail> syllableDetails; // 音节列表
}

[System.Serializable]
public class SyllableDetail
{
    public float arrivalTime; // 到达判定线的时间
    public int positionIndex; // 音节位置(0, 1, 2, 3, 4)
    public float duration; // 音节持续时间
    public SyllableType syllableType; // 音节类型
}