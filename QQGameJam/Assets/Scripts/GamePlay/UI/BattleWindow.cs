using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleWindow : BaseWindowWrapper<BattleWindow>
{
    public SettingControl settingControl;
    public Button btn1;
    public Button btn2;
    public Button btn3;
    public Button btn_return1;
    public Button btn_return2;
    public Transform Menu_1;
    public Transform Menu_2;
    private Transform scenePanel;
    protected override void InitCtrl()
    {
        scenePanel = gameObject.GetChildControl<Transform>("InScenePanel");
    }

    protected override void OnPreOpen()
    {
        scenePanel.gameObject.SetActive(true);
    }

    protected override void OnOpen()
    {
        btn1.onClick.AddListener(OnBtn_1);
        btn2.onClick.AddListener(OnBtn_2);
        btn3.onClick.AddListener(OnBtn_3);

        btn_return1.onClick.AddListener(OnBtnReturn);
        btn_return2.onClick.AddListener(OnBtnReturn);
    }

    protected override void OnPreClose()
    {
        base.OnPreClose();
    }

    protected override void OnClose()
    {
        base.OnClose();
        btn1.onClick.RemoveListener(OnBtn_1);
        btn2.onClick.RemoveListener(OnBtn_2);
        btn3.onClick.RemoveListener(OnBtn_3);

        btn_return1.onClick.RemoveListener(OnBtnReturn);
        btn_return2.onClick.RemoveListener(OnBtnReturn);
    }

    protected override void InitMsg()
    {
    }

    protected override void ClearMsg()
    {
    }

    public void ShowScenePanel()
    {
        scenePanel.gameObject.SetActive(true);
    }


    // 退出游戏按钮
    private void OnBtn_3()
    {
        Application.Quit();
    }

    // 设置按钮
    private void OnBtn_2()
    {
        Menu_1.gameObject.SetActive(true);
        Menu_2.gameObject.SetActive(true);
    }

    // 返回按钮
    private void OnBtn_1()
    {
        settingControl.OnOutImageClicked();
    }


    private void OnBtnReturn()
    {
        // 同时关闭Menu_1和Menu_2
        Menu_1.gameObject.SetActive(false);
        Menu_2.gameObject.SetActive(false);

        settingControl.OnOutImageClicked();
    }
}
