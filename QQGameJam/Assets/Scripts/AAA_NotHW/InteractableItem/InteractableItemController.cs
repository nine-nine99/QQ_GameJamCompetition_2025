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
        var spriteRenderer = self.GetComponent<SpriteRenderer>();

        WindowMgr.Instance.OpenWindow<InteractableItemWindow>();
        InteractableItemWindow.Instance.SetContent(
            spriteRenderer.sprite,
            descriptionText
        );
    }

}
