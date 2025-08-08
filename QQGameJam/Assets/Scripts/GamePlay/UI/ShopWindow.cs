using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 商店界面
/// </summary>
public class ShopWindow : BaseWindowWrapper<ShopWindow> {
    private List<ShopItemView> itemProxyList = new List<ShopItemView>();
    private GameObject itemPrefab;
    private Transform transListParent;
    private Button btnClose;

    protected override void InitCtrl() {
        itemPrefab = gameObject.GetChildControl<RectTransform>("Panel/items/item").gameObject;
        transListParent = gameObject.GetChildControl<Transform>("Panel/items");
        btnClose= gameObject.GetChildControl<Button>("btnClose");
        itemPrefab.SetActive(false);
    }

    protected override void OnPreOpen() {
        RefreshList();
    }

    protected override void OnOpen() {
    }

    protected override void InitMsg() {
        Send.RegisterMsg(SendType.UseItemChange, OnUseItemChange);
        Send.RegisterMsg(SendType.UnLockItemSuccess, OnUnLockItem);
        btnClose.onClick.AddListener(OnCloseClick);
    }

    protected override void ClearMsg() {
        Send.UnregisterMsg(SendType.UseItemChange, OnUseItemChange);
        Send.UnregisterMsg(SendType.UnLockItemSuccess, OnUnLockItem);
        btnClose.onClick.RemoveListener(OnCloseClick);
    }

    private void OnUseItemChange(object[] objs) {
        RefreshList();
    }

    private void OnUnLockItem(object[] objs) {
        RefreshList();
    }

    private void OnCloseClick() {
        WindowMgr.Instance.CloseWindow<ShopWindow>();
    }

    private void RefreshList() {
        int length = ShopMgr.Instance.itemInfoList.Count;
        //支持动态物品列表变化
        for (int index = 0; index < length; index++) {
            ShopItemInfo itemInfo = ShopMgr.Instance.itemInfoList[index];
            ShopItemView itemView = null;
            if (itemProxyList.Count > index) {
                itemView = itemProxyList[index];
            }
            else {
                GameObject itemGo = GameObject.Instantiate(itemPrefab, transListParent, false);
                itemView = new ShopItemView(itemGo);
                itemProxyList.Add(itemView);
            }

            itemView.SetData(itemInfo);
        }

        //隐藏多余的物品
        for (int index = length; index < itemProxyList.Count; index++) {
            itemProxyList[index].ClearData();
        }
    }
}

/// <summary>
/// 单物品显示类
/// </summary>
public class ShopItemView {
    private GameObject itemGo;
    private ShopItemInfo shopItemInfo;
    private Button btnClick;

    public ShopItemView(GameObject _itemGo) {
        itemGo = _itemGo;
        btnClick = itemGo.GetComponent<Button>();
        btnClick.onClick.AddListener(OnClick);
    }

    public void SetData(ShopItemInfo itemInfo) {
        shopItemInfo = itemInfo;
        itemGo.SetActive(true);
        Refresh();
    }

    public void ClearData() {
        shopItemInfo = null;
        itemGo.SetActive(false);
    }

    private void Refresh() {

    }

    private void OnClick() {
        if (shopItemInfo.itemState == ShopItemState.CanUse) {
            ShopMgr.Instance.UseItemId = shopItemInfo.refShop.ItemId;
            return;
        }
    }
}