using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 数据管理类
/// </summary>
public class RefDataMgr : BaseRefDataMgr<RefDataMgr> {
    /// <summary>
    /// 初始加载
    /// </summary>
    public IEnumerator Init() {
        Debug.Log("RefDataMgr Init Start!!!");

        List<IEnumerator> co_list = new List<IEnumerator>() {
            Co_LoadGeneric(RefShop.cacheMap),
            Co_LoadGeneric(RefSign.cacheMap),
            Co_LoadGeneric(RefTask.cacheMap),
        };
        for (int index = 0, total = co_list.Count; index < total; index++) {
            yield return CoDelegator.Coroutine(co_list[index]);
            //WinMsg.SendMsg(WinMsgType.ProcessLoad_Refdata, index, total, (index + 1.0f) / total);
        }
        Debug.Log("RefDataMgr Init End!!!");

        yield break;
    
}

    public void InitBasic() {
        LoadGeneric(RefShop.cacheMap);
        LoadGeneric(RefSign.cacheMap);
        LoadGeneric(RefTask.cacheMap);
    
}
}
