using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
   public static Action<string> SceneTransitionEvent; // 场景切换事件
    public static void CallSceneTransitionEvent(string newSceneName)
    {
        SceneTransitionEvent?.Invoke(newSceneName);
    }
    public static Action<string> SceneAddEvent; // 场景添加事件
    public static void CallSceneAddEvent(string newSceneName)
    {
        SceneAddEvent?.Invoke(newSceneName);
    }
    public static Action GameStartEvent; // 游戏开始事件
    public static void CallGameStartEvent()
    {
        GameStartEvent?.Invoke();
    }
    public static Action GameOverEvent; // 游戏结束事件
    public static void CallGameOverEvent()
    {
        GameOverEvent?.Invoke();
    }
    public static Action<int> ClickSlot1001;
    public static void CallClickSlot1001(int id)
    {
        ClickSlot1001?.Invoke(id);
    }
    // 场景卸载前
    public static Action<string> SceneUnloadEvent;
    public static void CallSceneUnloadEvent(string sceneName)
    {
        SceneUnloadEvent?.Invoke(sceneName);
    }
    // 场景加载之前
    public static Action<string> BeforeSceneLoadEvent;
    public static void CallBeforeSceneLoadEvent(string sceneName)
    {
        BeforeSceneLoadEvent?.Invoke(sceneName);
    }
    // 场景加载完成
    public static Action<string> AfterSceneLoadEvent;
    public static void CallAfterSceneLoadEvent(string sceneName)
    {
        AfterSceneLoadEvent?.Invoke(sceneName);
    }



    public static Action<int, Vector3> CreaterNewSoldier;
    public static void CallCreaterNewSoldier(int id, Vector3 pos)
    {
        CreaterNewSoldier?.Invoke(id, pos);
    }

    public static Action<int, Vector3> CreaterNewEnemy;
    public static void CallCreaterNewEnemy(int id, Vector3 pos)
    {
        CreaterNewEnemy?.Invoke(id, pos);
    }

    public static Action<int, Vector3> CreaterNewItem;
    public static void CallCreaterNewItem(int id, Vector3 pos)
    {
        CreaterNewItem?.Invoke(id, pos);
    }

    // 储存的硬币数量变化的事件
    public static Action<int> CoinCountChange;
    public static void CallCoinCountChange(int count)
    {
        CoinCountChange?.Invoke(count);
    }

    // 离开Bound边界
    public static Action<Transform> LeaveBound;
    public static void CallLeaveBound(Transform transform)
    {
        LeaveBound?.Invoke(transform);
    }

    public static Action<int, Vector3> CreaterNewMagicRain;
    public static void CallCreaterNewMagicRain(int id, Vector3 pos)
    {
        CreaterNewMagicRain?.Invoke(id, pos);
    }

    public static Action OnLevelUp;
    public static void CallOnLevelUp()
    {
        OnLevelUp?.Invoke();
    }

    public static Action<int> ClickSlot1002;
    public static void CallClickSlot1002(int index)
    {
        ClickSlot1002?.Invoke(index);
    }
}
