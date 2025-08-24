using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnTag : MonoBehaviour
{
    public Button tag_1;
    public Button tag_2;
    public Transform menu1;
    public Transform menu2;

    private void OnEnable()
    {
        // 为按钮添加点击事件
        tag_1.onClick.AddListener(OnTag1Clicked);
        tag_2.onClick.AddListener(OnTag2Clicked);
    }
    private void OnDisable()
    {
        // 移除按钮点击事件
        tag_1.onClick.RemoveListener(OnTag1Clicked);
        tag_2.onClick.RemoveListener(OnTag2Clicked);
    }

    private void OnTag1Clicked()
    {
        menu1.SetAsLastSibling(); // 确保菜单1在最上层
        // 显示菜单1，隐藏菜单2
        menu1.gameObject.SetActive(true);
        menu2.gameObject.SetActive(true);
    }

    private void OnTag2Clicked()
    {
        menu2.SetAsLastSibling(); // 确保菜单2在最上层
        // 显示菜单2，隐藏菜单1
        menu1.gameObject.SetActive(true);
        menu2.gameObject.SetActive(true);
    }
}
