using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ConflictResolutionMode
{
    Swap,           // 交换按键
    AutoAssign,     // 自动分配可用按键
    CancelAndWarn   // 取消设置并警告
}

public class SetMenuControl : SingletonMonoBehavior<SetMenuControl>
{
    [Header("按钮设置")]
    public List<Button> Btns = new List<Button>();
    [Header("重置按钮")]
    public Button ResetBtn;
    [Header("按键配置")]
    [SerializeField] private string[] keyNames = { "", "", "", "", "", "" };
    [SerializeField]
    private KeyCode[] defaultKeys = {
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y
    };

    [Header("设置")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color waitingColor = Color.yellow;
    [SerializeField] private Color conflictColor = Color.red;
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private float waitingTimeout = 5f; // 等待输入超时时间

    [Header("冲突处理设置")]
    [SerializeField] private ConflictResolutionMode conflictMode = ConflictResolutionMode.AutoAssign;
    [SerializeField] private bool showConflictMessages = true;

    // 当前按键配置
    private KeyCode[] currentKeys;
    private int waitingForKeyIndex = -1; // 当前等待输入的按钮索引
    private Coroutine waitingCoroutine;

    // UI组件缓存
    private TextMeshProUGUI[] buttonTexts;
    private Image[] buttonImages;

    // 按键设置保存的键名
    private readonly string SAVE_KEY_PREFIX = "GameKey_";

    private void Start()
    {
        InitializeKeySettings();
        SetupButtons();
        LoadKeySettings();
        UpdateButtonDisplays();

        ResetBtn.onClick.AddListener(ResetToDefaultFromInspector);
    }

    public KeyCode GetcurrentKeys(int index)
    {
        return currentKeys[index];
    }

    /// <summary>
    /// 初始化按键设置
    /// </summary>
    private void InitializeKeySettings()
    {
        // 确保数组长度一致
        if (Btns.Count != keyNames.Length || keyNames.Length != defaultKeys.Length)
        {
            Debug.LogError("按钮数量、按键名称和默认按键数量不匹配！");
            return;
        }

        // 初始化当前按键配置
        currentKeys = new KeyCode[defaultKeys.Length];
        System.Array.Copy(defaultKeys, currentKeys, defaultKeys.Length);

        // 缓存UI组件
        CacheUIComponents();
    }

    /// <summary>
    /// 缓存UI组件
    /// </summary>
    private void CacheUIComponents()
    {
        buttonTexts = new TextMeshProUGUI[Btns.Count];
        buttonImages = new Image[Btns.Count];

        for (int i = 0; i < Btns.Count; i++)
        {
            if (Btns[i] != null)
            {
                // 获取按钮图片组件
                buttonImages[i] = Btns[i].GetComponent<Image>();

                // 获取子物体上的文本组件
                buttonTexts[i] = Btns[i].GetComponentInChildren<TextMeshProUGUI>();

                if (buttonTexts[i] == null)
                {
                    // 如果没有找到TMPro，尝试查找普通Text
                    var legacyText = Btns[i].GetComponentInChildren<Text>();
                    if (legacyText != null)
                    {
                        Debug.LogWarning($"按钮 {i} 使用的是Legacy Text组件，建议使用TextMeshPro");
                    }
                    else
                    {
                        Debug.LogError($"按钮 {i} 的子物体上没有找到文本组件");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 设置按钮事件
    /// </summary>
    private void SetupButtons()
    {
        for (int i = 0; i < Btns.Count; i++)
        {
            if (Btns[i] != null)
            {
                int buttonIndex = i; // 闭包变量
                Btns[i].onClick.AddListener(() => StartKeyBinding(buttonIndex));
            }
        }
    }

    /// <summary>
    /// 开始按键绑定
    /// </summary>
    private void StartKeyBinding(int buttonIndex)
    {
        if (waitingForKeyIndex != -1)
        {
            // 如果已经在等待其他按键输入，先停止
            StopKeyBinding();
        }

        waitingForKeyIndex = buttonIndex;

        // 更新按钮显示
        UpdateButtonDisplay(buttonIndex, "按任意键...", waitingColor);

        // 开始等待输入协程
        waitingCoroutine = StartCoroutine(WaitForKeyInput());

        Debug.Log($"等待为 {keyNames[buttonIndex]} 设置新按键...");
    }

    /// <summary>
    /// 等待按键输入的协程
    /// </summary>
    private IEnumerator WaitForKeyInput()
    {
        float waitTime = 0f;

        while (waitTime < waitingTimeout)
        {
            // 检测按键输入
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    // 检查是否是有效按键（排除鼠标按键等）
                    if (IsValidKey(key))
                    {
                        SetNewKey(waitingForKeyIndex, key);
                        StopKeyBinding();
                        yield break;
                    }
                }
            }

            waitTime += Time.deltaTime;
            yield return null;
        }

        // 超时处理
        Debug.Log("等待按键输入超时");
        StopKeyBinding();
    }

    /// <summary>
    /// 检查是否是有效按键
    /// </summary>
    private bool IsValidKey(KeyCode key)
    {
        // 排除鼠标按键
        if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
            return false;

        // 排除操纵杆按键（可根据需要调整）
        if (key >= KeyCode.JoystickButton0 && key <= KeyCode.Joystick8Button19)
            return false;

        return true;
    }

    /// <summary>
    /// 设置新按键 - 改进版本，防止重复键位
    /// </summary>
    private void SetNewKey(int buttonIndex, KeyCode newKey)
    {
        // 检查是否与其他按键冲突
        int conflictIndex = FindKeyConflict(newKey, buttonIndex);

        if (conflictIndex != -1)
        {
            // 找到冲突，处理冲突
            HandleKeyConflict(buttonIndex, newKey, conflictIndex);
        }
        else
        {
            // 没有冲突，直接设置
            currentKeys[buttonIndex] = newKey;
            Debug.Log($"{keyNames[buttonIndex]} 设置为: {newKey}");
        }

        // 更新显示和保存设置
        UpdateButtonDisplays();
        SaveKeySettings();
    }

    /// <summary>
    /// 查找按键冲突
    /// </summary>
    /// <param name="newKey">要设置的新按键</param>
    /// <param name="excludeIndex">排除的索引（当前正在设置的按钮）</param>
    /// <returns>冲突的按钮索引，-1表示无冲突</returns>
    private int FindKeyConflict(KeyCode newKey, int excludeIndex)
    {
        for (int i = 0; i < currentKeys.Length; i++)
        {
            if (i != excludeIndex && currentKeys[i] == newKey)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 处理按键冲突
    /// </summary>
    /// <param name="newButtonIndex">新按钮索引</param>
    /// <param name="newKey">新按键</param>
    /// <param name="conflictIndex">冲突按钮索引</param>
    private void HandleKeyConflict(int newButtonIndex, KeyCode newKey, int conflictIndex)
    {
        switch (conflictMode)
        {
            case ConflictResolutionMode.Swap:
                SwapKeys(newButtonIndex, conflictIndex, newKey);
                break;

            case ConflictResolutionMode.AutoAssign:
                AssignAvailableKey(newButtonIndex, newKey, conflictIndex);
                break;

            case ConflictResolutionMode.CancelAndWarn:
                CancelKeySettingWithWarning(newButtonIndex, newKey, conflictIndex);
                break;
        }
    }

    /// <summary>
    /// 方案1：交换按键
    /// </summary>
    private void SwapKeys(int newButtonIndex, int conflictIndex, KeyCode newKey)
    {
        KeyCode oldKey = currentKeys[newButtonIndex];
        currentKeys[newButtonIndex] = newKey;
        currentKeys[conflictIndex] = oldKey;

        Debug.Log($"按键冲突：{keyNames[newButtonIndex]} 设置为 {newKey}，{keyNames[conflictIndex]} 改为 {oldKey}");

        if (showConflictMessages)
        {
            StartCoroutine(ShowSwapMessage(newButtonIndex, conflictIndex, newKey, oldKey));
        }
    }

    /// <summary>
    /// 方案2：为冲突按钮分配可用按键
    /// </summary>
    private void AssignAvailableKey(int newButtonIndex, KeyCode newKey, int conflictIndex)
    {
        // 设置新按键
        currentKeys[newButtonIndex] = newKey;

        // 为冲突的按钮找一个可用的按键
        KeyCode availableKey = FindAvailableKey();

        if (availableKey != KeyCode.None)
        {
            currentKeys[conflictIndex] = availableKey;
            Debug.Log($"按键冲突解决：{keyNames[newButtonIndex]} 设置为 {newKey}，{keyNames[conflictIndex]} 自动改为 {availableKey}");

            if (showConflictMessages)
            {
                StartCoroutine(ShowAutoAssignMessage(newButtonIndex, conflictIndex, newKey, availableKey));
            }
        }
        else
        {
            // 如果没有可用按键，回退到交换方案
            SwapKeys(newButtonIndex, conflictIndex, newKey);
        }
    }

    /// <summary>
    /// 方案3：取消设置并警告用户
    /// </summary>
    private void CancelKeySettingWithWarning(int newButtonIndex, KeyCode newKey, int conflictIndex)
    {
        Debug.LogWarning($"按键 {newKey} 已被 {keyNames[conflictIndex]} 使用，请选择其他按键");

        if (showConflictMessages)
        {
            StartCoroutine(ShowConflictWarning(newButtonIndex, newKey, conflictIndex));
        }
    }

    /// <summary>
    /// 查找可用的按键
    /// </summary>
    private KeyCode FindAvailableKey()
    {
        // 定义候选按键列表（按优先级排序）
        KeyCode[] candidateKeys = {
            KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M,
            KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P,
            KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L,
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
            KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0,
            KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4, KeyCode.F5, KeyCode.F6,
            KeyCode.F7, KeyCode.F8, KeyCode.F9, KeyCode.F10, KeyCode.F11, KeyCode.F12
        };

        // 查找未被使用的按键
        foreach (KeyCode candidate in candidateKeys)
        {
            if (!IsKeyInUse(candidate))
            {
                return candidate;
            }
        }

        return KeyCode.None; // 没有可用按键
    }

    /// <summary>
    /// 检查按键是否已被使用
    /// </summary>
    private bool IsKeyInUse(KeyCode key)
    {
        for (int i = 0; i < currentKeys.Length; i++)
        {
            if (currentKeys[i] == key)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 显示按键交换提示
    /// </summary>
    private IEnumerator ShowSwapMessage(int newIndex, int conflictIndex, KeyCode newKey, KeyCode oldKey)
    {
        // 临时改变颜色显示交换效果
        if (buttonTexts[newIndex] != null)
            buttonTexts[newIndex].color = successColor;
        if (buttonTexts[conflictIndex] != null)
            buttonTexts[conflictIndex].color = successColor;

        yield return new WaitForSeconds(1f);

        // 恢复正常颜色
        if (buttonTexts[newIndex] != null)
            buttonTexts[newIndex].color = normalColor;
        if (buttonTexts[conflictIndex] != null)
            buttonTexts[conflictIndex].color = normalColor;
    }

    /// <summary>
    /// 显示自动分配提示
    /// </summary>
    private IEnumerator ShowAutoAssignMessage(int newIndex, int conflictIndex, KeyCode newKey, KeyCode assignedKey)
    {
        // 新设置的按钮显示成功颜色
        if (buttonTexts[newIndex] != null)
            buttonTexts[newIndex].color = successColor;

        // 被重新分配的按钮显示警告颜色
        if (buttonTexts[conflictIndex] != null)
            buttonTexts[conflictIndex].color = waitingColor;

        yield return new WaitForSeconds(2f);

        // 恢复正常颜色
        if (buttonTexts[newIndex] != null)
            buttonTexts[newIndex].color = normalColor;
        if (buttonTexts[conflictIndex] != null)
            buttonTexts[conflictIndex].color = normalColor;
    }

    /// <summary>
    /// 显示冲突警告
    /// </summary>
    private IEnumerator ShowConflictWarning(int buttonIndex, KeyCode conflictKey, int conflictIndex)
    {
        // 显示错误状态
        if (buttonTexts[buttonIndex] != null)
        {
            string originalText = buttonTexts[buttonIndex].text;
            buttonTexts[buttonIndex].text = $"冲突！{conflictKey} 已被{keyNames[conflictIndex]}使用";
            buttonTexts[buttonIndex].color = conflictColor;

            yield return new WaitForSeconds(2f);

            // 恢复并重新开始绑定
            buttonTexts[buttonIndex].color = normalColor;
            StartKeyBinding(buttonIndex);
        }
    }

    /// <summary>
    /// 停止按键绑定
    /// </summary>
    private void StopKeyBinding()
    {
        if (waitingCoroutine != null)
        {
            StopCoroutine(waitingCoroutine);
            waitingCoroutine = null;
        }

        if (waitingForKeyIndex != -1)
        {
            // 恢复按钮显示
            UpdateButtonDisplay(waitingForKeyIndex, GetKeyDisplayName(currentKeys[waitingForKeyIndex]), normalColor);
            waitingForKeyIndex = -1;
        }
    }

    /// <summary>
    /// 更新单个按钮显示
    /// </summary>
    private void UpdateButtonDisplay(int index, string text, Color color)
    {
        if (index < 0 || index >= Btns.Count) return;

        if (buttonTexts[index] != null)
        {
            buttonTexts[index].text = $"{keyNames[index]}: {text}";
            buttonTexts[index].color = color;
        }

        if (buttonImages[index] != null)
        {
            buttonImages[index].color = color;
        }
    }

    /// <summary>
    /// 更新所有按钮显示
    /// </summary>
    private void UpdateButtonDisplays()
    {
        for (int i = 0; i < Btns.Count; i++)
        {
            if (i < currentKeys.Length)
            {
                UpdateButtonDisplay(i, GetKeyDisplayName(currentKeys[i]), normalColor);
            }
        }
    }

    /// <summary>
    /// 获取按键显示名称
    /// </summary>
    private string GetKeyDisplayName(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.LeftArrow: return "←";
            case KeyCode.RightArrow: return "→";
            case KeyCode.UpArrow: return "↑";
            case KeyCode.DownArrow: return "↓";
            case KeyCode.Space: return "空格";
            case KeyCode.Return: return "回车";
            case KeyCode.Escape: return "ESC";
            case KeyCode.LeftShift: return "左Shift";
            case KeyCode.RightShift: return "右Shift";
            case KeyCode.LeftControl: return "左Ctrl";
            case KeyCode.RightControl: return "右Ctrl";
            case KeyCode.LeftAlt: return "左Alt";
            case KeyCode.RightAlt: return "右Alt";
            case KeyCode.Tab: return "Tab";
            case KeyCode.CapsLock: return "大写锁定";
            default: return key.ToString();
        }
    }

    /// <summary>
    /// 保存按键设置
    /// </summary>
    private void SaveKeySettings()
    {
        for (int i = 0; i < currentKeys.Length; i++)
        {
            PlayerPrefs.SetString(SAVE_KEY_PREFIX + i, currentKeys[i].ToString());
        }
        PlayerPrefs.Save();
        Debug.Log("按键设置已保存");
    }

    /// <summary>
    /// 加载按键设置
    /// </summary>
    private void LoadKeySettings()
    {
        for (int i = 0; i < currentKeys.Length; i++)
        {
            string savedKey = PlayerPrefs.GetString(SAVE_KEY_PREFIX + i, "");
            if (!string.IsNullOrEmpty(savedKey))
            {
                if (System.Enum.TryParse(savedKey, out KeyCode loadedKey))
                {
                    currentKeys[i] = loadedKey;
                }
            }
        }
        Debug.Log("按键设置已加载");
    }

    /// <summary>
    /// 重置为默认按键
    /// </summary>
    public void ResetToDefault()
    {
        System.Array.Copy(defaultKeys, currentKeys, defaultKeys.Length);

        // 验证默认配置是否有重复
        if (!ValidateKeyConfiguration())
        {
            Debug.LogError("默认按键配置有重复，请检查 defaultKeys 数组");
            return;
        }

        UpdateButtonDisplays();
        SaveKeySettings();
        Debug.Log("按键设置已重置为默认值");
    }

    /// <summary>
    /// 验证按键配置的有效性
    /// </summary>
    private bool ValidateKeyConfiguration()
    {
        // 检查是否有重复按键
        for (int i = 0; i < currentKeys.Length; i++)
        {
            for (int j = i + 1; j < currentKeys.Length; j++)
            {
                if (currentKeys[i] == currentKeys[j])
                {
                    Debug.LogError($"按键配置无效：{keyNames[i]} 和 {keyNames[j]} 使用相同按键 {currentKeys[i]}");
                    return false;
                }
            }
        }

        // 检查是否有无效按键
        for (int i = 0; i < currentKeys.Length; i++)
        {
            if (currentKeys[i] == KeyCode.None)
            {
                Debug.LogError($"按键配置无效：{keyNames[i]} 没有分配按键");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 获取当前按键配置（供其他脚本调用）
    /// </summary>
    public KeyCode GetKey(int index)
    {
        if (index >= 0 && index < currentKeys.Length)
        {
            return currentKeys[index];
        }
        return KeyCode.None;
    }

    /// <summary>
    /// 获取指定功能的按键
    /// </summary>
    public KeyCode GetKeyByName(string keyName)
    {
        for (int i = 0; i < keyNames.Length; i++)
        {
            if (keyNames[i] == keyName)
            {
                return currentKeys[i];
            }
        }
        return KeyCode.None;
    }

    // NOTE:
    /// <summary>
    /// 检查指定按键是否被按下
    /// </summary>
    public bool IsKeyPressed(int index)
    {
        if (index >= 0 && index < currentKeys.Length)
        {
            return Input.GetKeyDown(currentKeys[index]);
        }
        return false;
    }

    public bool IsKeyHold(int index)
    {
        if (index >= 0 && index < currentKeys.Length)
        {
            return Input.GetKey(currentKeys[index]);
        }
        return false;
    }

    private void OnDestroy()
    {
        // 清理事件监听
        for (int i = 0; i < Btns.Count; i++)
        {
            if (Btns[i] != null)
            {
                Btns[i].onClick.RemoveAllListeners();
            }
        }

        // 停止协程
        if (waitingCoroutine != null)
        {
            StopCoroutine(waitingCoroutine);
        }
    }

    #region 调试和扩展功能

    /// <summary>
    /// 在Inspector中显示重置按钮
    /// </summary>
    [ContextMenu("重置为默认按键")]
    private void ResetToDefaultFromInspector()
    {
        ResetToDefault();
    }

    /// <summary>
    /// 显示当前按键配置
    /// </summary>
    [ContextMenu("显示当前按键配置")]
    private void ShowCurrentKeys()
    {
        Debug.Log("=== 当前按键配置 ===");
        for (int i = 0; i < keyNames.Length; i++)
        {
            if (i < currentKeys.Length)
            {
                Debug.Log($"{keyNames[i]}: {currentKeys[i]}");
            }
        }
    }

    /// <summary>
    /// 检查所有按键是否有重复（调试用）
    /// </summary>
    [ContextMenu("检查按键重复")]
    private void CheckForDuplicateKeys()
    {
        Debug.Log("=== 检查按键重复 ===");

        bool foundDuplicates = false;
        for (int i = 0; i < currentKeys.Length; i++)
        {
            for (int j = i + 1; j < currentKeys.Length; j++)
            {
                if (currentKeys[i] == currentKeys[j])
                {
                    Debug.LogError($"发现重复按键：{keyNames[i]} 和 {keyNames[j]} 都使用 {currentKeys[i]}");
                    foundDuplicates = true;
                }
            }
        }

        if (!foundDuplicates)
        {
            Debug.Log("没有发现重复按键，配置正常");
        }

        Debug.Log("按键重复检查完成");
    }

    /// <summary>
    /// 测试冲突处理模式
    /// </summary>
    [ContextMenu("测试冲突处理")]
    private void TestConflictHandling()
    {
        Debug.Log($"当前冲突处理模式: {conflictMode}");
        Debug.Log("可以在Inspector中修改冲突处理模式进行测试");
    }

    #endregion
}
