using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_1001 : IState
{
    private FSM_1001 fsm;

    public IdleState_1001(FSM_1001 fsm)
    {
        this.fsm = fsm;
    }

    public void OnEnter()
    {
        // Logic for entering the idle state
    }

    public void OnUpdate()
    {
        if (fsm.inputDirection != Vector2.zero)
        {
            fsm.ChangeState(State.Run);
        }
    }

    public void OnExit()
    {
        
    }
}
