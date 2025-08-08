using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HWMsgTipWindow : BaseWindowWrapper<HWMsgTipWindow> {

    private Text txtTitle;
    private Animator m_anim;
    private Button btnClose;
    private float curTime;
    private float closeTime = 1.5f;
    private Coroutine coDelay;

    protected override void InitCtrl() {
        txtTitle = gameObject.GetChildControl<Text>("Root/imgBG/txtTitle");
        m_anim = gameObject.GetChildControl<Animator>("Root/imgBG");
        btnClose = gameObject.GetChildControl<Button>("Root/btnClose");
    }

    protected override void OnPreOpen() {
        curTime = 0;
    }

    protected override void OnOpen() {
        m_anim?.Rebind();
        if (coDelay != null) {
            CoDelegator.StopCoroutineEx(coDelay);
        }
    }

    protected override void InitMsg() {
        btnClose.onClick.AddListener(OnCloseClick);
    }

    protected override void ClearMsg() {
        btnClose.onClick.RemoveListener(OnCloseClick);
    }

    private void OnCloseClick() {
        if (coDelay == null) {
            curTime = closeTime;
        }
    }

    private void Update() {
        if (coDelay != null)
            return;
        curTime += Time.deltaTime;
        if (curTime >= closeTime) {
            curTime = 0;
            m_anim?.SetTrigger("Close");
            coDelay = CoDelegator.Coroutine(Delay());
        }
    }

    IEnumerator Delay() {
        yield return new WaitForSeconds(0.5f);
        WindowMgr.Instance.CloseWindow<HWMsgTipWindow>();
        CoDelegator.StopCoroutineEx(coDelay);
        coDelay = null;
    }

    //广告未加载提示接口
    public void ShowNoAdTip() {
        ShowTip("No Ad Ready!");
    }

    //其它信息提示接口
    public void ShowTip(string titleStr, params object[] objs) {
        txtTitle.SetTextFormat(titleStr, objs);
        WindowMgr.Instance.OpenWindow<HWMsgTipWindow>();
    }
}