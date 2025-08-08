using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace HWGames.Bundles.Internal {
    public class HWCoDelegator : SingletonMonoBehavior<HWCoDelegator> {
        // 开始协程
        public static Coroutine Coroutine(IEnumerator routine) {
            return Instance.StartCoroutine(routine);
        }

        // 停止协程
        public static void StopCoroutineEx(IEnumerator routine) {
            Instance.StopCoroutine(routine);
        }

        public static void StopCoroutineEx(Coroutine routine) {
            Instance.StopCoroutine(routine);
        }

        public static void StopCoroutineEx(string _routine) {
            Instance.StopCoroutine(_routine);
        }
    }
}