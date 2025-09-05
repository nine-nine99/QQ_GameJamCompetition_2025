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

    [Header("物品的属性")]
    public bool isTransItem = false;
    public Item item;
    [Header("要跳转到的Part")]
    public int partID;
    [Header("要进入的战斗的ID")]
    public int battleID;
    void OnMouseEnter()
    {
        originItem.SetActive(false);
        zoomedItem.SetActive(true);
    }

    void OnMouseExit()
    {
        originItem.SetActive(true);
        zoomedItem.SetActive(false);
    }

    public void Init()
    {
        originItem.SetActive(true);
        zoomedItem.SetActive(false);
    }

    public void Clear()
    {
        originItem.SetActive(true);
        zoomedItem.SetActive(false);
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

        Clear();

        mLevel.ChangePart(partIndex);   // 进入里世界
    }

    private void battleItem()
    {
        if (item != Item.battle) return;

        Send.SendMsg(SendType.Into_Conversation, 2); // 发送消息，准备进入音乐战斗

        Clear();
    }
}
