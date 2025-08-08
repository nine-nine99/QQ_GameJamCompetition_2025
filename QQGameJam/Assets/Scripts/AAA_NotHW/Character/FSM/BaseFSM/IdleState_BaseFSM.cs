using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_BaseFSM : IState
{
    private BaseFSM fsm;
    public IdleState_BaseFSM(BaseFSM fsm)
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
