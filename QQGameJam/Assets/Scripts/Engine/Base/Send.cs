using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 消息广播管理（一般消息，不含req）
/// </summary>
public class Send : BaseSend<SendType, Send> {
    /// <summary>
    /// 发送广播
    /// </summary>
    public static void SendMsg (SendType _type, params object[] _objs) {
        Send.Instance.BaseSendMsg(_type, _objs);
    }

    /// <summary>
    /// 注册广播事件
    /// </summary>
    /// <param name="_type">广播类型</param>
    /// <param name="_callback">广播回调</param>
    /// Send.RegisterMsg(MsgType.None, SampleBroadcast);
    /// Send.UnregisterMsg(MsgType.None, SampleBroadcast);
    /// Send.SendMsg(MsgType.None, new object(), new object()......);
    /// void SampleBroadcast(object[] _objs){
    ///     .......
    /// }
    public static void RegisterMsg (SendType _type, BroadcastCallBack _callback) {
        Send.Instance.BaseRegisterMsg(_type, _callback);
    }

    /// <summary>
    /// 反注册广播事件
    /// </summary>
    public static void UnregisterMsg (SendType _type, BroadcastCallBack _callback) {
        Send.Instance.BaseUnregisterMsg(_type, _callback);
    }
}

/// <summary>
/// 消息广播管理基类
/// </summary>
public class BaseSend<T, K> : Singleton<K> where K : ISingleton, new() {
    Dictionary<T, List<BroadcastCallBack>> broadcastTable = new Dictionary<T, List<BroadcastCallBack>>();

    /// <summary>
    /// 注册广播事件
    /// </summary>
    /// <param name="_type">广播类型</param>
    /// <param name="_callback">广播回调</param>
    /// Send.RegisterMsg(MsgType.None, SampleBroadcast);
    /// Send.UnregisterMsg(MsgType.None, SampleBroadcast);
    /// Send.SendMsg(MsgType.None, new object(), new object()......);
    /// void SampleBroadcast(object[] _objs){
    ///     .......
    /// }
    public void BaseRegisterMsg (T _type, BroadcastCallBack _callback) {
        List<BroadcastCallBack> broadcast;
        if (!broadcastTable.TryGetValue(_type, out broadcast)) {
            broadcast = new List<BroadcastCallBack>();
            broadcastTable[_type] = broadcast;
        }
        if (!broadcast.Contains(_callback)) {
            broadcast.Add(_callback);
        }
    }

    /// <summary>
    /// 反注册广播事件
    /// </summary>
    /// <param name="_type">反注册类型</param>
    /// <param name="_callback">反注册广播事件</param>
    public void BaseUnregisterMsg (T _type, BroadcastCallBack _callback) {
        List<BroadcastCallBack> broadcast;
        if (!broadcastTable.TryGetValue(_type, out broadcast)) {
            return;
        }
        if (broadcast.Contains(_callback)) {
            broadcast.Remove(_callback);
        }
    }

    /// <summary>
    /// 广播
    /// </summary>
    /// <param name="_type">广播类型</param>
    /// <param name="_objs">广播不定参</param>
    public void BaseSendMsg (T _type, params object[] _objs) {
        if (broadcastTable.ContainsKey(_type)) {
            List<BroadcastCallBack> broadcast = new List<BroadcastCallBack>(broadcastTable[_type]);
            for (int i = 0; i != broadcast.Count; ++i) {
                broadcast[i](_objs);
            }
        }
    }
}