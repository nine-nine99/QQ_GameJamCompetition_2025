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
    }

    protected override void OnOpen()
    {

    }

    protected override void InitMsg()
    {
        Send.RegisterMsg(SendType.Over_Conversation, OnConversationOver);
    }

    protected override void ClearMsg()
    {
        Send.UnregisterMsg(SendType.Over_Conversation, OnConversationOver);
    }

    private void OnConversationOver(params object[] data)
    {
        // 初始化菜单
        RefreshList();
        menuPanel.gameObject.SetActive(true);
    }
    private IEnumerator StartPanelAnimation()
    {
        CanvasGroup canvasGroup = StartPanel.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;

        StartPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        // Send.SendMsg(SendType.Into_Conversation, 0); // 进入对话0
        // 缓慢变透明 
        float duration = 1f; // 动画持续时间
        float elapsedTime = 0f;
        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / duration));
            yield return null; // 等待下一帧
        }

        StartPanel.gameObject.SetActive(false);
        RefreshList();
        menuPanel.gameObject.SetActive(true);
    }

    private void RefreshList()
    {
        int length = 8;
        // 支持动态菜单列表变化
        for (int index = 0; index < length; index++)
        {
            MenuSlotView slotView;
            GameObject slotGo = transListParent.GetChild(index).gameObject;
            slotView = new MenuSlotView(slotGo, 1);
            menuSlots.Add(slotView);

            slotView.SetData(1);
        }
    }
}

public class MenuSlotView
{
    private GameObject slotGo;
    private int curSlotIndex;
    private Button btnClick;

    public MenuSlotView(GameObject _slotGo, int _slotIndex)
    {
        slotGo = _slotGo;
        curSlotIndex = _slotIndex;
        btnClick = slotGo.GetComponent<Button>();
        btnClick.onClick.AddListener(OnClick);
    }

    public void SetData(int index)
    {
        curSlotIndex = index;
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
        // Debug.Log(curSlotIndex);
        Send.SendMsg(SendType.MenuSlotClick, curSlotIndex);
        // Debug.Log(curSlotIndex);
        if (curSlotIndex == 1) // 第一关
        {
            Send.SendMsg(SendType.Into_Conversation, 0);
        }
        // // 根据槽索引执行相应的逻辑
        // LevelMgr.Instance.CurrentLevel = curSlotIndex; // 假设槽索引对应关卡

        // GameStateMgr.Instance.SwitchState(GameState.Battle);
    }
}

