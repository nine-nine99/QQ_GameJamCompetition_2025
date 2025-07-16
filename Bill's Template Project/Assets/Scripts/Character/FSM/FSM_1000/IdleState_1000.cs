using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_1000 : IState
{
    private FSM_1000 fsm;
    public IdleState_1000(FSM_1000 fsm)
    {
        this.fsm = fsm;
    }

    public void OnEnter()
    {
        // Idle state enter logic
    }

    public void OnUpdate()
    {
        // Idle state update logic
    }

    public void OnExit()
    {
        // Idle state exit logic
    }
}
