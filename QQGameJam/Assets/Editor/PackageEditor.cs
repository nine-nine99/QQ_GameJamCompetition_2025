using System;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace HW.HWEditor {
    public class PackageEditor : IPreprocessBuildWithReport {
        static RemoveRequest s_RemRequest;
        static Queue<string> s_pkgNameQueue;

        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report) {
#if UNITY_ANDROID || UNITY_IOS
            StartRemovingBadPackages();
#endif
        }

        static public void StartRemovingBadPackages() {
            s_pkgNameQueue = new Queue<string>();
            s_pkgNameQueue.Enqueue("com.unity.ads");

            // callback for every frame in the editor
            EditorApplication.update += PackageRemovalProgress;
            EditorApplication.LockReloadAssemblies();

            var nextRequestStr = s_pkgNameQueue.Dequeue();
            s_RemRequest = Client.Remove(nextRequestStr);

            return;
        }


        static void PackageRemovalProgress() {
            if (s_RemRequest.IsCompleted) {
                switch (s_RemRequest.Status) {
                    case StatusCode.Failure:    // couldn't remove package
                        Debug.Log("Couldn't remove package '" + s_RemRequest.PackageIdOrName + "': " + s_RemRequest.Error.message);
                        break;

                    case StatusCode.InProgress:
                        break;

                    case StatusCode.Success:
                        Debug.Log("Removed package: " + s_RemRequest.PackageIdOrName);
                        break;
                }

                if (s_pkgNameQueue.Count > 0) {
                    var nextRequestStr = s_pkgNameQueue.Dequeue();
                    Debug.Log("Requesting removal of '" + nextRequestStr + "'.");
                    s_RemRequest = Client.Remove(nextRequestStr);

                }
                else {    // no more packages to remove
                    EditorApplication.update -= PackageRemovalProgress;
                    EditorApplication.UnlockReloadAssemblies();
                }
            }

            return;
        }
    }
}
