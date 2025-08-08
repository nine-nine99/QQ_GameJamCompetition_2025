using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace HW.HWEditor {
    [CustomEditor(typeof(BaseWindow), true)]
    public class BaseWindowInspector : Editor {
        BaseWindow baseWindow;

        private void OnEnable() {
            baseWindow = (BaseWindow)target;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (!baseWindow.gameObject.name.Equals("Win_" + baseWindow.GetType().Name)) {
                EditorGUILayout.HelpBox($"Window Name Error,Please Check！", MessageType.Error);
            }
        }
    }
}