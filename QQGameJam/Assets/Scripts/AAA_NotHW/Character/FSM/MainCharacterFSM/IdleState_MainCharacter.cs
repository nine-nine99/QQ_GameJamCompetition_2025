using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_MainCharacter : IState
{
    private MainCharacterFSM fsm;

    public IdleState_MainCharacter(MainCharacterFSM fsm)
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
