using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Level : MonoBehaviour
{
    // 敌人生成点
    public Transform EnemySpawnTransform;

    public GameObject curCharacter;

    private void OnDisable()
    {
        ObjectPool.Instance.Recycle(curCharacter);
    }
    public void OnInit()
    {
        GenCharater();
    }

    public void GenCharater()
    {
        curCharacter = ObjectPool.Instance.Get("Character", "MelodyCharacter");
        curCharacter.transform.position = EnemySpawnTransform.position;
    }

}
