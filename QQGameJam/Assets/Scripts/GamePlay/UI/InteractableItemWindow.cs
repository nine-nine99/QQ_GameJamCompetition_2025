using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableItemWindow : BaseWindowWrapper<InteractableItemWindow>
{
    private Button btnClose;

    protected override void InitCtrl()
    {
        btnClose = transform.Find("btnClose").GetComponent<Button>();
    }

    protected override void OnPreOpen()
    {

    }

    protected override void OnOpen()
    {

    }

    protected override void OnClose()
    {
        base.OnClose();
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
