using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

#if UNITY_IPHONE || UNITY_IOS
public class UnityStoreKit {

    [DllImport("__Internal")]
    private static extern void _goComment();

    public void GoToCommnet() {
#if UNITY_IOS && !UNITY_EDITOR
        _goComment();
#endif
    }

}
#endif