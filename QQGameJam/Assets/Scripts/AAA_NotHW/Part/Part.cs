using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour
{
    // 玩家生成点
    public Transform PlayerSpawnTransform;
    // 敌人生成点
    public Transform EnemySpawnTransform;
    public GameObject curPlayer;
    public List<GameObject> curEnemys;
    public GameObject GenPlayer(Transform Tran)
    {
        GameObject player = ObjectPool.Instance.Get("Character", "MainCharacter", Tran);
        player.transform.position = Tran.position;
        return this.curPlayer = player;
    }

    public void GenEnemy(Transform Tran)
    {
        GameObject curEnemy = ObjectPool.Instance.Get("Character", "MelodyCharacter", Tran);
        curEnemy.transform.position = Tran.position;
        curEnemys.Add(curEnemy);
    }
}
