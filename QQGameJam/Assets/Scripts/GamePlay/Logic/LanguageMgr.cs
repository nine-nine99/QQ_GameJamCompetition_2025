using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageMgr : Singleton<LanguageMgr> {
    //当前多语言的值
    private const string CUR_LANGUAGE_KEY = "CurLanguage";
    private SystemLanguage curLang = SystemLanguage.English;
    public SystemLanguage CurLang {
        get {
            return curLang;
        }
        set {
            curLang = value;
            LocalSave.SetInt(CUR_LANGUAGE_KEY, (int)CurLang);
            //当语言改变，通知所有的文本
            Send.SendMsg(SendType.LangChange);
        }
    }

    //语言种类
    public Dictionary<SystemLanguage, string> languageDic = new Dictionary<SystemLanguage, string>();

    public void Init() {
        languageDic.Add(SystemLanguage.English, "English");
        languageDic.Add(SystemLanguage.ChineseSimplified, "简体中文");
        languageDic.Add(SystemLanguage.ChineseTraditional, "繁體中文");
        languageDic.Add(SystemLanguage.German, "Deutsch");
        languageDic.Add(SystemLanguage.Spanish, "Español");
        languageDic.Add(SystemLanguage.French, "français");
        languageDic.Add(SystemLanguage.Indonesian, "Bahasa Indonesia");
        languageDic.Add(SystemLanguage.Italian, "Italiano");
        languageDic.Add(SystemLanguage.Japanese, "日本語");
        languageDic.Add(SystemLanguage.Korean, "한국인");
        languageDic.Add(SystemLanguage.Portuguese, "português");
        languageDic.Add(SystemLanguage.Russian, "русский");
#if UNITY_ANDROID
        languageDic.Add(SystemLanguage.Thai, "ไทย");
#endif
        languageDic.Add(SystemLanguage.Turkish, "Türk");
        languageDic.Add(SystemLanguage.Vietnamese, "Tiếng Việt");

        if (!LocalSave.HasKey(CUR_LANGUAGE_KEY)) {
            SystemLanguage lang = Application.systemLanguage;
            lang = lang == SystemLanguage.Chinese ? SystemLanguage.ChineseSimplified : lang;
            if (!languageDic.ContainsKey(lang)) {
                lang = SystemLanguage.English;
            }
            CurLang = lang;
        }
        else {
            CurLang = (SystemLanguage)LocalSave.GetInt(CUR_LANGUAGE_KEY, (int)SystemLanguage.English);
        }
        InitMsg();
    }

    public void Clear() {
        ClearMsg();
    }

    public void InitMsg() {
    }

    public void ClearMsg() {
    }

    //切语言接口
    public void SwitchLanguage(SystemLanguage language) {
        CurLang = language;
    }
}
