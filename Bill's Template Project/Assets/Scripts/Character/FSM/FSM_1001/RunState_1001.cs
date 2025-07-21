using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState_1001 : IState
{
    private FSM_1001 fsm;
    private float Speed; // 运行速度
    private Rigidbody2D rb;
    private Vector2 direction => fsm.inputDirection.normalized;


    public RunState_1001(FSM_1001 fsm)
    {
        this.fsm = fsm;
        this.rb = fsm.GetComponent<Rigidbody2D>();
    }

    public void OnEnter()
    {
        // Logic for entering the run state
        Debug.Log("Entering Run State");
        this.Speed = 1.2f; // 初始运行速度

    }

    public void OnUpdate()
    {
        // 在Run状态下，检查输入方向
        if (direction == Vector2.zero)
        {
            // 如果没有输入方向，则切换回Idle状态
            fsm.ChangeState(State.Idle);
            return;
        }
        else
        {
            // 更新刚体的速度
            rb.velocity = direction * Speed;
            
            fsm.PlayWalkBob(); // 播放行走动画
        }
    }

    public void OnExit()
    {
        // Logic for exiting the run state
        Debug.Log("Exiting Run State");
        rb.velocity = Vector2.zero;
    }
}
