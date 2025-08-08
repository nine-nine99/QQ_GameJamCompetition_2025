using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 窗口管理
/// </summary>
public class WindowMgr : Singleton<WindowMgr> {

    public Dictionary<string, BaseWindow> allList = new Dictionary<string, BaseWindow>();
    public List<string> openList = new List<string>();
    private List<MaskItemView> maskItemViewList = new List<MaskItemView>();

    public void Init() {

    }

    public void Clear() {
        allList.Clear();
        openList.Clear();
        maskItemViewList.Clear();
    }

    public void OpenWindow<T>() where T : BaseWindow {
        string winName = typeof(T).Name;
        BaseWindow window = GetWindow(winName);
        if (window == null) {
            Debug.LogError("open window fail window is null " + winName);
            return;
        }
        if (window.hasOpen)
            return;

        if (window.windowInfo.group != 0) {
            CloseGroupWindow(window.windowInfo.group);
        }

        openList.Add(winName);
        window.transform.SetAsLastSibling();
        window.DoOpen();
    }

    public void CloseGroupWindow(int group) {
        List<string> tempList = new List<string>(openList);
        for (int index = 0; index < tempList.Count; index++) {
            string name = tempList[index];
            if (allList[name].windowInfo.group == group) {
                allList[name].CloseWindow();
            }
        }
    }

    public void CloseWindow<T>() {
        string winName = typeof(T).Name;
        CloseWindow(winName);
    }

    public void CloseWindow(string winName) {
        BaseWindow window = GetWindow(winName);
        if (window == null) {
            Debug.LogError("open window fail window is null " + winName);
            return;
        }

        if (!window.hasOpen)
            return;

        openList.Remove(winName);
        window.DoClose();
    }

    public BaseWindow GetWindow(string name) {
        BaseWindow window = null;
        if (allList.ContainsKey(name)) {
            window = allList[name];
        }
        else {
            window = InstantiateWin(name);
            allList.Add(name, window);
        }

        return window;
    }

    private BaseWindow InstantiateWin(string winName) {
        BaseWindow window = null;
        GameObject winPrefab = LocalAssetMgr.Instance.Load_UI(winName);
        if (winPrefab == null) {
            Debug.LogError(string.Format("无法获得窗口{0}资源！", winName));
            return window;
        }
        GameObject winGo = GameObject.Instantiate(winPrefab) as GameObject;
        winGo.name = "Win_" + winName;
        winGo.SetActive(false);
        window = winGo.GetComponent<BaseWindow>();
        if (window == null) {
            Debug.LogError("预制上没有挂对应的脚本:" + winName);
        }
        WinSetParent(window);
        return window;
    }

    private void WinSetParent(BaseWindow window) {
        UIRootTwoD.Instance.SortWindow(window.transform, window.windowInfo.windowType);
    }

    public MaskItemView GetMaskView(Transform winTR) {
        MaskItemView itemView = null;
        foreach (var item in maskItemViewList.Where(x => x.State == UIMaskState.None)) {
            itemView = item;
            break;
        }
        if (itemView == null) {
            GameObject itemGO = GameObject.Instantiate(LocalAssetMgr.Instance.Load_UIPrefab("WindowMask")) as GameObject;
            itemView = new MaskItemView(itemGO);
            maskItemViewList.Add(itemView);
        }
        itemView.SetData(winTR);
        return itemView;
    }
}

public class MaskItemView {

    public GameObject ItemGO { get; }
    public UIMaskState State { get; private set; } = UIMaskState.None;

    public MaskItemView(GameObject _itemGO) {
        ItemGO = _itemGO;
    }

    public void SetData(Transform winTR) {
        ItemGO.layer = winTR.gameObject.layer;
        ItemGO.name = "Mask";
        ItemGO.transform.SetParent(winTR.parent, false);
        int curIndex = winTR.GetSiblingIndex();
        ItemGO.transform.SetSiblingIndex(curIndex);
        winTR.SetSiblingIndex(curIndex + 1);
        State = UIMaskState.Use;
        ItemGO.SetActive(true);
    }

    public void ClearData() {
        State = UIMaskState.None;
        ItemGO.SetActive(false);
    }
}

public enum UIMaskState {
    None,
    Use,
}