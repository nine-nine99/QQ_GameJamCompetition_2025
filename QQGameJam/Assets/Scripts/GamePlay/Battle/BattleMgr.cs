using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

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
    public void OnMenuSlotClick(params object[] objects)
    {
        GameStateMgr.Instance.SwitchState(GameState.Battle);
        // 根据槽索引执行相应的逻辑
        LevelMgr.Instance.CurrentLevel = (int)objects[0]; // 假设槽索引对应关卡，同时会触发关卡生成
    }
    public void StartBattle()
    {
        PlayerMgr.Instance.StartBattle();   // 无用之物

        // DialogueMgr.Instance.OpenDialogue(0);
        state = BattleState.Game;
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