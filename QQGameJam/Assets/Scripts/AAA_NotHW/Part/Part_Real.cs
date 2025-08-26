using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part_Real : Part, IPart
{
    private void OnDisable()
    {
        OnExit();
    }
    public void OnEnter()
    {
        gameObject.SetActive(true);
        CameraMgr.Instance.target = GenPlayer(PlayerSpawnTransform).transform;
        // GenEnemy(EnemySpawnTransform);
    }

    public void OnExit()
    {
        if (curPlayer != null)
            ObjectPool.Instance.Recycle(curPlayer);
        foreach (var obj in curEnemys)
        {
            ObjectPool.Instance.Recycle(obj);
        }
        curPlayer = null;
        curEnemys.Clear();
        gameObject.SetActive(false);
    }
}
