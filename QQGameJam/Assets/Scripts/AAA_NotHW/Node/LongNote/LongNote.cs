using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNote : BaseNote
{
    [Header("音符_头部")]
    public LongNote_PartMove headNote; // 音符头部
    public float headCurSpeed = 0;
    private float headShowTime = 0;
    [Header("音符_尾部")]
    public LongNote_PartMove tailNote; // 音符尾部
    private float tailCurSpeed = 0;
    private float tailShowTime = 0;
    private float tailEndTime = 0;

    public Transform noteBody; // 音符连接线/身体
    public LineRenderer bodyLineRenderer; // 身体线条渲染器

    private float FaliLine = -4f;
    private int Damage = -8;
    private float currentTime => BGMListener.Instance.GetCurrentTime();

    private void Start()
    {
        type = SyllableType.Hold;
    }
    private void OnEnable()
    {

    }
    void OnDisable()
    {
        Clear();
    }
    private void Update()
    {
        // 不断更新线条的LineRenderer
        UpdateLineRenderer();

        if (tailNote.transform.position.y < FaliLine)
        {
            Debug.Log("音符掉出判定线!");

            ObjectPool.Instance.Recycle(gameObject);
            ComboManager.Instance.ResetCombo();

            // 扣除血量
            Send.SendMsg(SendType.HPChange, Damage);
        }
    }

    public void Init(SyllableDetail detail, float _distance, float _faliLine)
    {
        float _headCurSpeed = _distance / (detail.endTime_1 - detail.showTime_1);
        headCurSpeed = _headCurSpeed;
        headShowTime = detail.showTime_1;

        float _tailCurSpeed = _distance / (detail.endTime_2 - detail.showTime_2);
        tailCurSpeed = _tailCurSpeed;
        tailShowTime = detail.showTime_2;
        tailEndTime = detail.endTime_2;

        headNote.transform.position = transform.position;
        headNote.NoteInit(headCurSpeed, headShowTime);

        tailNote.transform.position = transform.position;
        tailNote.NoteInit(tailCurSpeed, tailShowTime);

        FaliLine = _faliLine;

    }
    private void Clear()
    {
        headCurSpeed = 0;
        tailCurSpeed = 0;
    }

    public override void OnHit()
    {
        base.OnHit();
        // 头部音节停止移动
        headNote.StopMove();
        // 播放特效
        noteEffect.StartEffect(null);
    }

    public void OnHold()
    {
        // 头部音节停止移动
        headNote.StopMove();

        // 时刻检测是否完成
        if (currentTime >= tailEndTime)
        {
            OnFinish();
            return;
        }
        noteEffect.PlayEffect();
    }
    public void OnLose(float _speed)
    {
        // 头部音节重新移动移动
        headNote.StartMove(_speed);
        noteEffect.PlayIdle();
    }

    public void OnFinish()
    {
        // 停止移动
        headNote.StopMove();
        tailNote.StopMove();
        // 播放特效
        noteEffect.PlayIdle();
        // 销毁音节
        ObjectPool.Instance.Recycle(gameObject);
    }

    private void UpdateLineRenderer()
    {
        if (bodyLineRenderer == null || headNote == null || tailNote == null) return;

        // 设置LineRenderer的起点和终点
        Vector3 startPoint = headNote.transform.position;
        Vector3 endPoint = tailNote.transform.position;

        bodyLineRenderer.SetPosition(0, startPoint);
        bodyLineRenderer.SetPosition(1, endPoint);
    }
}
