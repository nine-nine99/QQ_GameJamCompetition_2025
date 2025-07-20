using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMover : MonoBehaviour
{
    public float speed = 2.0f;
    public float underJudgementLine = -4f;//如果玩家没有按下按键，那该音符miss的判定线。随时可以改数值
    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < underJudgementLine)
        {
            Debug.Log("音符掉出判定线!");
            Destroy(gameObject);
        }
    }
}
