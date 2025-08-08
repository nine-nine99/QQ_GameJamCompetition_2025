using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 基础窗口类
/// </summary>
public class BaseWindow : MonoBehaviour {

    private GameObject emptyClose;
    private MaskItemView mask;
    private WindowInfo m_windowInfo = null;

    [HideInInspector]
    public bool hasOpen = false;
    public WindowInfo windowInfo {
        get { return m_windowInfo ?? (m_windowInfo = this.GetComponent<WindowInfo>()); }
    }

    private Tweener tweener = null;

    private CanvasGroup canvasGroup;

    protected void Awake() {
        AddClose();
        InitCtrl();

        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (!canvasGroup) {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    protected void OnDestory() {
        DestroyUI();
    }

    protected virtual void InitCtrl() {

    }

    protected virtual void DestroyUI() {

    }

    public void DoOpen() {
        if (hasOpen)
            return;
        hasOpen = true;

        ResetWindow();
        OnPreOpen();
        InitMsg();
        this.gameObject.SetActive(true);
        PlayOpenAnim();

        AddMask();
    }

    public void DoClose(bool needPlay = true) {
        if (!hasOpen)
            return;
        hasOpen = false;
        ClearMsg();
        OnPreClose();
        if (needPlay) {
            PlayCloseAnim();
        }
        else {
            OnClose();
        }
    }

    protected virtual void OnPreOpen() {

    }

    protected virtual void OnOpen() {

    }

    protected virtual void OnPreClose() {

    }

    protected virtual void OnClose() {
        if (mask != null) {
            mask.ClearData();
            mask = null;
        }
        this.gameObject.SetActive(false);
    }

    protected virtual void InitMsg() {

    }

    protected virtual void ClearMsg() {

    }

    //按钮监听
    protected virtual void InitBtn() {

    }

    private void ResetWindow() {
        if (windowInfo.openAnimType != OpenAnimType.Position) {
            transform.localPosition = Vector3.zero;
        }
        else {
            transform.localPosition = windowInfo.openPos;
        }
        transform.localScale = Vector3.one;
        canvasGroup.alpha = 1;
    }

    private void PlayOpenAnim() {
        tweener?.Kill();
        switch (windowInfo.openAnimType) {
            case OpenAnimType.None:
                OnOpen();
                break;
            case OpenAnimType.Position:
                gameObject.transform.localPosition = windowInfo.defaultPos;
                tweener = gameObject.transform.DOLocalMove(windowInfo.openPos, windowInfo.animTime).SetUpdate(true);
                break;
            case OpenAnimType.Scale:
                gameObject.transform.localScale = Vector3.zero;
                tweener = gameObject.transform.DOScale(1, windowInfo.animTime).SetUpdate(true);
                break;
            case OpenAnimType.Alpha:
                canvasGroup.alpha = 0;
                tweener = canvasGroup.DOFade(1, windowInfo.animTime).SetUpdate(true);
                break;
            case OpenAnimType.ScaleAndAlpha:
                canvasGroup.alpha = 0;
                gameObject.transform.localScale = Vector3.zero;
                tweener = gameObject.transform.DOScale(1, windowInfo.animTime).SetUpdate(true);
                canvasGroup.DOFade(1, windowInfo.animTime).SetUpdate(true);
                break;
            case OpenAnimType.Custom:
                OnOpen();
                Debug.LogError("未实现");
                break;
        }
        tweener?.OnComplete(() => {
            OnOpen();
            tweener = null;
        });
    }

    private void PlayCloseAnim() {
        tweener?.Kill();
        switch (windowInfo.closeAnimType) {
            case OpenAnimType.None:
                OnClose();
                break;
            case OpenAnimType.Position:
                gameObject.transform.localPosition = windowInfo.openPos;
                tweener = gameObject.transform.DOLocalMove(windowInfo.defaultPos, windowInfo.animTime).SetUpdate(true);
                break;
            case OpenAnimType.Scale:
                tweener = gameObject.transform.DOScale(0, windowInfo.animTime).SetUpdate(true);
                break;
            case OpenAnimType.Alpha:
                tweener = gameObject.transform.GetComponent<CanvasGroup>().DOFade(0, windowInfo.animTime).SetUpdate(true);
                break;
            case OpenAnimType.ScaleAndAlpha:
                tweener = gameObject.transform.DOScale(0, windowInfo.animTime).SetUpdate(true);
                canvasGroup.DOFade(0, windowInfo.animTime).SetUpdate(true);
                break;
            case OpenAnimType.Custom:
                OnClose();
                Debug.LogError("未实现");
                break;
        }
        tweener?.OnComplete(() => {
            OnClose();
            tweener = null;
        });
    }

    private void AddMask() {
        if (windowInfo.mask == false)
            return;
        if (mask != null) {
            mask.ClearData();
            mask = null;
        }
        mask = WindowMgr.Instance.GetMaskView(transform);
    }

    private void AddClose() {
        if (windowInfo.closeOnEmpty == false)
            return;

        if (emptyClose == null) {
            emptyClose = Instantiate(LocalAssetMgr.Instance.Load_UIPrefab("WindowClose")) as GameObject;
            emptyClose.layer = this.gameObject.layer;
            emptyClose.name = "EmptyClose";
            emptyClose.transform.SetParent(transform, false);
            emptyClose.transform.SetAsFirstSibling();
            Button button = emptyClose.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(CloseWindow);
        }
    }

    public virtual void CloseWindow() {
        WindowMgr.Instance.CloseWindow(this.GetType().Name);
    }
}