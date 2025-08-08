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
        itemImage = transform.Find("itemImage")?.GetComponent<Image>();
        itemText = transform.Find("itemText")?.GetComponent<TextMeshProUGUI>();
    }

    protected override void OnOpen()
    {
        StartCoroutine(EnableCloseAfterRelease());
    }

    IEnumerator EnableCloseAfterRelease()
    {
        btnClose.interactable = false;
        yield return new WaitUntil(() => !Input.GetMouseButton(0));
        yield return null;
        btnClose.interactable = true;
    }

    public void SetContent(Sprite sprite, string text, float width, float height)
    {
        if (itemImage != null && (width > 0 && height > 0))
        {
            itemImage.sprite = sprite;
            var rt = itemImage.rectTransform;
            rt.localScale = new Vector3(width, height, 1f);
        }

        if (itemText != null)
        {
            itemText.text = text;
            itemText.font = customFont;
        }
    }


    protected override void InitMsg() => btnClose.onClick.AddListener(OnCloseClick);
    protected override void ClearMsg() => btnClose.onClick.RemoveListener(OnCloseClick);

    void OnCloseClick() => WindowMgr.Instance.CloseWindow<InteractableItemWindow>();

}
