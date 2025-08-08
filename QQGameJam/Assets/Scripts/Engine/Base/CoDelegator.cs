using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
/// <summary>
/// 常驻协程代理类，可有效避免因SetActive(false)引发的不执行问题
/// </summary>
public class CoDelegator : SingletonMonoBehavior<CoDelegator> {
    // 开始协程
    public static Coroutine Coroutine (IEnumerator routine) {
        return Instance.StartCoroutine(routine);
    }

    // 停止协程
    public static void StopCoroutineEx(IEnumerator routine) {
        Instance.StopCoroutine(routine);
    }

    public static void StopCoroutineEx(Coroutine routine) {
        Instance.StopCoroutine(routine);
    }

    public static void StopCoroutineEx (string _routine) {
        Instance.StopCoroutine(_routine);
    }
}
