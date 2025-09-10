using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNote_PartMove : MonoBehaviour
{
    public float speed = 0f;
    public LongNote mLongNote => transform.parent.GetComponent<LongNote>();
    private float showTime = 0;
    private float currentTime => BGMListener.Instance.GetCurrentTime();

    private void OnEnable()
    {
        // NoteInit();
    }
    private void OnDisable()
    {
        NoteClear();
    }
    private void Update()
    {
        // 到达移动时间后再移动
        if (currentTime < showTime) return;
        // 移动的默认实现
        Movement();
    }

    public void StartMove(float speed)
    {
        this.speed = speed;
    }
    public void StopMove()
    {
        speed = 0;
    }
    private void Movement()
    {
        // if (speed == 0) return;
 
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    public void NoteInit(float _speed, float _showTime)
    {
        speed = _speed;
        showTime = _showTime;
    }
    public void NoteClear()
    {
        speed = 0;
    }
}
