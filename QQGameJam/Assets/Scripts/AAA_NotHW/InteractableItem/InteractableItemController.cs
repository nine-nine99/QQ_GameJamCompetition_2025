using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItemController : MonoBehaviour
{
    private Transform spriteTransform;

    private void Start()
    {
        spriteTransform = transform;
    }

    // 鼠标进入时调用
    private void OnMouseEnter()
    {
        // 添加高亮效果，比如放大
        spriteTransform.localScale = Vector3.one * 1.1f;
    }

    // 鼠标离开时调用
    private void OnMouseExit()
    {
        // 恢复原始大小
        spriteTransform.localScale = Vector3.one;
    }

    // 鼠标悬停时持续调用
    private void OnMouseOver()
    {
        // 在这里添加持续的悬停效果
    }

    // 鼠标点击时调用
    private void OnMouseDown()
    {
        // 添加点击处理逻辑
        WindowMgr.Instance.OpenWindow<InteractableItemWindow>();
    }
}
