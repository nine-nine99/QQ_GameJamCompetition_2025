using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelodyController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 处理玩家与音节的交互
            Debug.Log("玩家与音节发生碰撞");
            // 可以在这里添加更多的逻辑，比如播放音效、更新分数等
            BattleMgr.Instance.state = BattleState.canIntoMusicBattle; // 允许进入音乐战斗
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 处理玩家与音节的交互结束
            Debug.Log("玩家与音节结束碰撞");
            // 可以在这里添加更多的逻辑，比如停止音效、更新分数等
            BattleMgr.Instance.state = BattleState.Game; // 禁止进入音乐战斗
        }
    }
}
