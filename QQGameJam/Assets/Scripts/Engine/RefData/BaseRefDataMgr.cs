using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基础数据管理类
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseRefDataMgr<T> : Singleton<T> where T : ISingleton, new() {

    protected static int pendings = 0;

    protected static IEnumerator Co_LoadGeneric<TKey, TValue> (Dictionary<TKey, TValue> _table) where TValue : RefBase {
        pendings += 1;
        Type _valueType = typeof(TValue);
        // 读二维表
        // 从D2TableManager 获取
        string tableName = _valueType.Name.Substring(3).ToLower();

        LocalAssetMgr.Instance.Load_RefData(tableName,
                delegate(TextAsset asset) {
                    Dictionary<string, Dictionary<string, string>> tableInfo = D2TableMgr.LoadTable(asset);
                    if (D2TableMgr.LoadByTable(_table, tableInfo)) {
                        pendings -= 1;
                    }
                });

        while (pendings > 0) {
            yield return null;
        }
    }

    public static void LoadGeneric<TKey, TValue> (Dictionary<TKey, TValue> _table) where TValue : RefBase {
        pendings += 1;
        Type _valueType = typeof(TValue);
        // 读二维表
        // 从D2TableManager 获取
        string tableName = _valueType.Name.Substring(3).ToLower();
        Debug.Log(string.Format("LoadTable:<{0}> start ...", tableName));

        LocalAssetMgr.Instance.Load_RefData(tableName,
                delegate(TextAsset asset) {
                    Dictionary<string, Dictionary<string, string>> tableInfo = D2TableMgr.LoadTable(asset);
                    if (D2TableMgr.LoadByTable(_table, tableInfo)) {
                        pendings -= 1;
                    }
                });
    }

    // 解析字段值
    private static object ParseValue (string _value, Type _type) {
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
