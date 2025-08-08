using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HWGames.HWEditor.Tool {
    public class CopyPathEditor : MonoBehaviour {

        [MenuItem("GameObject/Copy Path/Copy Path", false, 1500)]
        private static void CopyPath() {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null) {
                Debug.LogWarning("请先选中一个物体！");
                return;
            }
            string path = GetHierarchyPath(selectedObject.transform);
            if (path == null) {
                path = selectedObject.gameObject.name;
            }
            string result = $"{path}";
            EditorGUIUtility.systemCopyBuffer = result;
            Debug.Log($"路径已复制到剪贴板: {result}");
        }

        [MenuItem("GameObject/Copy Path/Copy Path", true)]
        private static bool ValidateCopyPath() {
            return Selection.activeGameObject != null;
        }

        [MenuItem("GameObject/Copy Path/Copy Name", false, 0)]
        private static void CopyPathName() {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null) {
                Debug.LogWarning("请先选中一个物体！");
                return;
            }
            string path = selectedObject.gameObject.name;
            string result = $"{path}";
            EditorGUIUtility.systemCopyBuffer = result;
            Debug.Log($"名称已复制到剪贴板: {result}");
        }

        [MenuItem("GameObject/Copy Path/Copy Name", true)]
        private static bool ValidateCopyPathName() {
            return Selection.activeGameObject != null;
        }

        [MenuItem("CONTEXT/Component/Copy Path/Copy Full", false, -1000)]
        private static void CopyPathFull(MenuCommand command) {
            Component selectedComponent = command.context as Component;
            if (selectedComponent == null) {
                Debug.LogWarning("请先选中一个物体！");
                return;
            }
            string path = GetHierarchyPath(selectedComponent.gameObject.transform);
            string name = selectedComponent.gameObject.name;
            if (path == null) {
                name = null;
            }
            Type type = selectedComponent.GetType();
            string result = GetComponentScript(type, path, name);
            EditorGUIUtility.systemCopyBuffer = result;
            Debug.Log($"路径和类型已复制到剪贴板: {result}");
        }

        [MenuItem("CONTEXT/Component/Copy Path/Copy Simple", false, -1000)]
        private static void CopyPathSimple(MenuCommand command) {
            Component selectedComponent = command.context as Component;
            if (selectedComponent == null) {
                Debug.LogWarning("请先选中一个物体！");
                return;
            }
            string path = GetHierarchyPath(selectedComponent.gameObject.transform);
            string name = selectedComponent.gameObject.name;
            if (path == null) {
                name = null;
            }
            Type type = selectedComponent.GetType();
            string result = GetComponentScript(type, path);
            EditorGUIUtility.systemCopyBuffer = result;
            Debug.Log($"路径和类型已复制到剪贴板: {result}");
        }

        [MenuItem("CONTEXT/Component/Copy Path/Copy Self", false, -1000)]
        private static void CopyPathSelf(MenuCommand command) {
            Component selectedComponent = command.context as Component;
            if (selectedComponent == null) {
                Debug.LogWarning("请先选中一个物体！");
                return;
            }
            string name = selectedComponent.gameObject.name;
            Type type = selectedComponent.GetType();
            string result = GetComponentScript(type);
            EditorGUIUtility.systemCopyBuffer = result;
            Debug.Log($"路径和类型已复制到剪贴板: {result}");
        }

        private static string GetHierarchyPath(Transform obj) {
            StringBuilder path = new StringBuilder();
            Transform root = obj;
            Transform lastObj = obj;
            if (root.parent == null) {
                return null;
            }
            while (root.parent != null) {
                lastObj = root;
                root = root.parent;
            }
            if (root.gameObject.name.Equals("Canvas (Environment)")) {
                root = lastObj;
            }
            while (obj != root) {
                path.Insert(0, obj.name + "/");
                obj = obj.parent;
            }
            return path.ToString().TrimEnd('/');
        }

        public static string GetComponentScript(Type type, string path = null, string name = null) {
            string shortTypeName = GetShortTypeName(type.ToString());
            string componentAccess = (type == typeof(GameObject) && path != null)
                ? $"gameObject.GetChildControl<Transform>(\"{path}\")"
                : (path != null)
                    ? $"gameObject.GetChildControl<{shortTypeName}>(\"{path}\")"
                    : $"gameObject.GetComponent<{shortTypeName}>()";

            if (!string.IsNullOrEmpty(name)) {
                return $"{name} = {componentAccess};";
            }
            return componentAccess + ";";
        }

        private static string GetShortTypeName(string fullTypeName) {
            int lastDotIndex = fullTypeName.LastIndexOf('.');
            if (lastDotIndex >= 0) {
                return fullTypeName.Substring(lastDotIndex + 1);
            }
            return fullTypeName;
        }

    }
}