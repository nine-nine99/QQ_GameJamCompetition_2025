using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableItemController : MonoBehaviour
{
    private Level mLevel => LevelMgr.Instance.curLevelObj;
    private GameObject originItem => transform.GetChild(0).gameObject;
    private GameObject zoomedItem => transform.GetChild(1).gameObject;
    public string descriptionText = "待填入...";
    [Header("物品ID")]
    public int itemId;
    //图片点击后的尺寸，可以单独设置
    public float width;
    public float height;

    public Item item;
    [Header("要跳转到的Part")]
    public int partID;
    [Header("要进入的战斗的ID")]
    public int battleID;
    [Header("进入战斗对话SO的ID")]
    public int BattleIntoDialogue_ID;  // 开始对话SO的ID = 1000
    [Header("战斗失败时对话数据ID")]
    public int battleFailDialogue_ID;
    [Header("战斗成功时对话数据ID")]
    public int battleFinishDialogue_ID;


    private void Start()
    {
        CloseItem();
    }

    void OnMouseEnter()
    {
        OpenItem();
    }

    void OnMouseExit()
    {
        CloseItem();
    }

    // 鼠标点击时调用
    private void OnMouseDown()
    {
        normalItem();

        transItem(partID);

        battleItem();
    }

    private void normalItem()
    {
        if (item != Item.normal) return;

        // 普通物品：弹出详情窗口
        var spriteRenderer = originItem.GetComponent<SpriteRenderer>();
        WindowMgr.Instance.OpenWindow<InteractableItemWindow>();
        InteractableItemWindow.Instance.SetContent(
            spriteRenderer.sprite,
            descriptionText,
            width,
            height
        );
        return;
    }

    private void transItem(int partIndex)
    {
        if (item != Item.tran) return;

        CloseItem();
        // DialogueCtrl.Instance.FadeOut(() =>
        // {
        //     mLevel.ChangePart(partIndex);   // 进入里世界

        //     // 然后淡入新场景
        //     DialogueCtrl.Instance.FadeIn();
        // });
        // 进入里世界,然后淡入新场景
        DialogueCtrl.Instance.ChangeSceneWithFade(() => mLevel.ChangePart(partIndex));

    }

    private void battleItem()
    {
        if (item != Item.battle) return;
        DialogueCtrl.Instance.OpenDialogueScene(BattleIntoDialogue_ID, () => mLevel.StartMusicBattleScene(battleID, battleFailDialogue_ID, battleFinishDialogue_ID));
        // BAN:弃用
        // Send.SendMsg(SendType.Into_Conversation, 2); // 发送消息，准备进入音乐战斗

        CloseItem();
    }

    public void OpenItem()
    {
        originItem.SetActive(false);
        zoomedItem.SetActive(true);
    }

    public void CloseItem()
    {
        originItem.SetActive(true);
        zoomedItem.SetActive(false);
    }
}
