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

    }

    public void ClearMsg()
    {

    }

    public void StartBattle()
    {
        PlayerMgr.Instance.StartBattle();
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