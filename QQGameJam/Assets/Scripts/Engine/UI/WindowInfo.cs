using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 基础界面信息
/// 存储窗口的基本属性和动画设置
/// </summary>
[SerializeField]
public class WindowInfo : MonoBehaviour {
    /// <summary>
    /// 窗口类型
    /// </summary>
    public WindowType windowType = WindowType.Normal;

    /// <summary>
    /// 打开动画类型
    /// </summary>
    public OpenAnimType openAnimType = OpenAnimType.None;

    /// <summary>
    /// 关闭动画类型
    /// </summary>
    public OpenAnimType closeAnimType = OpenAnimType.None;

    /// <summary>
    /// 动画持续时间（秒）
    /// </summary>
    public float animTime = 0.3f;

    /// <summary>
    /// 点击空白区域是否关闭窗口
    /// </summary>
    public bool closeOnEmpty = false;

    /// <summary>
    /// 是否显示背景遮罩
    /// </summary>
    public bool mask = false;

    /// <summary>
    /// 默认位置
    /// </summary>
    public Vector3 defaultPos = Vector3.zero;

    /// <summary>
    /// 打开后的位置
    /// </summary>
    public Vector3 openPos = Vector3.zero;

    /// <summary>
    /// 窗口分组
    /// </summary>
    public int group = 0;

    /// <summary>
    /// 用于自动配置的子物体列表 - 仅在编辑器中使用，不会打包进游戏
    /// 可以拖拽预制体下的子物体到此列表中
    /// </summary>
#if UNITY_EDITOR
    [HideInInspector]
    public List<GameObject> childObjects = new List<GameObject>();
#endif
}