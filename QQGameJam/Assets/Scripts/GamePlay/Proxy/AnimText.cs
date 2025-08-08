using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 数字动画文本
/// </summary>
public class AnimText : Text {
    private string headStr = "";
    private float animTime = 0.5f;
    private float curAnimTime = 0f;
    private int targetValue;
    private int curValue;
    private int lastValue;
    private bool skipAnim = true; //每次开界面第一次赋值是否跳过动画 大多数情况下要跳过
    private int skipNumLimit = 3;//数字低于某值没有动画的必要

    protected override void OnDisable() {
        base.OnDisable();
        skipAnim = true;
    }

    public void SetHeadStr(string _headStr) {
        headStr = _headStr;
    }

    public void SetAnimTime(float _animTime) {
        animTime = _animTime;
    }

    public void SetTarget(int _targetValue) {
        if (Mathf.Abs(curValue - _targetValue) <= skipNumLimit)
            skipAnim = true;

        if (skipAnim) {
            lastValue = _targetValue;
            curValue = _targetValue;
            targetValue = _targetValue;
            curAnimTime = animTime;
            skipAnim = false;
            Texthandle();
        }
        else {
            lastValue = curValue;
            targetValue = _targetValue;
            curAnimTime = 0;
        }
    }

    void Update() {
        if (curAnimTime < animTime) {
            curAnimTime += Time.deltaTime;
            curValue = ToolMgr.Lerp(lastValue, targetValue, curAnimTime / animTime);
            if (curAnimTime >= animTime) {
                curValue = targetValue;
            }
            Texthandle();
        }
    }

    private void Texthandle() {
        text = headStr + curValue;
    }
}