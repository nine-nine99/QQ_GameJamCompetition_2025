using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;

/// <summary>
/// 数据解析类
/// </summary>
public static class D2TableMgr {

    public static Dictionary<string, Dictionary<string, string>> LoadTable (TextAsset tableText, int fieldLine = 2, int contentLine = 3) {
        Dictionary<string, Dictionary<string, string>> table = new Dictionary<string, Dictionary<string, string>>();
        if (tableText == null) {
            return table;
        }
        return LoadTableByText(tableText.name, tableText.text, fieldLine, contentLine);
    }

    public static Dictionary<string, Dictionary<string, string>> LoadTableByText (string tableName, string tableText, int fieldLine = 2, int contentLine = 3) {
        Dictionary<string, Dictionary<string, string>> table = new Dictionary<string, Dictionary<string, string>>();

        string[] stringReader = tableText.Split(new char[]{'\n'}, StringSplitOptions.RemoveEmptyEntries);
        int curLine = 0;
        string temp = null;
        string[] fields = null;
        for (int i = 0, max = stringReader.Length; i < max; ++i) {
            curLine = i + 1;
            temp = stringReader[i];

            if (string.IsNullOrEmpty(temp.Trim()))
                continue;

            if (fieldLine == curLine) {
                // 解析表头
                string[] values = temp.Split('\t');

                if (0 >= values.Length) {
                    Debug.LogError(string.Format("ParseTable fieldCnt <= 0, name:{0}", tableName));
                    return table;
                }

                fields = new string[values.Length];
                // 过滤空格
                for (int j = 0, jMax = values.Length; j < jMax; ++j) {
                    fields[j] = values[j].Trim();
                }
            }
            else if (curLine >= contentLine) {
                if (null == fields) {
                    Debug.LogError(string.Format("ParseTable fieldCnt null(fieldLine: {0}), name: {1}", fieldLine, tableName));
                    return table;
                }
                // 解析内容
                string[] values = temp.Split('\t');

                if (values.Length <= 1 && values.Length < fields.Length) {
                    Debug.LogError(string.Format("这是一个空行,表:{0} , 行:{1}", tableName, curLine));
                    continue;
                }

                // 单行字典
                Dictionary<string, string> lineValue = new Dictionary<string, string>();
                for (int index = 0; index < fields.Length; index++) {
                    if (values.Length > index) {
                        lineValue[fields[index]] = values[index].TrimStart().TrimEnd();
                    }
                    else {
                        lineValue[fields[index]] = "";
                    }
                }
                // 放入总表
                string key = values[0];
                try {
                    table.Add(key, lineValue);
                }
                catch (Exception ex) {
                    Debug.LogError(string.Format("{0} {1} : 键:{1} , 行:{2}", ex.Message, tableName, key, curLine));
                }
            }
        }
        return table;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="_table"></param>
    /// <param name="tableInfo"></param>
    /// <returns>true if you need sub counter of "pendings" </returns>
    public static bool LoadByTable<TKey, TValue> (Dictionary<TKey, TValue> _table, Dictionary<string, Dictionary<string, string>> tableInfo) where TValue : RefBase {
        int lineNumber = 3;

        Type _valueType = typeof(TValue);
        string tableName = _valueType.Name.Substring(3).ToLower();
        //RefBase ins = System.Activator.CreateInstance<TValue>();
        bool hasreturn = false;

        foreach (Dictionary<string, string> line in tableInfo.Values) {
            try {
                LoadGenericLineBySelf(_table, line, lineNumber);
            }
            catch (System.Exception ex) {
                Debug.LogException(ex);
                Debug.LogError(string.Format("{0}表第{1}行解析错误 字段填写错误或是枚举不存在或是存在空行", tableName, lineNumber));
            }
            ++lineNumber;
        }

        Debug.Log(string.Format("LoadTable:<{0}> end !!! cnt:{1}", tableName, _table.Count));

        return !hasreturn;
    }

    //用自己提供的方法解析 提高速度
    public static void LoadGenericLineBySelf<TKey, TValue> (Dictionary<TKey, TValue> _table, Dictionary<string, string> _value, int _line) where TValue : RefBase {
        // 对结构体设置数据
        TValue ins = System.Activator.CreateInstance<TValue>();
        TKey key = default(TKey);
        string keyValue;
        _value.TryGetValue(ins.GetFirstKeyName(), out keyValue);
        if (typeof(TKey) == typeof(int)) {
            int tempValue = 0;
            int.TryParse(keyValue, out tempValue);
            key = (TKey)Convert.ChangeType(tempValue, typeof(TKey));
        }
        else if (typeof(TKey) == typeof(string)) {
            key = (TKey)Convert.ChangeType(keyValue, typeof(TKey));
        }
        else if (typeof(TKey).IsEnum) {
            key = (TKey)Enum.Parse(typeof(TKey), keyValue, true);
        }

        ins.LoadByLine(_value, _line);
        ins.ClearData();


        if (_table.ContainsKey(key))
            _table[key] = ins;
        else
            _table.Add(key, ins);
    }
}
