using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainWindow : BaseWindowWrapper<MainWindow>
{
    private Transform fadePanel;
    private Transform openPanel;
    public Button btnStart;
    public int ID_Dialogue;  // 开始对话SO的ID = 1000
    protected override void InitCtrl()
    {
        fadePanel = gameObject.GetChildControl<Transform>("fadePanel");
        openPanel = gameObject.GetChildControl<Transform>("openPanel");
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
        // 为按钮绑定点击
        btnStart.onClick.AddListener(OnBtnStart);
    }

    protected override void ClearMsg()
    {
        btnStart.onClick.RemoveListener(OnBtnStart);
    }

    private void OnBtnStart()
    {
        // BAN:弃用
        // Send.SendMsg(SendType.Into_Conversation, 0);
        // 开启对话框
        // 提供方法，当对话结束时触发关卡生成事件
        DialogueCtrl.Instance.OpenDialogueScene(ID_Dialogue, () => Send.SendMsg(SendType.MenuSlotClick, 1));
    }

    private IEnumerator StartPanelAnimation()
    {
        CanvasGroup canvasGroup = fadePanel.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;

        fadePanel.gameObject.SetActive(true);
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

        fadePanel.gameObject.SetActive(false);
    }
}

