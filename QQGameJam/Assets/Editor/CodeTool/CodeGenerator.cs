using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace HW.EditorTools {
    public class CodeGenerator {
        private static string systemTemplatePath = "Assets/Editor/CodeTool/ScriptTemplates";

        [MenuItem("Assets/Create/C# Singleton Script", false, 20)]
        public static void CreateSingletonCS() {
            string ScriptName = "SingletonScript_NewBehaviourScript";
            CreateSystemCSScriptAsset.className = ScriptName;
            //参数为传递给CreateEventCSScriptAsset类action方法的参数 
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateSystemCSScriptAsset>(), GetSelectPathOrFallback() + "/New Script.cs", null, systemTemplatePath + $"/{ScriptName}.cs");
        }

        [MenuItem("Assets/Create/C# Singleton Mono Script", false, 21)]
        public static void CreateSystemCS() {
            string ScriptName = "SingletonMonoScript_NewBehaviourScript";
            CreateSystemCSScriptAsset.className = ScriptName;
            //参数为传递给CreateEventCSScriptAsset类action方法的参数 
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateSystemCSScriptAsset>(), GetSelectPathOrFallback() + "/New Script.cs", null, systemTemplatePath + $"/{ScriptName}.cs");
        }

        [MenuItem("Assets/Create/C# BaseWindow Script", false, 22)]
        public static void CreateWindowCS() {
            string ScriptName = "WindowScript_NewBehaviourScript";
            CreateSystemCSScriptAsset.className = ScriptName;
            //参数为传递给CreateEventCSScriptAsset类action方法的参数 
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateSystemCSScriptAsset>(), GetSelectPathOrFallback() + "/New Script.cs", null, systemTemplatePath + $"/{ScriptName}.cs");
        }

        [MenuItem("Assets/Create/C# Class Script", false, 23)]
        public static void CreateClassCS() {
            string ScriptName = "ClassScript_NewBehaviourScript";
            CreateSystemCSScriptAsset.className = ScriptName;
            //参数为传递给CreateEventCSScriptAsset类action方法的参数 
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateSystemCSScriptAsset>(), GetSelectPathOrFallback() + "/New Script.cs", null, systemTemplatePath + $"/{ScriptName}.cs");
        }


        public static string GetSelectPathOrFallback() {
            string path = "Assets"; //遍历选中的资源以获得路径 
                                    //Selection.GetFiltered是过滤选择文件或文件夹下的物体，assets表示只返回选择对象本身
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets)) {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path)) { path = Path.GetDirectoryName(path); break; }
            }
            return path;
        }
    }

    //要创建模板文件必须继承EndNameEditAction，重写action方法 
    class CreateSystemCSScriptAsset : EndNameEditAction {
        public static string className = "";
        static string pattern = "SystemClass";

        public override void Action(int instanceId, string pathName, string resourceFile) { //创建资源 
            Object obj = CreateScriptAssetFromTemplate(pathName, resourceFile); ProjectWindowUtil.ShowCreatedAsset(obj);
            //高亮显示资源 
        }

        private static Object CreateScriptAssetFromTemplate(string pathName, string resourceFile) { //获取要创建资源的绝对路径 
            string fullPath = Path.GetFullPath(pathName);
            //读取本地的模板文件
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd(); streamReader.Close();
            //获取文件名，不含扩展名 
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            Debug.Log("text===" + text);
            //将模板类中的类名替换成你创建的文件名 
            text = Regex.Replace(text, pattern, fileNameWithoutExtension);
            text = Regex.Replace(text, className, fileNameWithoutExtension);
            //参数指定是否提供 Unicode 字节顺序标记 
            bool encoderShouldEmitUTF8Identifier = true;
            //是否在检测到无效的编码时引发异常
            bool throwOnInvalidBytes = false;
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;
            //写入文件 
            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding); streamWriter.Write(text);
            streamWriter.Close();
            //刷新资源管理器
            AssetDatabase.ImportAsset(pathName);
            AssetDatabase.Refresh(); return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
        }
    }
}

