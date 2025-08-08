using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashCallBack : MonoBehaviour {

    private void Awake() {
        Debug.Log("¡¾CrashCallBack :" + "Try InitCarshCallBack");
        Init();
    }
    public void CarshInitCallBack() {
        Debug.Log("¡¾CrashCallBack :" + "InitCarshCallBack");
    }

    public void CarshCallBack() {
        LocalSave.SaveAll();
        Debug.Log("¡¾CrashCallBack :" + "CarshCallBack");
    }

    public void CarshHandle() {
        LocalSave.SaveAll();
        Debug.Log("¡¾CrashCallBack :" + "CarshHandle");
    }
    public void Init() {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass("com.hw.carshcatch.CrashHandler");
        if (pluginClass != null) {
            pluginClass.CallStatic("Init");
        }
#endif
    }

}
