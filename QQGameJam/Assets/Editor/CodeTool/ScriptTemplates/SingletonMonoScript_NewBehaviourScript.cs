using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoScript_NewBehaviourScript : SingletonMonoBehavior<SingletonMonoScript_NewBehaviourScript> {

    public void Init() {
        InitMsg();
    }

    public void Clear() {
        ClearMsg();
    }

    public void InitMsg() {

    }

    public void ClearMsg() {

    }
}
