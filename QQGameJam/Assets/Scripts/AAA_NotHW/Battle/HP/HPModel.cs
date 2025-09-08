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
            if (value >= MaxHP)
            {
                hp = MaxHP;
                return;
            }
            if (value < 0)
            {
                hp = 0;
                return;
            }
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
        
        HP += change;

        if (HP <= 0)
        {
            if (BGMController.Instance.bgmBattleStart)
            {
                // 发出音游结束的命令
                Send.SendMsg(SendType.MusicBattleEnd, true);
                return;
            }
        }
        Debug.Log(HP);
    }
}
