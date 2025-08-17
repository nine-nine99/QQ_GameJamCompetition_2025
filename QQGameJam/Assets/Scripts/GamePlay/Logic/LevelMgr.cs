using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;

public class LevelMgr : Singleton<LevelMgr>
{
    // TODO:这里未来会优化数据获取的方式
    public PlayableDirector introDirector;
    private string levelSencePrefabPath = "Prefab/LevelScene/LevelScene0";
    private string realWorldScene = "Prefab/LevelScene/RealWorldScene_0";
    private string introTimelinePrefabPath = "Prefab/Timeline/IntroTimeline";
    private int _currentLevel;
    public int CurrentLevel
    {
        get { return _currentLevel; }
        set
        {
            Send.SendMsg(SendType.LevelChange, _currentLevel, value);
            _currentLevel = value;
        }
    }

    // 当前的关卡obj
    public Level curLevelObj;
    // 当前的音乐战斗obj
    public GameObject curMusicBattleObj;

    public void Init()
    {
        _currentLevel = -1; // 默认从-1开始
        InitMsg();
    }
    public void Clear()
    {
        // 清理当前关卡数据
        _currentLevel = -1;
        ClearMsg();
    }

    private void InitMsg()
    {
        // 初始化消息
        Send.RegisterMsg(SendType.LevelChange, OnLevelChange);
        Send.RegisterMsg(SendType.MusicBattleEnd, OnMusicBattleEnd);
    }
    private void ClearMsg()
    {
        // 清理消息
        Send.UnregisterMsg(SendType.LevelChange, OnLevelChange);
    }
    private void OnLevelChange(params object[] data)
    {
        // 处理关卡变更逻辑
        int curLevel = (int)data[0];
        int newLevel = (int)data[1];
        // 加载新关卡
        if (newLevel != curLevel)
        {
            LoadLevel(newLevel);
        }
    }

    private void LoadLevel(int level)
    {
        // 加载新关卡
        Debug.Log("Loading Level: " + level);
        if (curLevelObj != null)
        {
            ObjectPool.Instance.Recycle(curLevelObj.gameObject); // 销毁当前关卡对象
        }

        GameObject levelInstance = ObjectPool.Instance.Get("LevelScene", "RealWorldScene_0");
        levelInstance.name = "Level_" + level;
        curLevelObj = levelInstance.GetComponent<Level>();
        curLevelObj.OnInit();
        curLevelObj.transform.GetChild(0).gameObject.SetActive(true); // 激活关卡的第一个子对象

        // curLevelObj.transform.GetChild(1).gameObject.SetActive(false); // 第二个子对象是隐藏的

    }

    public void OnMusicBattleEnd(params object[] objects)
    {
        EndMusicBattle();
    }

    /// <summary>
    /// 音游战斗开始
    /// </summary>
    public void StartMusicBattle()
    {
        BattleMgr.Instance.state = BattleState.MusicBattle;

        curMusicBattleObj = ObjectPool.Instance.Get("MusicScenePrafab");

        BattleWindow.Instance.ShowBattlePanel(); // 显示战斗面板
        BGMController.Instance.StartBGM(); // 开始背景音乐
        Debug.Log("音乐战斗开始");
    }

    public void EndMusicBattle()
    {
        BattleMgr.Instance.state = BattleState.Game;

        ObjectPool.Instance.Recycle(curMusicBattleObj);
        BattleWindow.Instance.ShowScenePanel();
        PlayerMgr.Instance.PlacePlayer(new Vector2(0, -1.5f));
    }
}


