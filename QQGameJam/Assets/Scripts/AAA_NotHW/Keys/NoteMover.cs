using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMover : MonoBehaviour
{
    public float speed = 2.0f;
    public float underJudgementLine = -4f;//如果玩家没有按下按键，那该音符miss的判定线。随时可以改数值

    private int Damage = -8;
    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < underJudgementLine)
        {
            Debug.Log("音符掉出判定线!");

            ObjectPool.Instance.Recycle(gameObject);
            ComboManager.Instance.ResetCombo();

            // 扣除血量
            Send.SendMsg(SendType.HPChange, Damage);
        }
    }
}
