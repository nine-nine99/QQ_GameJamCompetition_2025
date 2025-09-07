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
            if (hp >= MaxHP)
                hp = MaxHP;
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

        // TODO:这里以后要修正
        if (hp <= 0)
        {
            hp = 0;
            if (BGMController.Instance.isBegin)
            {
                // 发出音游结束的命令
                Send.SendMsg(SendType.MusicBattleEnd, true);
            }
        }
        Debug.Log(HP);
    }
}
