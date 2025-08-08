using UnityEngine;
using System.Collections;
using System;
using System.ComponentModel;

/// <summary>
/// 窗口根节点
/// </summary>
public class UIRootTwoD : SingletonMonoBehavior<UIRootTwoD> {

    [Header("最大帧率")]
    public int MaxRate = 60;

    private Canvas normalCanvas;
    private Canvas modalCanvas;
    private Canvas tipCanvas;
    private Canvas systemCanvas;

    protected override void Awake() {
        base.Awake();
        InitCanvas();
    }

    private void InitCanvas() {
        //设置帧率为屏幕刷新率,编辑器不生效
#if !UNITY_EDITOR
        int refreshRate = Screen.currentResolution.refreshRate;
        refreshRate = Math.Min(MaxRate, refreshRate);
        Application.targetFrameRate = refreshRate;
#endif

        DontDestroyOnLoad(gameObject);
        if (normalCanvas == null) {
            normalCanvas = gameObject.GetChildControl<Canvas>("NormalPanel");
            modalCanvas = gameObject.GetChildControl<Canvas>("ModalPanel");
            tipCanvas = gameObject.GetChildControl<Canvas>("TipsPanel");
            systemCanvas = gameObject.GetChildControl<Canvas>("SystemPanel");
        }
    }

    public void SortWindow(Transform winTransform, WindowType winType) {
        InitCanvas();
        switch (winType) {
            case WindowType.Normal:
                winTransform.SetParent(normalCanvas.transform, false);
                break;
            case WindowType.Modal:
                winTransform.SetParent(modalCanvas.transform, false);
                break;
            case WindowType.Tips:
                winTransform.SetParent(tipCanvas.transform, false);
                break;
            case WindowType.System:
                winTransform.SetParent(systemCanvas.transform, false);
                break;
            default:
                break;
        }
    }
}
