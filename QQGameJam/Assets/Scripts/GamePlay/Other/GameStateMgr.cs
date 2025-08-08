using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏状态管理类 -- 这里的状态指大状态 通常需要关闭其他所有界面 打开新界面
/// </summary>
public class GameStateMgr : Singleton<GameStateMgr> {
    public GameState curState = GameState.None; //当前游戏状态
    private GameState preState = GameState.None;//上一个游戏状态

	public void Init(){
		
	}
	
	public void Clear(){
		
	}

    public void SwitchState(GameState state) {
        //check state 
        if (curState == state) {
            Debug.LogError("The Same State, Please Check Code");
        }

        preState = curState;
        curState = state;
        // close all window 
        int enumCount = System.Enum.GetNames(typeof(GameState)).Length;
        for (int index = 0; index < enumCount; index++) {
            WindowMgr.Instance.CloseGroupWindow(index);
        }
        // open window
        switch (curState) {
            case GameState.None:
                //do noting
                break;
            case GameState.Loading:
                WindowMgr.Instance.OpenWindow<LoadingWindow>();
                break;
            case GameState.Main:
                WindowMgr.Instance.OpenWindow<MainWindow>();
                break;
            case GameState.Battle:
                WindowMgr.Instance.OpenWindow<BattleWindow>();
                BattleMgr.Instance.StartBattle();
                break;
            case GameState.GameOver:
                WindowMgr.Instance.OpenWindow<GameOverWindow>();
                break;
            default:
                Debug.LogError("Undefined:" + curState);
                break;
        }
    }
}

public enum GameState {
    None = 0,
    Loading,
    Main,
    Battle,
    GameOver,
}
