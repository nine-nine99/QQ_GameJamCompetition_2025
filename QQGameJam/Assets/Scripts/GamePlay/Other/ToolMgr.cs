using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// 工具类
/// 依赖LocalAssetMgr CoDelegator 可删减
/// </summary>
public class ToolMgr : Singleton<ToolMgr> {

    /// <summary>
    /// 从List中随机返回值
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="list">需要随机取值的List</param>
    /// <returns>随机返回的值</returns>
    public int RandomRange(List<int> list) {
        if (list.Count == 0) {
            Debug.LogError("getrandom list is null");
            return 0;
        }
        else if (list.Count == 1) {
            return list[0];
        }
        else {
            return Random.Range(list[0], list[1] + 1);
        }
    }

    /// <summary>
    /// 从List中随机返回值并移除
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="list">需要随机取值的List</param>
    /// <returns>随机返回的值</returns>
    public T RandomAndRemove<T>(List<T> list) {
        if (list.Count == 0) {
            Debug.LogError("getrandom list is null");
            return default(T);
        }
        int index = Random.Range(0, list.Count);
        T result = list[index];
        list.RemoveAt(index);
        return result;
    }

    /// <summary>
    /// 从List中根据权重随机返回值
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="list">需要随机取值的List</param>
    /// <param name="weightList">权重List</param>
    /// <returns>根据权重随机处的值</returns>
    public T RandomWithWeight<T>(List<T> list, List<int> weightList) {
        if (list.Count == 0) {
            Debug.LogError("getrandom list is null");
            return default(T);
        }
        if (list.Count == 1) {
            return list[0];
        }
        if (list.Count != weightList.Count) {
            Debug.LogError("list and weight count is unique");
            return list[0];
        }

        T result = default(T);
        int totalWeight = weightList.Sum();
        int randomWeight = Random.Range(0, totalWeight);

        int curTotalWeight = 0;
        for (int index = 0; index < list.Count; index++) {
            int curWeight = weightList[index];
            curTotalWeight += curWeight;
            if (curTotalWeight > randomWeight) {
                return list[index];
            }
        }

        return result;
    }

    /// <summary>
    /// 从最小和最大值之间随机返回值(不包含最大值)
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>随机值</returns>
    public int Range(int min, int max) {
        return Random.Range(min, max);
    }

    /// <summary>
    /// 从最小和最大值之间随机返回值(包含最大值)
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>随机值</returns>
    public int RangeWithMax(int min, int max) {
        max = max + 1;
        return Random.Range(min, max);
    }

    /// <summary>
    /// 随机返回真假值
    /// </summary>
    /// <returns>真假值</returns>
    public  bool RangeTrueOrFalse() {
        return Random.Range(0f, 1f) >= 0.5f;
    }

    /// <summary>
    /// 延迟执行函数
    /// </summary>
    /// <param name="callBack">函数</param>
    /// <param name="delayTime">延迟的值</param>
    public void DelayCallBack(System.Action callBack, float delayTime) {
        CoDelegator.Coroutine(CoDelayCallBack(callBack, delayTime));
    }

    private IEnumerator CoDelayCallBack(System.Action callBack, float delayTime) {
        yield return new WaitForSeconds(delayTime);
        callBack?.Invoke();
    }

    /// <summary>
    /// 将秒格式为{min}:{second}格式
    /// </summary>
    /// <param name="_second">需要转化的秒</param>
    /// <param name="containZero">是否显示0点</param>
    /// <returns>格式后的时间值</returns>
    public string GetTimeDesc(float _second, bool containZero = true) {
        if (_second == 0 && containZero == false) {
            return "--:--";
        }
        string desc = "";
        int second = (int)_second;
        int min = second / 60;
        second = second % 60;
        desc = string.Format("{0}:{1}", min < 10 ? ("0" + min) : min.ToString(),
            second < 10 ? ("0" + second) : second.ToString());
        return desc;
    }

    /// <summary>
    /// 获取当前语言的描述
    /// </summary>
    /// <param name="lang">语言类型</param>
    /// <returns>语言描述</returns>
    public string GetLangDesc(SystemLanguage lang) {
        switch (lang) {
            case SystemLanguage.Chinese:
                return "中文";
            case SystemLanguage.German:
                return "Deutsch";
            case SystemLanguage.French:
                return "Français";
            case SystemLanguage.Spanish:
                return "Español (ES)";
            case SystemLanguage.SerboCroatian://借用key
                return "Español (AL)";
            case SystemLanguage.Portuguese:
                return "Português (BR)";
            case SystemLanguage.Romanian://借用key
                return "Português (PT)";
            case SystemLanguage.Italian:
                return "Italiano";
            case SystemLanguage.Dutch:
                return "Nederlands";
            case SystemLanguage.Japanese:
                return "日本語";
            case SystemLanguage.Korean:
                return "한국의";
            case SystemLanguage.Russian:
                return "Русский";
            case SystemLanguage.Ukrainian:
                return "Українська";
            case SystemLanguage.Greek:
                return "Ελληνικά";
            case SystemLanguage.Turkish:
                return "Türk";
            default:
                return "English";
        }
    }

