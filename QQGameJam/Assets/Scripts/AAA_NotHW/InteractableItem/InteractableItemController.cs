using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableItemController : MonoBehaviour
{
    private Vector3 originalScale;
    public float scaleMultiplier = 1.4f;

    // public GameObject blackOverlay;
    public GameObject zoomedItem;
    public GameObject self;
    public string descriptionText = "待填入...";

    [Header("世界引用")]
    public Part_Real realWorld;
    public Part_Melody melodyWorld;
    public Part_InsideWorld insideWorld;

    //图片点击后的尺寸，可以单独设置
    public float width;
    public float height;

    // 是否为关键物品（唱片）
    public bool isKeyItem = false;

    [Header("物品参数")]
    public int itemId;        // 唯一编号，用来区分不同 key item

    public enum PlayPosition
    {
        Prev,
        After
    }

    void Start()
    {
    }

    void OnMouseEnter()
    {
        self.SetActive(false);
        zoomedItem.SetActive(true);
    }

    void OnMouseExit()
    {
        self.SetActive(true);
        zoomedItem.SetActive(false);
    }

    // 鼠标点击时调用
    private void OnMouseDown()
    {
        // 普通物品：弹出详情窗口
        if (!isKeyItem)
        {
            var spriteRenderer = self.GetComponent<SpriteRenderer>();
            WindowMgr.Instance.OpenWindow<InteractableItemWindow>();
            InteractableItemWindow.Instance.SetContent(
                spriteRenderer.sprite,
                descriptionText,
                width,
                height
            );
            return;
        }

        //如果是key item
        PlayPosition dialogue_PlayPosition = PlayPosition.Prev;
        bool MusicBool = false;
        int dialogueId = 0;
        IPart targetWorld = null;

        if (isKeyItem)
        {
            // Debug.Log("hello" + itemId);
            // 根据 itemId 分配不同对话 & 世界
            switch (itemId)
            {
                //现实世界-> 里世界（第一章）
                case 0:
                    dialogueId = 1;
                    dialogue_PlayPosition = PlayPosition.Prev;
                    targetWorld = insideWorld;
                    Debug.Log("[KeyItem] 选择了物品 1 → 对话 1 → insideWorld");
                    break;
                case 1://里世界（第一章） -> melody对话 -> 音乐战斗场面
                    dialogueId = 2;
                    dialogue_PlayPosition = PlayPosition.Prev;
                    targetWorld = null;
                    MusicBool = true;
                    break;
                case 2:
                    dialogueId = 3;
                    targetWorld = realWorld;
                    break;
                default:
                    Debug.LogWarning($"未定义的 key item ID: {itemId}");
                    return;
            }
            if (dialogueId != 0)
            {
                // 如果对话在切换场景前
                if (dialogue_PlayPosition == PlayPosition.Prev)
                {
                    DialogueMgr.Instance.OpenDialogue(dialogueId);
                    // 对话结束后切换世界或者音乐战斗
                    DialogueMgr.Instance.onDialogueEnd = () =>
                    {
                        DialogueMgr.Instance.CloseDialogue(dialogueId);
                        if (realWorld != null) realWorld.OnExit();
                        else if (insideWorld != null)
                        {
                            // Debug.Log("you did");
                            insideWorld.OnExit();
                        }

                        if (MusicBool)
                        {
                            FindObjectOfType<Level>().OnConversationOver(new object[] { 1 });
                        }
    
                        if (targetWorld != null) PartManager.Instance.SwitchTo(targetWorld);
                    };
                }
                else
                {
                    // 如果对话在切换场景后
                    if (realWorld != null) realWorld.OnExit();
                    if (insideWorld != null) insideWorld.OnExit();
                    if (targetWorld != null) PartManager.Instance.SwitchTo(targetWorld);
                    DialogueMgr.Instance.OpenDialogue(dialogueId);
                }

            }
            else
            {
                if (realWorld != null) realWorld.OnExit();
                if (insideWorld != null) insideWorld.OnExit();
                PartManager.Instance.SwitchTo(targetWorld);
            }
        }
    }
}
