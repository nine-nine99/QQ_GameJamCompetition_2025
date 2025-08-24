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
        // Debug.Log(curLevel + " " + newLevel);
        // 加载新关卡
        if (newLevel != curLevel)
        {
            LoadLevel(newLevel);
        }
    }

    private void LoadLevel(int levelKey)
    {
        // 需要修正
        // 加载新关卡
        Debug.Log("Loading Level: " + levelKey);
        if (curLevelObj != null)
        {
            ObjectPool.Instance.Recycle(curLevelObj.gameObject); // 销毁当前关卡对象
        }

        GameObject levelInstance = GetLevelPrefab(levelKey);
        levelInstance.name = "Level_" + levelKey;

        curLevelObj = levelInstance.GetComponent<Level>();
        curLevelObj.Init();
    }
    /// <summary>
    /// 通过key 获取关卡预制体
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public GameObject GetLevelPrefab(int key)
    {
        string pathName = "Level_" + key;
        return ObjectPool.Instance.Get("Level", pathName);
    }
}


