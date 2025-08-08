using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 签到界面
/// </summary>
public class SignWindow : BaseWindowWrapper<SignWindow> {
    private List<SignItemView> signProxyList = new List<SignItemView>();

    private GameObject dayPrefab;
    private Transform transListParent;
    private Button btnClose;

    protected override void InitCtrl() {
        btnClose = gameObject.GetChildControl<Button>("btnClose");
        dayPrefab = gameObject.GetChildControl<RectTransform>("panelDay/dayPrefab").gameObject;
        dayPrefab.SetActive(false);
        transListParent = gameObject.GetChildControl<Transform>("panelDay");
    }

    protected override void OnPreOpen() {
        RefreshList();
    }

    protected override void OnOpen() {
    }

    protected override void InitMsg() {
        btnClose.onClick.AddListener(OnCloseClick);
        Send.RegisterMsg(SendType.SignDayChange, OnSignDayChange);
    }

    protected override void ClearMsg() {
        btnClose.onClick.RemoveListener(OnCloseClick);
        Send.UnregisterMsg(SendType.SignDayChange, OnSignDayChange);
    }

    private void OnSignDayChange(object[] objs) {
        RefreshList();
    }

    private void RefreshList() {
        int length = SignMgr.Instance.signInfoList.Count;
        //支持动态物品列表变化
        for (int index = 0; index < length; index++) {
            SignInfo signInfo = SignMgr.Instance.signInfoList[index];
            SignItemView signView = null;
            if (signProxyList.Count > index) {
                signView = signProxyList[index];
            }
            else {
                GameObject itemGo = GameObject.Instantiate(dayPrefab, transListParent, false);
                signView = new SignItemView(itemGo);
                signProxyList.Add(signView);
            }

            signView.SetData(signInfo);
        }

        //隐藏多余的物品
        for (int index = length; index < signProxyList.Count; index++) {
            signProxyList[index].ClearData();
        }
    }

    private void OnCloseClick() {
        WindowMgr.Instance.CloseWindow<SignWindow>();
    }
}

public class SignItemView {
    private GameObject signGo;
    private SignInfo signInfo;
    private Image imgGet;
    private Button btnCanGet;
    private Button btnLock;
    private Text txtDay;
    private Text txtReward;

    public SignItemView(GameObject _signGo) {
        signGo = _signGo;
        btnCanGet = signGo.GetChildControl<Button>("btnCanGet");
        btnLock = signGo.GetChildControl<Button>("btnLock");
        imgGet = signGo.GetChildControl<Image>("imgGet");
        txtDay = signGo.GetChildControl<Text>("txtDay");
        txtReward = signGo.GetChildControl<Text>("txtReward");

        btnCanGet.onClick.AddListener(OnGetClick);
        btnLock.onClick.AddListener(OnLockClick);
    }

    public void SetData(SignInfo _signInfo) {
        signInfo = _signInfo;
        signGo.SetActive(true);
        Refresh();
    }

    public void ClearData() {
        signInfo = null;
        signGo.SetActive(false);
    }

    private void Refresh() {
        btnCanGet.gameObject.SetActive(signInfo.signState == SignState.CanSign);
        btnLock.gameObject.SetActive(signInfo.signState == SignState.SignLock);
        imgGet.gameObject.SetActive(signInfo.signState == SignState.Signed);
        //如果奖励不止一种 需要做ICON的变化
        txtReward.text = signInfo.refSign.RewardNum.ToString();
        txtDay.text = "Day " + signInfo.refSign.Day;
    }

    private void OnGetClick() {
        SignMgr.Instance.TrySign(signInfo.refSign.Day);
        Refresh();
    }

    private void OnLockClick() {
        //show tip if need
    }
}