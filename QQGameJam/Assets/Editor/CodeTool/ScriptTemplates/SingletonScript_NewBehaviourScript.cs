using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonScript_NewBehaviourScript : Singleton<SingletonScript_NewBehaviourScript> {

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
