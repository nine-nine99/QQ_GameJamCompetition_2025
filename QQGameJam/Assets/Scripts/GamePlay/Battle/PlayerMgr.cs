using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家控制器 
/// </summary>
public class PlayerMgr : Singleton<PlayerMgr>
{
    // 玩家预制体路径 Path：Prefab/Character/MainCharacter
    private string playerPrefabPath = "Prefab/Character/MainCharacter";
    public GameObject curPlayerObj;

    //玩家在real world y轴的走动范围
    public float minY = -1.5f;
    public float maxY = -1f;
    //初始化
    public void Init()
    {
        InitMsg();
    }

    //清除数据
    public void Clear()
    {
        ClearMsg();
    }

    //注册消息
    public void InitMsg()
    {

    }

    //反注册消息
    public void ClearMsg()
    {

    }

    //开始游戏时调用，根据需求实现，需要在Battle.StartBattle()中调用
    public void StartBattle()
    {
        PlacePlayer(new Vector2(0, -1.5f)); // 默认位置
        Debug.Log("战斗开始，玩家已放置在默认位置 (0, -1.5)");
    }

    //Update函数，根据需求实现，需要在Launch.Update()中调用
    public void OnUpdate()
    {
        if (BattleMgr.Instance.state == BattleState.canIntoMusicBattle)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("进入音乐战斗");

                ClearPlayerPrefab(); // 清除玩家预制体
                LevelMgr.Instance.StartMusicBattle(); // 开始音乐战斗

            }
        }
    }

    public void PlacePlayer(Vector2 position)
    {
        if (curPlayerObj == null)
        {
            InitPlayerPrefab(position);
        }
        else
        {
            ResetPlayer(position);
        }
        Debug.Log("Player placed at: " + position);
    }

    // 重设玩家状态
    private void ResetPlayer(Vector2 position)
    {
        if (curPlayerObj != null)
        {
            // 重置玩家状态逻辑
            curPlayerObj.transform.position = position;
            curPlayerObj.transform.rotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning("No player object to reset.");
        }
    }

    // 初始化玩家预制体
    private void InitPlayerPrefab(Vector2 position)
    {
        // 这里可以加载玩家预制体
        curPlayerObj = ObjectPool.Instance.Get("Character", "MainCharacter");
        curPlayerObj.transform.position = position;

        if (curPlayerObj == null)
        {
            Debug.LogError("Failed to load player prefab.");
            return;
        }
    }

    // 清除玩家预制体
    public void ClearPlayerPrefab()
    {
        if (curPlayerObj != null)
        {
            ObjectPool.Instance.Recycle(curPlayerObj);
            curPlayerObj = null;
        }
        else
        {
            Debug.LogWarning("No player prefab to clear.");
        }
    }
}
