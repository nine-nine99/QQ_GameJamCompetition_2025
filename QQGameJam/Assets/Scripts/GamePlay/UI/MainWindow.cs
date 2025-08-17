using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainWindow : BaseWindowWrapper<MainWindow>
{

    private Button btnShop;
    private Button btnSign;
    private Transform StartPanel;
    private Transform menuPanel;
    private List<MenuSlotView> menuSlots = new List<MenuSlotView>();
    private Transform transListParent;

    protected override void InitCtrl()
    {
        // btnShop = gameObject.GetChildControl<Button>("btnShop");
        // btnSign = gameObject.GetChildControl<Button>("btnSign");
        StartPanel = gameObject.GetChildControl<Transform>("startPanel");
        menuPanel = gameObject.GetChildControl<Transform>("menuPanel");
        transListParent = gameObject.GetChildControl<Transform>("menuPanel/slots");
    }

    protected override void OnPreOpen()
    {
        // 游戏开始动画
        CoDelegator.Coroutine(StartPanelAnimation());

        // 初始化菜单
        RefreshList();
    }

    protected override void OnOpen()
    {

    }

    protected override void InitMsg()
    {
        // btnShop.onClick.AddListener(OnBtnShopClick);
        // btnSign.onClick.AddListener(OnBtnSignClick);
    }

    protected override void ClearMsg()
    {
        // btnShop.onClick.RemoveListener(OnBtnShopClick);
        // btnSign.onClick.RemoveListener(OnBtnSignClick);
    }

    private void OnBtnSignClick()
    {
        WindowMgr.Instance.OpenWindow<SignWindow>();
    }

    private void OnBtnShopClick()
    {
        WindowMgr.Instance.OpenWindow<ShopWindow>();
    }

    private IEnumerator StartPanelAnimation()
    {
        CanvasGroup canvasGroup = StartPanel.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;

        StartPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        // 缓慢变透明 
        float duration = 1f; // 动画持续时间
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / duration));
            yield return null; // 等待下一帧
        }

        StartPanel.gameObject.SetActive(false);
    }

    private void RefreshList()
    {
        int length = 8;
        // 支持动态菜单列表变化
        for (int index = 0; index < length; index++)
        {
            MenuSlotView slotView;
            GameObject slotGo = transListParent.GetChild(index).gameObject;
            slotView = new MenuSlotView(slotGo, index);
            menuSlots.Add(slotView);

            slotView.SetData(index);
        }
    }
}

public class MenuSlotView
{
    private GameObject slotGo;
    private int slotIndex;
    private Button btnClick;

    public MenuSlotView(GameObject _slotGo, int _slotIndex)
    {
        slotGo = _slotGo;
        slotIndex = _slotIndex;
        btnClick = slotGo.GetComponent<Button>();
        btnClick.onClick.AddListener(OnClick);
    }

    public void SetData(int index)
    {
        slotIndex = index;
        slotGo.SetActive(true);
        Refresh();
    }
    public void ClearData()
    {
        slotGo.SetActive(false);
    }
    public void Refresh()
    {
        // 刷新逻辑
    }

    private void OnClick()
    {
        OnMenuSlotClick();
    }
    private void OnMenuSlotClick()
    {
        // 根据槽索引执行相应的逻辑
        LevelMgr.Instance.CurrentLevel = slotIndex; // 假设槽索引对应关卡

        GameStateMgr.Instance.SwitchState(GameState.Battle);
    }
}

