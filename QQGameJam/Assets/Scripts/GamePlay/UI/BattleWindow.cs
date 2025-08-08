using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleWindow : BaseWindowWrapper<BattleWindow>
{
    private Transform scenePanel;
    private Transform battlePanel;
    protected override void InitCtrl()
    {
        scenePanel = gameObject.GetChildControl<Transform>("scenePanel");
        battlePanel = gameObject.GetChildControl<Transform>("battlePanel");
    }

    protected override void OnPreOpen()
    {
        scenePanel.gameObject.SetActive(false);
        battlePanel.gameObject.SetActive(false);
    }

    protected override void OnOpen()
    {
    }

    protected override void OnPreClose()
    {
        base.OnPreClose();
    }

    protected override void OnClose()
    {
        base.OnClose();
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
        battlePanel.gameObject.SetActive(false);
    }
    public void ShowBattlePanel()
    {
        scenePanel.gameObject.SetActive(false);
        battlePanel.gameObject.SetActive(true);
    }
}
