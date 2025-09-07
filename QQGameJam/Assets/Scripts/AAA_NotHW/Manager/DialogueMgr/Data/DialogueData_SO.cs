using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptableObjects/DialogueData")]
public class DialogueData_SO : ScriptableObject
{
    // 台词列表
    public List<LineDetail> lineDetails;
}

[System.Serializable]
public class LineDetail
{
    public int index;   // 台词序号

    // 背景ID
    public int BGID;
    [Header("人物左ID")]
    public int CharactorID_0;
    [Header("人物右ID")]
    public int CharactorID_1;
    [Header("对话框中的名字")]
    public string txt_name;
    [Header("对话框中的对话")]
    public string txt_dialogue;
}