using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;

/// <summary>
///  基础表结构，所有表继承自这个类
/// </summary>
[NeedsPreserve]
public class RefBase {
    protected static Dictionary<string, string> lineValue;
    protected int line;

    public RefBase() {

    }

    public virtual string GetFirstKeyName() {
        return "refbase";
    }

    public virtual void LoadByLine(Dictionary<string, string> _value, int _line) {
        lineValue = _value;
        line = _line;
    }

    public void ClearData() {
        lineValue = null;
    }

    protected int GetInt(string key) {
        string strValue = "";
        if (!lineValue.TryGetValue(key, out strValue)) {
            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
            return 0;
        }

        if (string.IsNullOrEmpty(strValue))
            return 0;

        int tempValue = 0;
        int.TryParse(strValue, out tempValue);
        return tempValue;
    }

    protected float GetFloat(string key) {
        string strValue = "";
        if (!lineValue.TryGetValue(key, out strValue)) {
            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
            return 0f;
        }

        if (string.IsNullOrEmpty(strValue)) {
            return 0;
        }

        float tempValue = 0;
        float.TryParse(strValue, out tempValue);
        return tempValue;
    }

    protected double GetDouble(string key) {
        string strValue = "";
        if (!lineValue.TryGetValue(key, out strValue)) {
            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
            return 0f;
        }

        if (string.IsNullOrEmpty(strValue)) {
            return 0;
        }

        double tempValue = 0;
        double.TryParse(strValue, out tempValue);
        return tempValue;
    }

    protected bool GetBool(string key) {
        string strValue = "";
        if (!lineValue.TryGetValue(key, out strValue)) {
            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
            return false;
        }

        if (string.IsNullOrEmpty(strValue)) {
            return false;
        }

        bool tempValue = false;
        if (!bool.TryParse(strValue, out tempValue)) {
            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
        }
        return tempValue;
    }

    protected string GetString(string key) {
        string strValue = "";
        if (!lineValue.TryGetValue(key, out strValue)) {
            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
            return "";
        }

        if (string.IsNullOrEmpty(strValue)) {
            return "";
        }
        return strValue;
    }

    protected object GetEnum(string key, Type _type) {
        string strValue = "";
        if (!lineValue.TryGetValue(key, out strValue)) {
            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
            return Enum.Parse(_type, strValue, true);
        }
        if (strValue == string.Empty || strValue == "") {
            return Activator.CreateInstance(_type);
        }
        try {
            return Enum.Parse(_type, strValue, true);
        }
        catch {
            Debug.LogError(string.Format("字段名{0}:填写错误 {1}枚举不存在{2}这个值, 将返回一个默认值", key, _type.Name, strValue));
            return Activator.CreateInstance(_type);
        }
    }

    protected List<T> GetList<T>(string key, Type _type) {
        string strValue = "";
        List<T> list = new List<T>();
        if (!lineValue.TryGetValue(key, out strValue)) {
            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
            return list;
        }

        if (!strValue.Equals("")) {
            string[] strs = strValue.Split(new char[] { ';' });
            foreach (string tempStr in strs) {
                object value = ParseValue(tempStr, _type);
                if (value == null) {
                    Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
                }

                list.Add((T)value);
            }
        }

        return list;
    }

    protected List<T> GetList<T>(string orgi, Type _type, char separator) {

        List<T> list = new List<T>();

        if (string.IsNullOrEmpty(orgi)) {
            Debug.LogError(string.Format("原字符串{0}：为空", orgi));
            return list;
        }

        string[] strs = orgi.Split(new char[] { separator });
        foreach (string tempStr in strs) {
            object value = ParseValue(tempStr, _type);
            if (value == null) {
                Debug.LogError(string.Format("字符串转换失败,原字符串为{0}", orgi));
            }

            list.Add((T)value);
        }
        return list;
    }


    // 解析字段值
    private object ParseValue(string _value, Type _type) {
        try {
            if (_value.Equals(string.Empty)) {
                if (_type == typeof(string)) {
                    return "";
                }
                return Activator.CreateInstance(_type);
            }
            else {
                _value = _value.Trim();

                // 枚举 暂不支持
                if (_type.IsEnum) {
                    return Enum.Parse(_type, _value, true);
                }

                // 字符串
                else if (_type == typeof(string)) {
                    return _value;
                }

                // 浮点型
                else if (_type == typeof(float)) {
                    if (_value == "0" || _value == "" || _value == string.Empty)
                        return 0;

                    return float.Parse(_value);
                }

                // 整形
                else if (_type == typeof(int)) {
                    if (_value == "")
                        return 0;

                    return int.Parse(_value);
                }

                else if (_type == typeof(bool)) {
                    return bool.Parse(_value);
                }

                else if (_type == typeof(long)) {
                    return long.Parse(_value);
                }
            }
        }
        catch (System.Exception ex) {
            Debug.LogError(string.Format("ParseValue type:{0}, value:{1}, failed: {2}", _type.ToString(), _value, ex.Message));
        }
        return null;
    }
}
