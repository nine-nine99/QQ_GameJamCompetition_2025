using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HWGames.Bundles.RateGame {
    public class HWRateMgr {

        private const string RoutinesClassPath = "com.aar.rate.RateUtils";

        /// <summary>
        /// 应用内点赞，一个APP只会触发一次
        /// </summary>
        public static void ShowRate() {
#if UNITY_IOS && !UNITY_EDITOR
        UnityStoreKit storeKit = new UnityStoreKit();
        storeKit.GoToCommnet();
#elif UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(RoutinesClassPath);
        if (pluginClass != null) {
            pluginClass.CallStatic("ShowRate");
        }
#endif
        }

        /// <summary>
        /// 跳转商店点赞,无次数限制
        /// </summary>
        public static void ShowJumpRate() {
#if UNITY_IOS && !UNITY_EDITOR
        UnityStoreKit storeKit = new UnityStoreKit();
        storeKit.GoToCommnet();
#elif UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(RoutinesClassPath);
        if (pluginClass != null) {
            pluginClass.CallStatic("ShowJumpRate");
        }
#endif
        }
    }
}
