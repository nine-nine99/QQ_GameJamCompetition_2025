using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

[InitializeOnLoad]
public class PreserveChecker {
    static PreserveChecker() {
        // 注册到 Unity 的构建前事件
        BuildPlayerWindow.RegisterBuildPlayerHandler(CheckAndAddPreserveBeforeBuild);
    }

    // 在构建前执行检查
    private static void CheckAndAddPreserveBeforeBuild(BuildPlayerOptions options) {
        // 获取所有程序集
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies) {
            // 获取所有标记了 [NeedsPreserve] 特性的类
            var types = assembly.GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(NeedsPreserveAttribute)))
                .ToList();

            foreach (var type in types) {
                // 检查类是否包含 [Preserve] 特性
                if (!Attribute.IsDefined(type, typeof(UnityEngine.Scripting.PreserveAttribute))) {
                    // 自动为没有 Preserve 特性的类添加 [Preserve]
                    AddPreserveAttributeToClass(type);
                }
            }
        }

        // 执行默认的构建流程
        BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
    }

    // 添加 Preserve 特性到类
    private static void AddPreserveAttributeToClass(Type type) {
        // 查找包含该类型的 C# 文件
        string[] guids = AssetDatabase.FindAssets($"{type.Name} t:script");

        if (guids.Length == 0) {
            Debug.LogWarning($"未找到类 {type.Name} 对应的脚本文件，跳过 Preserve 添加");
            return;
        }
        // 获取文件路径
        string filePath = AssetDatabase.GUIDToAssetPath(guids[0]);

        if (string.IsNullOrEmpty(filePath)) {
            Debug.LogWarning($"无法找到 {type.Name} 的源文件，跳过 Preserve 添加");
            return;
        }
        // 读取文件内容
        string fileContent = File.ReadAllText(filePath);
        // 查找类声明的位置
        string classDeclaration = $"class {type.Name}";
        // 查找类声明在文件中的位置
        int classPosition = fileContent.IndexOf(classDeclaration);
        if (classPosition == -1) {
            Debug.LogWarning($"无法找到类 {type.Name} 的声明，跳过 Preserve 添加");
            return;
        }
        // 查找类声明的上一行，以便插入特性
        int insertPosition = fileContent.LastIndexOf("\n", classPosition);  // 查找类声明前的换行
        if (insertPosition == -1) {
            Debug.LogWarning($"无法定位类 {type.Name} 声明前的换行，跳过 Preserve 添加");
            return;
        }
        // 插入 Preserve 特性
        string preserveAttribute = "[UnityEngine.Scripting.Preserve]";
        fileContent = fileContent.Insert(insertPosition + 1, preserveAttribute + "\n");
        // 保存修改后的文件
        File.WriteAllText(filePath, fileContent);
        AssetDatabase.Refresh();
        Debug.Log($"自动为 {type.Name} 类添加了 [Preserve] 特性");
    }
}
