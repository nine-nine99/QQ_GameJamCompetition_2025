using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 加载界面，不一定会有
/// </summary>
public class LoadingWindow : BaseWindowWrapper<LoadingWindow> {
    //to do progress

    protected override void InitCtrl() {
    }

    protected override void OnPreOpen() {
    }

    protected override void OnOpen() {
    }

    protected override void InitMsg() {
        Send.RegisterMsg(SendType.LoadingProgress, OnLoadingProgress);
    }

    protected override void ClearMsg() {
        Send.UnregisterMsg(SendType.LoadingProgress, OnLoadingProgress);
    }

    private void OnLoadingProgress(object[] objs) {
        float progress = (float)objs[0];
    }
}
