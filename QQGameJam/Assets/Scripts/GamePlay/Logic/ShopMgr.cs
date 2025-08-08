using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 商店管理
/// </summary>
public class ShopMgr : Singleton<ShopMgr> {
    private const string USE_ITEM_ID = "UseItemID";
    public List<ShopItemInfo> itemInfoList = new List<ShopItemInfo>();

    public int UseItemId {
        get {
            return LocalSave.GetInt(USE_ITEM_ID, 0);
        }
        set {
            LocalSave.SetInt(USE_ITEM_ID, value);
            Send.SendMsg(SendType.UseItemChange, value);
        }
    }

	public void Init(){
        InitList();
        Send.RegisterMsg(SendType.TryUnLockItem, OnUnLockItem);
        Send.RegisterMsg(SendType.TaskComplete, OnTaskComplete);
	}
	
	public void Clear(){
        Send.UnregisterMsg(SendType.TryUnLockItem, OnUnLockItem);
        Send.UnregisterMsg(SendType.TaskComplete, OnTaskComplete);
	}

    private void InitList() {
        itemInfoList.Clear();
        foreach (RefShop refshop in RefShop.cacheMap.Values) {
            itemInfoList.Add(new ShopItemInfo(refshop));
        }
    }

    private void OnUnLockItem(object[] objs) {
        int itemId = (int)objs[0];
        TryUnLockItem(itemId);
    }

    public void TryUnLockItem(int itemId) {
        ShopItemInfo itemInfo = GetItemInfo(itemId);
        if (itemInfo == null) {
            Debug.LogError("iteminfo is null:" + itemId);
            return;
        }
        if (itemInfo.CanUnLock()) {
            itemInfo.UnLock();
        }
        else {
            //show tip if need
            Debug.LogError("Condition is Fail:" + itemId);
        }
    }

    public ShopItemInfo GetItemInfo(int itemId) {
        for (int index = 0; index < itemInfoList.Count; index++) {
            ShopItemInfo shopItemInfo = itemInfoList[index];
            if (shopItemInfo.refShop.ItemId == itemId) {
                return shopItemInfo;
            }
        }
        return null;
    }

    private void OnTaskComplete(object[] objs) {
        int taskId = (int)objs[0];
        for (int index = 0; index < itemInfoList.Count; index++) {
            ShopItemInfo shopItemInfo = itemInfoList[index];
            if (shopItemInfo.refShop.UnLockType == UnLockType.Task && shopItemInfo.refShop.Param == taskId) {
                shopItemInfo.UnLock();
            }
        }
    }

    public bool HasNewItem() {
        for (int index = 0; index < itemInfoList.Count; index++) {
            ShopItemInfo shopItemInfo = itemInfoList[index];
            if (shopItemInfo.HasNewItem()) {
                return true;
            }
        }

        return false;
    }
}

/// <summary>
/// 物品信息
/// </summary>
public class ShopItemInfo {
    private const string UNLOCK_KEY = "UnLockItemId";
    private const string NEW_ITEM_KEY = "NewItemId";
    public RefShop refShop;
    public ShopItemState itemState;

    public ShopItemInfo(RefShop _refShop) {
        refShop = _refShop;
        Refresh();
    }

    public void Refresh() {
        itemState = LocalSave.GetBool(UNLOCK_KEY + refShop.ItemId) ? ShopItemState.CanUse : ShopItemState.Lock;
        itemState = ShopMgr.Instance.UseItemId == refShop.ItemId ? ShopItemState.InUse : itemState;
    }

    public void UnLock() {
        LocalSave.SetBool(UNLOCK_KEY + refShop.ItemId, true);
        Refresh();
        Send.SendMsg(SendType.UnLockItemSuccess, itemState);
    }

    /// <summary>
    /// 是否是新物品
    /// </summary>
    /// <returns></returns>
    public bool HasNewItem() {
        if (itemState != ShopItemState.CanUse)
            return false;
        return LocalSave.GetBool(NEW_ITEM_KEY + refShop.ItemId, true);
    }

    /// <summary>
    /// 发现物品
    /// </summary>
    public void FindItem() {
        LocalSave.GetBool(NEW_ITEM_KEY + refShop.ItemId, false);
    }

    ///// <summary>
    ///// 获取描述 若需求变更 对应枚举自行增删改 
    ///// </summary>
    ///// <returns></returns>
    public string GetDesc() {
        string desc = "";
        switch (refShop.UnLockType) {
            case UnLockType.None:
                desc = refShop.Desc;
                break;
            case UnLockType.Gold:
                desc = string.Format(refShop.Desc, refShop.Param);
                break;
            case UnLockType.Task:
                RefTask refTask = RefTask.GetRef(refShop.Param);
                desc = string.Format(refShop.Desc, refTask.Condition);
                break;
            default:
                Debug.LogError("not define type:" + refShop.UnLockType);
                break;
        }

        return desc;
    }

    ///// <summary>
    ///// 获取进度 若需求变更 对应枚举自行增删改
    ///// </summary>
    ///// <returns></returns>
    public string GetProgress() {
        string progress = "";
        bool success = CanUnLock();

        switch (refShop.UnLockType) {
            case UnLockType.None:
                progress = "";
                break;
            case UnLockType.Gold:
                progress = string.Format(success ? "<color=#00FF00>{0}</color>/{1}" : "<color=#FF0000>{0}</color>/{1}", CurrencyMgr.Instance.Gold, refShop.Param);
                break;
            case UnLockType.Task:
                TaskInfo taskInfo = TaskMgr.Instance.GetTaskInfo(refShop.Param);
                if (taskInfo == null) {
                    Debug.LogError("taskinfo is null:" + refShop.Param);
                }
                else {
                    progress = string.Format(success ? "<color=#00FF00>{0}</color>/{1}" : "<color=#FF0000>{0}</color>/{1}", taskInfo.CurValue, taskInfo.refTask.Condition);
                }
                break;
            default:
                Debug.LogError("not define type:" + refShop.UnLockType);
                break;
        }

        return progress;
    }

    /// <summary>
    /// 尝试解锁 若需求变更 对应枚举自行增删改
    /// </summary>
    /// <returns></returns>
    public bool CanUnLock() {
        bool can = false;
        switch (refShop.UnLockType) {
            case UnLockType.None:
                can = true;
                break;
            case UnLockType.Gold:
                can = CurrencyMgr.Instance.Gold >= refShop.Param;
                break;
            case UnLockType.Task:
                can = TaskMgr.Instance.TaskHasComplete(refShop.Param);
                break;
            default:
                Debug.LogError("not define type:" + refShop.UnLockType);
                break;
        }

        return can;
    }

    /// <summary>
    /// 尝试解锁 若需求变更 对应枚举自行增删改
    /// </summary>
    /// <returns></returns>
    public UnLockItemResult TryUnLockItem() {
        if (itemState != ShopItemState.Lock)
            return UnLockItemResult.Unlocked;

        if(CanUnLock() == true){
            Send.SendMsg(SendType.TryUnLockItem, refShop.ItemId);
            return UnLockItemResult.Success;
        }
        else{
            return UnLockItemResult.Fail;
        }
    }
}