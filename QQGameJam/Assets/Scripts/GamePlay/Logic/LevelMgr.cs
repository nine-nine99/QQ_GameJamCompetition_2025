using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelMgr : Singleton<LevelMgr>
{
    // TODO:这里未来会优化数据获取的方式
    private string levelSencePrefabPath = "Prefab/LevelScene/LevelScene0";
    private string realWorldScene = "Prefab/LevelScene/RealWorldScene";
    private int currentLevel;
    public int CurrentLevel
    {
        get { return currentLevel; }
        set
        {
            Send.SendMsg(SendType.LevelChange, currentLevel, value);
            currentLevel = value;
        }
    }

    public GameObject curLevelObj;

    public void Init()
    {
        currentLevel = -1; // 默认从-1开始
        InitMsg();
    }
    public void Clear()
    {
        // 清理当前关卡数据
        currentLevel = -1;
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
            UnityEngine.Object.Destroy(curLevelObj); // 销毁当前关卡对象
        }

        // 这里可以添加加载关卡的具体逻辑，比如从资源中加载关卡数据等
        GameObject levelPrefab = Resources.Load<GameObject>(levelSencePrefabPath);
        GameObject realWorldPrefab = Resources.Load<GameObject>(realWorldScene);
        if (realWorldPrefab != null)
        {
            GameObject levelInstance = UnityEngine.Object.Instantiate(realWorldPrefab);
            levelInstance.name = "Level_" + level;
            curLevelObj = levelInstance;
            curLevelObj.transform.GetChild(0).gameObject.SetActive(true); // 激活关卡的第一个子对象
            // curLevelObj.transform.GetChild(1).gameObject.SetActive(false); // 第二个子对象是隐藏的
        }
        else
        {
            Debug.LogError("Failed to load level prefab at path: " + levelSencePrefabPath);
        }
    }

}
