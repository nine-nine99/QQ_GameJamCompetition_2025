using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableItemWindow : BaseWindowWrapper<InteractableItemWindow>
{
    private Button btnClose;
    private Image itemImage;
    private TextMeshProUGUI itemText;
    public TMP_FontAsset customFont;

    protected override void InitCtrl()
    {
        btnClose = transform.Find("btnClose")?.GetComponent<Button>();
        if (btnClose == null) Debug.LogError("btnClose 未找到");

        itemImage = transform.Find("itemImage")?.GetComponent<Image>();
        if (itemImage == null) Debug.LogError("itemImage 未找到");

        itemText = transform.Find("itemText")?.GetComponent<TextMeshProUGUI>();
        if (itemText == null) Debug.LogError("itemText 未找到");
    }

    protected override void OnPreOpen()
    {

    }

    protected override void OnOpen()
    {

    }

    public void SetContent(Sprite sprite, string text)
    {
        if (itemImage != null)
        {
            itemImage.sprite = sprite;
            var rt = itemImage.rectTransform;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(-1416f, -1064f);
            rt.localScale = new Vector3(7f, 7f, 7f);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 887.46f);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 500f);
        }

        if (itemText != null)
        {
            itemText.text = text;
            itemText.font = customFont;
        }
    }


    protected override void InitMsg()
    {
        btnClose.onClick.AddListener(OnCloseClick);
    }

    protected override void ClearMsg()
    {
        btnClose.onClick.RemoveListener(OnCloseClick);
    }

    private void OnCloseClick()
    {
        WindowMgr.Instance.CloseWindow<InteractableItemWindow>();
    }
}
