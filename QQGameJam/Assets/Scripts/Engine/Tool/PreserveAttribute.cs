using System;
using UnityEngine.Scripting;

/// <summary>
/// 强制保留类和派生类不被剔除
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class NeedsPreserveAttribute : Attribute {
}