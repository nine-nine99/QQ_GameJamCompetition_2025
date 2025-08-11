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
        Send.SendMsg(SendType.BattleStart);
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
                // BattleMgr.Instance.state = BattleState.MusicBattle; // 更新战斗状态
                // BGMController.Instance.StartBGM();
                BattleMgr.Instance.StartMusicBattle(); // 开始音乐战斗
            }
        }

        // 限制玩家Y轴位置
        // TODO: 玩家的y轴达到maxY以后，就不能往下了。待解决
        if (curPlayerObj != null)
        {
            Vector3 pos = curPlayerObj.transform.position;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            curPlayerObj.transform.position = pos;

            var rb = curPlayerObj.GetComponent<Rigidbody2D>();
            if (rb)
            {
                // Debug.Log("you reach here");
                if (rb.position.y >= maxY && rb.velocity.y > 0f)
                    rb.velocity = new Vector2(rb.velocity.x, 0f);
                if (rb.position.y <= minY && rb.velocity.y < 0f)
                    rb.velocity = new Vector2(rb.velocity.x, 0f);
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
        GameObject playerPrefab = Resources.Load<GameObject>(playerPrefabPath);
        curPlayerObj = UnityEngine.Object.Instantiate(playerPrefab);
        curPlayerObj.transform.position = position;

        if (curPlayerObj == null)
        {
            Debug.LogError("Failed to load player prefab.");
            return;
        }
        Debug.Log("Player prefab initialized at position: " + position);
    }

    // 清除玩家预制体
    public void ClearPlayerPrefab()
    {
        if (curPlayerObj != null)
        {
            UnityEngine.Object.Destroy(curPlayerObj);
            curPlayerObj = null;
            Debug.Log("Player prefab cleared.");
        }
        else
        {
            Debug.LogWarning("No player prefab to clear.");
        }
    }
}
