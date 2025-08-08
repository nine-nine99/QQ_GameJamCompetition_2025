using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战场管理类，管理战场的进行逻辑
/// </summary>
public class BattleMgr : Singleton<BattleMgr>
{
    public BattleState state = BattleState.Wait;

    public void Init()
    {
        InitMsg();
    }

    public void Clear()
    {
        ClearMsg();
    }

    public void InitMsg()
    {
        Send.RegisterMsg(SendType.MenuSlotClick, OnMenuSlotClick);
    }

    public void ClearMsg()
    {
        Send.UnregisterMsg(SendType.MenuSlotClick, OnMenuSlotClick);
    }

    public void StartBattle()
    {
        PlayerMgr.Instance.StartBattle();
        state = BattleState.Game;
    }

    public void StartMusicBattle()
    {
        // PlayerMgr.Instance.StartMusicBattle();
        state = BattleState.MusicBattle;

        PlayerMgr.Instance.ClearPlayerPrefab(); // 清除玩家预制体
        LevelMgr.Instance.curLevelObj.transform.GetChild(0).gameObject.SetActive(false); // 隐藏当前关卡的场景
        LevelMgr.Instance.curLevelObj.transform.GetChild(1).gameObject.SetActive(true); // 显示音乐战斗场景
        BattleWindow.Instance.ShowBattlePanel(); // 显示战斗面板
        BGMController.Instance.StartBGM(); // 开始背景音乐
        Debug.Log("音乐战斗开始");
    }

    private void OnMenuSlotClick(params object[] data)
    {
        Debug.Log("BattleMgr OnMenuSlotClick: " + data.Length);
        // 处理菜单槽点击事件
        int slotIndex = (int)data[0];
        // 根据槽索引执行相应的逻辑
        LevelMgr.Instance.CurrentLevel = slotIndex; // 假设槽索引对应关卡

        GameStateMgr.Instance.SwitchState(GameState.Battle);
    }
}

public enum BattleState
{
    Wait,
    Game,
    canIntoMusicBattle,
    MusicBattle,
    End,
    Pause,
    WaitRevive,
    GameOver,
}