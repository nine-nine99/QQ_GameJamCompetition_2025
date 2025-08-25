using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterFSM : BaseFSM
{
    public Vector2 inputDirection => GetInputDirection2D();
    public Animator animator => transform.GetChild(0).GetComponent<Animator>();

    [Header("移动边界")]
    public bool useLimit = false;
    public Vector2 minPos;
    public Vector2 maxPos;
    public override void Start()
    {
        states.Add(State.Idle, new IdleState_MainCharacter(this));
        states.Add(State.Run, new RunState_MainCharacter(this));

        currentState = states[State.Idle];
        currentState.OnEnter();
    }

    private void Update()
    {
        // 保持状态机的更新
        if (currentState != null)
        {
            currentState.OnUpdate();
        }

        // 每帧应用边界限制
        ClampPosition();
    }
    private Vector2 GetInputDirection2D()
    {
        float x = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
        float y = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
        Vector2 dir = new Vector2(x, y);
        return dir.normalized;
    }

    private void ClampPosition()
    {
        if (!useLimit) return;

        float clampX = Mathf.Clamp(transform.position.x, minPos.x, maxPos.x);
        float clampY = Mathf.Clamp(transform.position.y, minPos.y, maxPos.y);

        transform.position = new Vector3(clampX, clampY, transform.position.z);
    }
}
