using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace HW.HWEditor {
    public class ExtendMethod {

        [MenuItem("Tools/重启编辑器")]
        public static void RefreshTemplates() {
            // 获取当前 Unity 编辑器的进程
            Process currentProcess = Process.GetCurrentProcess();
            string unityPath = currentProcess.MainModule.FileName;

            // 保存当前打开的项目路径
            string projectPath = Directory.GetCurrentDirectory();

            // 启动一个新的 Unity 编辑器进程
            Process.Start(unityPath, "-projectPath \"" + projectPath + "\"");

            // 退出当前的 Unity 编辑器进程
            currentProcess.Kill();
        }

        [MenuItem("Tools/AddMoney")]
        public static void AddMoney() {
            CurrencyMgr.Instance.Gold += 100000;
        }

        [MenuItem("Tools/AddGrade")]
        public static void AddGrade() {
            GradeMgr.Instance.CurGrade += 10;
        }

        [MenuItem("Tools/DeleteKeys")]
        public static void DeleteKeys() {
            LocalSave.DeleteAll();
        }
    }
}