    /// <summary>
    /// 根据时间t 返回long型 a 到 b 的插件函数
    /// </summary>
    /// <param name="a">起始值</param>
    /// <param name="b">目标值</param>
    /// <param name="t">时间插值(0到1)</param>
    /// <returns>插值后的值</returns>
    public long Lerp(long a, long b, float t) {
        if (t <= 0)
            return a;
        else if (t >= 1)
            return b;
        return a + (long)((b - a) * t);
    }

    /// <summary>
    /// 根据时间t 返回int型 a 到 b 的插件函数
    /// </summary>
    /// <param name="a">起始值</param>
    /// <param name="b">目标值</param>
    /// <param name="t">时间插值(0到1)</param>
    /// <returns>插值后的值</returns>
    public static int Lerp(int a, int b, float t) {
        if (t <= 0)
            return a;
        else if (t >= 1)
            return b;
        return a + (int)((b - a) * t);
    }

    /// <summary>
    /// 获取贝塞尔曲线点
    /// </summary>
    /// <param name="startPoint">起点</param>
    /// <param name="midPoint">中点</param>
    /// <param name="endPoint">终点</param>
    /// <param name="vertexCount">曲线点个数</param>
    /// <returns>贝塞尔曲线上的点</returns>
    public List<Vector3> Bezier(Vector3 startPoint, Vector3 midPoint, Vector3 endPoint, float vertexCount) {
        List<Vector3> vertexsList = new List<Vector3>();
        float interval = 1 / vertexCount;
        for (int i = 0; i < vertexCount; i++) {
            vertexsList.Add(GetPoint(startPoint, midPoint, endPoint, i * interval));
        }
        vertexsList[vertexsList.Count - 1] = endPoint;
        return vertexsList;
    }

    private Vector3 GetPoint(Vector3 startPoint, Vector3 midPoint, Vector3 endPoint, float t) {
        float a = 1 - t;
        Vector3 target = startPoint * Mathf.Pow(a, 2) + 2 * midPoint * t * a + endPoint * Mathf.Pow(t, 2);
        return target;
    }

    /// <summary>
    /// Color转16进制
    /// </summary>
    /// <param name="color">颜色</param>
    /// <returns>16进制的值</returns>
    public string ColorToHex(Color color) {
        return ColorUtility.ToHtmlStringRGB(color);
    }

    /// <summary>
    /// 16进制转Color
    /// </summary>
    /// <param name="hex">16进制的值例如 "#DE66FFFF"</param>
    /// <returns>颜色</returns>
    public Color HexToColor(string hex) {
        Color color;
        if (!ColorUtility.TryParseHtmlString(hex, out color)) {
            Debug.LogError($"Hex {hex} To Color Failed!");
        }
        return color;
    }

    /// <summary>
    /// 根据圆心和半径获取圆上的点
    /// </summary>
    /// <param name="center">圆心</param>
    /// <param name="r">半径</param>
    /// <param name="count">点的个数</param>
    /// <returns>圆上的点</returns>
    public List<Vector2> GetCirclePoint(Vector2 center, float r, int count) {
        List<Vector2> circlePos = new List<Vector2>();
        float addAngle = (float)360 / count;//数量
        float curAngle = 0;
        for (int i = 0; i < count; i++) {
            float x = center.x + r * Mathf.Sin(curAngle * Mathf.Deg2Rad);
            float y = center.y + r * Mathf.Cos(curAngle * Mathf.Deg2Rad);
            curAngle += addAngle;
            circlePos.Add(new Vector2(x, y));
        }
        return circlePos;
    }

    /// <summary>
    /// 从屏幕点击处发出射线检查
    /// </summary>
    /// <param name="pos">屏幕坐标</param>
    /// <param name="RAY_DIS">射线距离</param>
    /// <param name="layerName">层级</param>
    /// <param name="camera">UI摄像头</param>
    /// <returns></returns>
    public Vector3? RayToPoint(Vector2 pos, float RAY_DIS, string layerName, Camera camera) {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(pos);
        LayerMask mask = 1 << LayerMask.NameToLayer(layerName);
        if (!Physics.Raycast(ray, out hit, RAY_DIS, mask)) {
            return null;
        }
        return hit.point;
    }

    /// <summary>
    /// 展示点赞
    /// </summary>
    public void ShowRate() {
#if UNITY_IOS
        UnityStoreKit storeKit = new UnityStoreKit();
        storeKit.GoToCommnet();
#elif UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass("com.aar.rate.RateUtils");
        if (pluginClass != null) {
            pluginClass.CallStatic("ShowRate");
        }
#endif
        Debug.Log("Show Rate");
    }
}
