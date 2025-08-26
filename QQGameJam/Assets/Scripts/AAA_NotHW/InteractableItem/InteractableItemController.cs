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

    public Part_Real realWorld;
    public Part_Melody melodyWorld;

    //图片点击后的尺寸，可以单独设置
    public float width;
    public float height;

    // 是否为关键物品（唱片）
    public bool isKeyItem = false;

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
        if (isKeyItem)
        {
            // 跳转触发：key item点击后进入里世界（暂时设定了melody）
            //并且触发dialogue
            // 打开第 1 个对话
            DialogueMgr.Instance.OpenDialogue(1);

            // 注册结束回调
            DialogueMgr.Instance.onDialogueEnd = () =>
            {
                Debug.Log("对话结束，切换世界");
                realWorld.OnExit();
                melodyWorld.OnEnter();
            };

        }
        else
        {
            // 普通物品：弹出详情窗口
            var spriteRenderer = self.GetComponent<SpriteRenderer>();
            WindowMgr.Instance.OpenWindow<InteractableItemWindow>();
            InteractableItemWindow.Instance.SetContent(
                spriteRenderer.sprite,
                descriptionText,
                width,
                height
            );
        }
    }
}
