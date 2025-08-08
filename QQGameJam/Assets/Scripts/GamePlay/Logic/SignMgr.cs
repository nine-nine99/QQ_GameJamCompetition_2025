using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 签到管理类
/// </summary>
public class SignMgr : Singleton<SignMgr> {
    private const string LAST_SIGNED_TIME = "LastSignedTime";

    //签到天数
    public int SignDay {
        get {
            return LocalSave.GetInt(TaskType.SignInDay);
        }
        set {
            LocalSave.SetInt(TaskType.SignInDay, value);
            Send.SendMsg(SendType.TaskValueChange, TaskType.SignInDay);
            Send.SendMsg(SendType.SignDayChange, SignDay);
        }
    }

    //最后签到时间
    public string LastSignedTime {
        get {
            return PlayerPrefs.GetString(LAST_SIGNED_TIME, DateTime.Now.Date.AddDays(-1).ToString());
        }
        set {
            PlayerPrefs.SetString(LAST_SIGNED_TIME, value);
        }
    }

    public List<SignInfo> signInfoList = new List<SignInfo>();

    //当前日期距最后签到日期天数
    public int dayInterval;

	public void Init(){
        dayInterval = DateTime.Now.Date.Subtract(Convert.ToDateTime(LastSignedTime)).Days;
        InitList();
	}
	
	public void Clear(){
		
	}

    //创建签到表
    public void InitList() {
        signInfoList.Clear();
        foreach (RefSign refSign in RefSign.cacheMap.Values) {
            signInfoList.Add(new SignInfo(refSign));
        }
    }

    //刷新签到
    public void RefreshList() {
        //当前日期距最后签到日期天数
        dayInterval = DateTime.Now.Date.Subtract(Convert.ToDateTime(LastSignedTime)).Days;
        for (int index = 0; index < signInfoList.Count; index++ ) {
            SignInfo signInfo = signInfoList[index];
            signInfo.Refresh();
        }
    }

    //可签到接口
    public bool HasCanSign() {
        RefreshList();
        for (int index = 0; index < signInfoList.Count; index++) {
            SignInfo signInfo = signInfoList[index];
            if (signInfo.signState == SignState.CanSign)
                return true;
        }

        return false;
    }

    public SignInfo GetSignInfo(int day) {
        for (int index = 0; index < signInfoList.Count; index++) {
            SignInfo signInfo = signInfoList[index];
            if (signInfo.refSign.Day == day)
                return signInfo;
        }
        return null;
    }

    public void TrySign(int day) {
        SignInfo info = GetSignInfo(day);
        if (info == null) {
            Debug.LogError("signInfo is null:" + day);
            //show tip if need
            return;
        }

        if (info.signState != SignState.CanSign) {
            //show tip if need
            return;
        }

        SignMgr.Instance.SignDay++;
        SignMgr.Instance.LastSignedTime = System.DateTime.Now.Date.ToString();
        dayInterval = DateTime.Now.Date.Subtract(Convert.ToDateTime(LastSignedTime)).Days;
        info.Refresh();
        //这里只考虑了奖励只有一种的情况 有多种要额外处理
        CurrencyMgr.Instance.Gold += info.refSign.RewardNum;
    }
}

public class SignInfo {
    public RefSign refSign;
    public SignState signState;

    public SignInfo(RefSign _refSign) {
        refSign = _refSign;
        Refresh();
    }

    public void Refresh() {
        if (refSign.Day <= SignMgr.Instance.SignDay) {
            signState = SignState.Signed;
        }
        else if(refSign.Day == SignMgr.Instance.SignDay + 1 && SignMgr.Instance.dayInterval > 0 ) {
            //如果是已签到天数的下一天 并且上次签到间隔大于1
            signState = SignState.CanSign;
        }
        else {
            signState = SignState.SignLock;
        }
    }
}