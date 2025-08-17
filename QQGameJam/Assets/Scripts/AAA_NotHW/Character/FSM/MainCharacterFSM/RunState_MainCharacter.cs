using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState_MainCharacter : IState
{
    private MainCharacterFSM fsm;
    private float Speed; // 运行速度
    private Rigidbody2D rb;
    private Vector2 direction => fsm.inputDirection.normalized;
    private float maxY = -1f; // 最大 Y 值
    private float minY = -1.8f; // 最小 Y 值

    public RunState_MainCharacter(MainCharacterFSM fsm)
    {
        this.fsm = fsm;
        this.rb = fsm.GetComponent<Rigidbody2D>();
    }

    public void OnEnter()
    {
        // Debug.Log("Entering Run State");
        this.Speed = 2.2f; // 初始运行速度
        if (fsm.animator == null)
        {
            Debug.LogError("Animator is not assigned in RunState_MainCharacter");
        }
        else
        {
            fsm.animator.SetBool("isMoving", true); // 确保动画状态正确设置
        }
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
            Vector2 curDirection = direction;
            Vector2 curPosition = rb.position;
            // 限制 Y 轴的移动范围
            if (curPosition.y > maxY && curDirection.y > 0)
            {
                curDirection.y = 0; // 限制最大 Y 值
            }
            else if (curPosition.y < minY && curDirection.y < 0)
            {
                curDirection.y = 0; // 限制最小 Y 值
            }
            // 更新刚体的速度
            rb.velocity = curDirection * Speed;
            fsm.RotateTowardsTarget(direction, false); // 旋转角色朝向目标
        }
    }

    public void OnExit()
    {
        // Debug.Log("Exiting Run State");
        rb.velocity = Vector2.zero;
        fsm.animator.SetBool("isMoving", false); // 设置动画状态为非运行
    }
}
