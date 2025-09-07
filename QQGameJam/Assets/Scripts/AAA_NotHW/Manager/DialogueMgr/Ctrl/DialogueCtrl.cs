using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueCtrl : SingletonMonoBehavior<DialogueCtrl>
{
    [Header("对话框对话事件触发")]
    public Button dialogueImage;

    // 背景，左右两个人物，对话框中的名字，对话框中的对话
    [Header("背景")]
    public Image BG;
    [Header("人物左")]
    public Image Charactor_0;
    [Header("人物右")]
    public Image Charactor_1;
    [Header("对话框中的名字")]
    public TextMeshProUGUI txt_name;
    [Header("对话框中的对话")]
    public TextMeshProUGUI txt_dialogue;

    [Header("当前的对话SO文件")]
    public DialogueData_SO Dia_SO;
    private int CurLine_Index;
    private void OnEnable()
    {
        // 添加点击事件监听
        if (dialogueImage != null)
        {
            dialogueImage.onClick.AddListener(OnImageClicked);
        }

        //TODO: 暂时的
        Init_Dialogue();
    }

    // 初始化对话
    private void Init_Dialogue()
    {
        CurLine_Index = 0;  // 初始化对话Index
        Refesh();
    }

    // 当对话框被点击时
    private void OnImageClicked()
    {
        Debug.Log("玩家点击了对话图片!");

        // 每次点击跳到下一段对话

        // 在这里添加你的点击逻辑
        // Send.SendMsg(SendType.DialogueImageClicked);

        CurLine_Index++;
        if (CurLine_Index >= Dia_SO.lineDetails.Count)
        {
            CurLine_Index--;
            return;
        }
        Refesh();
    }

    // 刷新对话场景
    private void Refesh()
    {
        LineDetail CurLine = Dia_SO.lineDetails[CurLine_Index];
        // 背景
        BG.sprite = RefImage.GetSpriteByID(CurLine.BGID);
        // 背景
        Charactor_0.sprite = RefImage.GetSpriteByID(CurLine.CharactorID_0);
        // 背景
        Charactor_1.sprite = RefImage.GetSpriteByID(CurLine.CharactorID_1);
        // 对话框名字
        txt_name.text = CurLine.txt_name;
        // 对话框中的对话
        txt_dialogue.text = CurLine.txt_dialogue;
    }


    private void OnDestroy()
    {
        // 清理监听器
        if (dialogueImage != null)
        {
            dialogueImage.onClick.RemoveListener(OnImageClicked);
        }
    }
}
