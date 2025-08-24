using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPModel : MonoBehaviour
{
    private int hp = 100;
    public int HP
    {
        set
        {
            hp = value;
        }
        get
        {
            return hp;
        }
    }
    public int MaxHP = 100;
    private void OnEnable()
    {
        HP = MaxHP;

        Send.RegisterMsg(SendType.HPChange, OnHPChange);
    }
    private void OnDisable()
    {
        Send.UnregisterMsg(SendType.HPChange, OnHPChange);
    }

    private void OnHPChange(params object[] data)
    {
        int change = (int)data[0];
        hp += change;
        Debug.Log(HP);
    }
}
