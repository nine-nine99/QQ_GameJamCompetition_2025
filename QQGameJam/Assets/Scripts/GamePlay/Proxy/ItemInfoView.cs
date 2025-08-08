using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 物品信息类
/// </summary>
public class ItemInfoView {
    private GameObject go;
    private ShopItemInfo itemInfo;
    private Text txtDesc;
    private Text txtProgress;
    private Button btnClose;
    private Button btnUnLock;

    public ItemInfoView(GameObject _go) {
        go = _go;
        txtDesc = go.GetChildControl<Text>("Message/info");
        txtProgress = go.GetChildControl<Text>("Message/process");
        btnClose = go.GetChildControl<Button>("Button/btnOK");
        btnUnLock = go.GetChildControl<Button>("Button/btnUnlock");

        btnClose.onClick.AddListener(OnCloseClick);
        btnUnLock.onClick.AddListener(OnUnLockClick);

        go.SetActive(false);
    }

    public void OpenInfo(ShopItemInfo _itemInfo) {
        itemInfo = _itemInfo;
        go.SetActive(true);
        txtDesc.text = itemInfo.GetDesc();
        txtProgress.text = itemInfo.GetProgress();
        btnUnLock.gameObject.SetActive(itemInfo.itemState == ShopItemState.Lock);
    }

    private void OnUnLockClick() {
        UnLockItemResult result = itemInfo.TryUnLockItem();
        switch (result) {
            case UnLockItemResult.Unlocked:
                //show tip if need
                break;
            case UnLockItemResult.Success:
                //show tip if need
                go.SetActive(false);
                break;
            case UnLockItemResult.Fail:
                //show tip if need
                break;
        }
    }

    private void OnCloseClick() {
        go.SetActive(false);
    }
}
