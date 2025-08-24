using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelodyController : MonoBehaviour
{
    public int key = 1;
    private bool playerIsHere = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            key = 1;
            // 处理玩家与音节的交互
            Debug.Log("玩家与音节发生碰撞");
            // 可以在这里添加更多的逻辑，比如播放音效、更新分数等
            BattleMgr.Instance.state = BattleState.canIntoMusicBattle; // 允许进入音乐战斗

            playerIsHere = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 处理玩家与音节的交互结束
            // Debug.Log("玩家与音节结束碰撞");
            // 可以在这里添加更多的逻辑，比如停止音效、更新分数等
            BattleMgr.Instance.state = BattleState.Game; // 禁止进入音乐战斗

            playerIsHere = false;
        }
    }

    private void Update()
    {
        // TODO: 可以优化
        if (Input.GetKeyDown(KeyCode.E) && playerIsHere)
        {
            Debug.Log("玩家按下E键，准备进入音乐战斗");
            Send.SendMsg(SendType.Into_Conversation, key); // 发送消息，准备进入音乐战斗
        }

        // // 检查是否可以进入音乐战斗
        // if (BattleMgr.Instance.state == BattleState.canIntoMusicBattle)
        // {
        //     if (Input.GetKeyDown(KeyCode.E))
        //     {
        //         Debug.Log("进入音乐战斗");

        //         LevelMgr.Instance.StartMusicBattle(); // 开始音乐战斗

        //     }
        // }
    }
}
