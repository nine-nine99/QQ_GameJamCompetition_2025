using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 时间模块
/// </summary>
public class TimeMgr : Singleton<TimeMgr> {
    private float curTime = 0;
	public void Init(){
        Send.RegisterMsg(SendType.TimeUpdate, OnTimeUpdate);
	}
	
	public void Clear(){
        Send.UnregisterMsg(SendType.TimeUpdate, OnTimeUpdate);
	}

    private void OnTimeUpdate(object[] objs) {
        float time = (float)objs[0];
    }

    public static long GetTimeStamp(bool bflag = false) {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        long ret;
        if (bflag)
            ret = Convert.ToInt64(ts.TotalSeconds);
        else
            ret = Convert.ToInt64(ts.TotalMilliseconds);
        return ret;
    }

    public static string GetTimeDesc() {
        return DateTime.Now.ToString();
    }
}
