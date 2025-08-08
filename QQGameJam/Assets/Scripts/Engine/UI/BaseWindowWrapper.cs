using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BaseWindowWrapper<T> : BaseWindow where T : BaseWindow {

    /// <summary>
    /// instance 保证一定加载了UI资源
    /// </summary>
    private static T instance_ = null;
    public static T Instance {
        get {
            if (null == WindowMgr.Instance)
                return null;
            if (instance_ == null) {
                instance_ = WindowMgr.Instance.GetWindow(typeof(T).Name) as T;
            }
            return instance_;
        }
    }
}