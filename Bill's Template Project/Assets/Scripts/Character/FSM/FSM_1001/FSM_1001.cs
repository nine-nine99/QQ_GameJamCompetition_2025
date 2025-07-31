using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_1001 : FSM_1000
{
    public Vector2 inputDirection => GetInputDirection2D();
    public Animator animator => transform.GetChild(0).GetComponent<Animator>();
    public override void Start()
    {
        states.Add(State.Idle, new IdleState_1001(this));
        states.Add(State.Run, new RunState_1001(this));

        currentState = states[State.Idle];
        currentState.OnEnter();
    }
    private Vector2 GetInputDirection2D()
    {
        float x = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
        float y = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
        Vector2 dir = new Vector2(x, y);
        return dir.normalized;
    }
}
