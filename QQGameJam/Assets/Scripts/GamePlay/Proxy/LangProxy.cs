using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LangProxy : MonoBehaviour {
    public string key = "";

    [HideInInspector]
    public string langKey;
    [HideInInspector]
    public object[] langValue;
    [HideInInspector]
    public bool isDynamic = false;

    private Text txt;

    void Awake() {
        txt = gameObject.GetComponent<Text>();
        if (txt == null) {
            return;
        }
        if (!isDynamic) {
            langKey = string.IsNullOrEmpty(key) ? txt.text : key;
            if (langValue == null) {
                txt.SetText(langKey);
            }
            else {
                txt.SetTextFormat(langKey, langValue);
            }
        }
        Send.RegisterMsg(SendType.LangChange, OnLangChange);
    }

    private void OnLangChange(object[] objs) {
        if (langValue == null) {
            txt.SetText(langKey);
        }
        else {
            txt.SetTextFormat(langKey, langValue);
        }
    }
}