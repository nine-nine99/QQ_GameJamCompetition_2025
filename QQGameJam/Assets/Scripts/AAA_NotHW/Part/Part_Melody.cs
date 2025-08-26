using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part_Melody : Part, IPart
{
    private void OnDisable()
    {
        OnExit();
    }
    public void OnEnter()
    {
        gameObject.SetActive(true);
        var player = GenPlayer(PlayerSpawnTransform);

        // 禁用玩家移动
        var fsm = player.GetComponent<MainCharacterFSM>();
        if (fsm != null) fsm.enabled = false;

        GenEnemy(EnemySpawnTransform);
    }
    public void OnExit()
    {
        if (curPlayer != null)
        {
            var fsm = curPlayer.GetComponent<MainCharacterFSM>();
            if (fsm != null) fsm.enabled = true;
            ObjectPool.Instance.Recycle(curPlayer);
        }
        foreach (var obj in curEnemys)
            {
                ObjectPool.Instance.Recycle(obj);
            }

        curPlayer = null;
        curEnemys.Clear();
        gameObject.SetActive(false);
    }
}
