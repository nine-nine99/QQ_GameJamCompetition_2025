using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalNote : BaseNote
{
    private float headCurSpeed = 0;
    private float headShowTime = 0;

    public float speed = 2.0f;
    private float FaliLine = -4f;//如果玩家没有按下按键，那该音符miss的判定线。随时可以改数值
    // 伤害
    private int Damage = -8;
    private void Start()
    {
        type = SyllableType.Normal;
    }

    public void Init(SyllableDetail detail, float _distance, float _faliLine)
    {
        float _headCurSpeed = _distance / (detail.endTime_1 - detail.showTime_1);
        headCurSpeed = _headCurSpeed;
        headShowTime = detail.showTime_1;

        speed = headCurSpeed;

        FaliLine = _faliLine;
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < FaliLine)
        {
            Debug.Log("音符掉出判定线!");

            ObjectPool.Instance.Recycle(gameObject);
            ComboManager.Instance.ResetCombo();

            // 扣除血量
            Send.SendMsg(SendType.HPChange, Damage);
        }
    }

    public override void OnHit()
    {
        base.OnHit();
        // 停止移动
        speed = 0;
        // 播放特效
        noteEffect.StartEffect(() =>
        // 最后一步回收Object
        ObjectPool.Instance.Recycle(gameObject)
        );



    }

}
