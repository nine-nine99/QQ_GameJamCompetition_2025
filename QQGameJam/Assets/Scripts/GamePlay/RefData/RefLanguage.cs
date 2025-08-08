using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;

public class RefLanguage : RefBase {

    //语言值
    public static Dictionary<string, RefLanguage> cacheMap = new Dictionary<string, RefLanguage>();

    public string Key;
    public string CN;
    public string DE;
    public string ES;
    public string FR;
    public string ID;
    public string IT;
    public string JA;
    public string KO;
    public string PT;
    public string RU;
    public string TH;
    public string TR;
    public string VI;
    public string ZH_TW;

    public override string GetFirstKeyName() {
        return "Key";
    }

    public override void LoadByLine(Dictionary<string, string> _value, int _line) {
        base.LoadByLine(_value, _line);
        Key = GetString("Key");
        CN = GetString("CN");
        DE = GetString("DE");
        ES = GetString("ES");
        FR = GetString("FR");
        ID = GetString("ID");
        IT = GetString("IT");
        JA = GetString("JA");
        KO = GetString("KO");
        PT = GetString("PT");
        RU = GetString("RU");
        TH = GetString("TH");
        TR = GetString("TR");
        VI = GetString("VI");
        ZH_TW = GetString("ZH_TW");
    }

    //获得翻译后的值
    public static string GetValue(string _key, bool isFormat = false) {
        if (string.IsNullOrEmpty(_key))
            return "";

        RefLanguage data = null;
        string value = "";
        if (cacheMap.TryGetValue(_key, out data)) {
            switch (LanguageMgr.Instance.CurLang) {
                case SystemLanguage.ChineseSimplified:
                    value = data.CN.Replace("\\n", "\n");
                    break;
                case SystemLanguage.ChineseTraditional:
                    value = data.ZH_TW.Replace("\\n", "\n");
                    break;
                case SystemLanguage.German:
                    value = data.DE.Replace("\\n", "\n");
                    break;
                case SystemLanguage.Spanish:
                    value = data.ES.Replace("\\n", "\n");
                    break;
                case SystemLanguage.French:
                    value = data.FR.Replace("\\n", "\n");
                    break;
                case SystemLanguage.Indonesian:
                    value = data.ID.Replace("\\n", "\n");
                    break;
                case SystemLanguage.Italian:
                    value = data.IT.Replace("\\n", "\n");
                    break;
                case SystemLanguage.Japanese:
                    value = data.JA.Replace("\\n", "\n");
                    break;
                case SystemLanguage.Korean:
                    value = data.KO.Replace("\\n", "\n");
                    break;
                case SystemLanguage.Portuguese:
                    value = data.PT.Replace("\\n", "\n");
                    break;
                case SystemLanguage.Russian:
                    value = data.RU.Replace("\\n", "\n");
                    break;
                case SystemLanguage.Thai:
                    value = data.TH.Replace("\\n", "\n");
                    break;
                case SystemLanguage.Turkish:
                    value = data.TR.Replace("\\n", "\n");
                    break;
                case SystemLanguage.Vietnamese:
                    value = data.VI.Replace("\\n", "\n");
                    break;
                default:
                    value = data.Key.Replace("\\n", "\n");
                    break;
            }
        }
        if (string.IsNullOrEmpty(value) == false)
            return value;

        if (data == null) {
            //Debug.Log("error RefLanguage key:" + _key);
        }
        return _key;
    }

    public static string GetValueParam(string _key, params object[] _obj) {
        string ret = GetValue(_key, true);
        if (null == ret || _obj == null || _obj.Length == 0) {
            return ret;
        }
        // 字符串替换
        string des = string.Format(ret, _obj);
        return des;
    }
}