using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// 辅助类,自定义unity组件方法
/// </summary>
static public class Util {

    //GameObject上添加组件，如果组件存在直接返回
    static public T AddMissingComponent<T>(this GameObject go) where T : Component {
        T comp = go.GetComponent<T>();
        if (comp == null) {
            comp = go.AddComponent<T>();
        }
        return comp;
    }

    //Component上添加组件，如果组件存在直接返回
    static public T AddMissingComponent<T>(this Component _comp) where T : Component {
        T result = null;
        if (_comp != null) {
            result = _comp.gameObject.AddMissingComponent<T>();
        }
        return result;
    }

    //设置带参数的Text的值
    static public void SetTextFormat(this Text text, string str, params object[] objs) {
        if (text != null) {
            LangProxy langProxy = text.AddMissingComponent<LangProxy>();
            langProxy.langKey = str;
            langProxy.langValue = objs;
            langProxy.isDynamic = true;
            text.text = string.Format(RefLanguage.GetValue(str), objs);
        }
    }

    //设置翻译后的Text的值
    static public void SetText(this Text text, string str) {
        if (text != null) {
            LangProxy langProxy = text.AddMissingComponent<LangProxy>();
            langProxy.langKey = str;
            langProxy.isDynamic = true;
            text.text = RefLanguage.GetValue(str);
        }
    }

    //设置Text显示的日期值为 y-m-d 的格式
    static public void SetText(this Text text, DateTime str) {
        if (text != null) {
            text.text = string.Format("{0}-{1}-{2}", str.Year, str.Month, str.Day);
        }
    }

    //获取GameObject下的子物体组件
    static public T GetChildControl<T>(this GameObject _obj, string _target) where T : Component {
        Transform child = _obj.transform.Find(_target);
        if (child != null) {
            return child.GetComponent<T>();
        }
        return null;
    }

    //设置Transform的本地坐标的Y值
    static public void SetPosY(this Transform trans, float y) {
        trans.localPosition = new Vector3(trans.localPosition.x, y, trans.localPosition.z);
    }

    //设置Transform的本地坐标的X值
    static public void SetPosX(this Transform trans, float x) {
        trans.localPosition = new Vector3(x, trans.localPosition.y, trans.localPosition.z);
    }

    //设置Transform的本地坐标的X值
    static public void SetPosZ(this Transform trans, float z) {
        trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, z);
    }

    //设置Transform的本地欧拉角的X值
    static public void SetRotX(this GameObject go, float angle) {
        go.transform.localEulerAngles = new Vector3(angle, go.transform.localEulerAngles.y, go.transform.localEulerAngles.z);
    }

    //设置Transform的本地欧拉角的Y值
    static public void SetRotY(this GameObject go, float angle) {
        go.transform.localEulerAngles = new Vector3(go.transform.localEulerAngles.x, angle, go.transform.localEulerAngles.z);
    }

    //设置Transform的本地欧拉角的Z值
    static public void SetRotZ(this GameObject go, float angle) {
        go.transform.localEulerAngles = new Vector3(go.transform.localEulerAngles.x, go.transform.localEulerAngles.y, angle);
    }

    //设置Image的图片
    static public void SetSprite(this Image image, string spriteName, string atlasName = "UI", bool setNative = false) {
        if (image == null)
            return;
        Sprite sprite = LocalAssetMgr.Instance.Load_UISprite(atlasName, spriteName);
        if (sprite == null) {
            Debug.LogError($"Sprite is null {atlasName}/{spriteName}");
        }
        image.sprite = sprite;

        if (setNative) {
            image.SetNativeSize();
        }
    }

    //设置SpriteRenderer的图片
    static public void SetSprite(this SpriteRenderer sr, string spriteName, string atlasName = "UI") {
        if (sr == null)
            return;
        Sprite sprite = LocalAssetMgr.Instance.Load_UISprite(atlasName, spriteName);
        if (sprite == null) {
            Debug.LogError($"Sprite is null {atlasName}/{spriteName}");
        }
        sr.sprite = sprite;
    }

    //设置Image的alpha值
    static public void SetAlpha(this Image img, float a) {
        img.color = new Color(img.color.r, img.color.g, img.color.b, a);
    }

    //设置Text的alpha值
    static public void SetAlpha(this Text txt, float a) {
        txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, a);
    }

    //设置RawImage的alpha值
    static public void SetAlpha(this RawImage img, float a) {
        img.color = new Color(img.color.r, img.color.g, img.color.b, a);
    }

    //设置Image的RBG值
    static public void SetRGB(this Image img, Color color) {
        img.color = new Color(color.r, color.g, color.b, img.color.a);
    }

    //设置Text的RBG值
    static public void SetRGB(this Text txt, Color color) {
        txt.color = new Color(color.r, color.g, color.b, txt.color.a);
    }

    //设置Transform的本地缩放值
    static public void SetScale(this Transform trans, float scale) {
        trans.localScale = new Vector3(scale, scale, scale);
    }

    //EventTrigger组件监听
    static public void AddListener(this EventTrigger eventTrigger, EventTriggerType eventType, UnityAction<BaseEventData> action) {
        for (int index = 0; index < eventTrigger.triggers.Count; index++) {
            if (eventTrigger.triggers[index].eventID == eventType) {
                eventTrigger.triggers[index].callback.AddListener(action);
                return;
            }
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener(action);
        eventTrigger.triggers.Add(entry);
    }

    //EventTrigger组件反监听
    static public void RemoveListener(this EventTrigger eventTrigger, EventTriggerType eventType, UnityAction<BaseEventData> action) {
        for (int index = 0; index < eventTrigger.triggers.Count; index++) {
            if (eventTrigger.triggers[index].eventID == eventType) {
                eventTrigger.triggers[index].callback.RemoveListener(action);
            }
        }
    }
}
