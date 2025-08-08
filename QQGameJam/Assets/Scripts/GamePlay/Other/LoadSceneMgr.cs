using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneMgr : Singleton<LoadSceneMgr> {

	public void Init(){
		
	}
	
	public void Clear(){
		
	}

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="场景名"></param>
    /// <param name="下一个游戏状态"></param>
    /// <param name="是否同步"></param>
    public void LoadScene(string sceneName, GameState nextState, bool syn = false) {
        GameStateMgr.Instance.SwitchState(GameState.Loading);
        if (syn) {
            LoadSceneSyn(sceneName, nextState);
        }
        else {
            CoDelegator.Coroutine( LoadSceneAsyn(sceneName, nextState) );
        }
    }

    private void LoadSceneSyn(string sceneName, GameState nextState) {
        SceneManager.LoadScene(sceneName);
        GameStateMgr.Instance.SwitchState(nextState);

    }

    private IEnumerator LoadSceneAsyn(string sceneName, GameState nextState) {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        while(asyncOperation.progress < 1){
            Send.SendMsg(SendType.LoadingProgress, asyncOperation.progress);
            yield return null;
        }
        GameStateMgr.Instance.SwitchState(nextState);
    }
}
