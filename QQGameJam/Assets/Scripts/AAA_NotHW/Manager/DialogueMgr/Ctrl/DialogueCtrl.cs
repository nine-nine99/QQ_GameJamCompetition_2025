using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueCtrl : SingletonMonoBehavior<DialogueCtrl>
{
    [Header("对话框父物体")]
    public Transform Parent;
    [Header("对话框对话事件触发")]
    public Button dialogueImage;

    // 背景，左右两个人物，对话框中的名字，对话框中的对话
    [Header("背景")]
    public Image BG;
    [Header("人物左")]
    public Image Charactor_0;
    [Header("人物右")]
    public Image Charactor_1;
    [Header("对话框中的名字")]
    public TextMeshProUGUI txt_name;
    [Header("对话框中的对话")]
    public TextMeshProUGUI txt_dialogue;

    [Header("当前的对话SO文件")]
    public DialogueData_SO Dia_SO;
    [Header("FadePanel渐变面板")]
    public Transform FadePanel;
    private CanvasGroup Fade => FadePanel.GetComponent<CanvasGroup>();
    private int CurLine_Index;

    [Header("渐变设置")]
    public float fadeDuration = 0.8f; // 渐变持续时间
    private System.Action onDialogueCompleteCallback;

    private void OnEnable()
    {
        // OpenDialogueScene(1000);

    }

    // NOTE:打开对话框
    public void OpenDialogueScene(int ID, System.Action onComplete = null)
    {
        Dia_SO = RefDialogues.GetDialogueData_SOByID(ID);

        // 保存回调函数
        onDialogueCompleteCallback = onComplete;

        Init_Dialogue();
    }

    // 初始化对话
    private void Init_Dialogue()
    {
        // 启用对话框
        Parent.gameObject.SetActive(true);
        // 初始化对话Index
        CurLine_Index = 0;
        // 刷新对话框
        Refesh();

        // 添加渐变淡入效果
        FadeIn();

        // 添加点击事件监听
        if (dialogueImage != null)
        {
            dialogueImage.onClick.AddListener(OnImageClicked);
        }
    }

    // 当对话框被点击时
    private void OnImageClicked()
    {
        // Debug.Log("玩家点击了对话图片!");

        // 每次点击跳到下一段对话

        // 在这里添加你的点击逻辑
        // Send.SendMsg(SendType.DialogueImageClicked);

        CurLine_Index++;
        if (CurLine_Index >= Dia_SO.lineDetails.Count)
        {
            // 对话结束
            CurLine_Index--;
            // 触发对话结束事件
            OnDialogueEnd();
            return;
        }
        Refesh();
    }

    private void OnDialogueEnd()
    {
        // 先执行淡出，完成后关闭对话框
        FadeOut(() =>
        {
            // 取消监听器
            if (dialogueImage != null)
            {
                dialogueImage.onClick.RemoveListener(OnImageClicked);
            }

            // 停用对话框
            Parent.gameObject.SetActive(false);

            // 执行回调函数
            onDialogueCompleteCallback?.Invoke();

            // 清空回调引用
            onDialogueCompleteCallback = null;

            // 上面的运行都结束后触发淡入
            FadeIn();
        });
    }


    // 刷新对话场景
    private void Refesh()
    {
        LineDetail CurLine = Dia_SO.lineDetails[CurLine_Index];
        // 背景
        BG.sprite = RefImage.GetSpriteByID(CurLine.BGID);
        // 背景
        Charactor_0.sprite = RefImage.GetSpriteByID(CurLine.CharactorID_0);
        // 背景
        Charactor_1.sprite = RefImage.GetSpriteByID(CurLine.CharactorID_1);
        // 对话框名字
        txt_name.text = CurLine.txt_name;
        // 对话框中的对话
        txt_dialogue.text = CurLine.txt_dialogue;
    }

    // 淡入效果（从黑屏到透明）
    public void FadeIn(System.Action onComplete = null)
    {
        StartCoroutine(FadeCoroutine(1f, 0f, onComplete));
    }

    // 淡出效果（从透明到黑屏）
    public void FadeOut(System.Action onComplete = null)
    {
        StartCoroutine(FadeCoroutine(0f, 1f, onComplete));
    }

    // 渐变协程
    private IEnumerator FadeCoroutine(float startAlpha, float targetAlpha, System.Action onComplete)
    {
        // 激活渐变面板
        FadePanel.gameObject.SetActive(true);

        // 设置初始透明度
        Fade.alpha = startAlpha;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration;

            // 使用平滑插值
            Fade.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);

            yield return null;
        }

        // 确保最终值准确
        Fade.alpha = targetAlpha;

        // 如果淡入完成，隐藏面板以避免阻挡交互
        if (targetAlpha <= 0f)
        {
            FadePanel.gameObject.SetActive(false);
        }

        // 执行完成回调
        onComplete?.Invoke();
    }

    // 场景切换时的渐变效果（可在对话过程中切换背景）
    public void ChangeSceneWithFade(int newBGID)
    {
        FadeOut(() =>
        {
            // 在黑屏时切换背景
            BG.sprite = RefImage.GetSpriteByID(newBGID);
            // 然后淡入新场景
            FadeIn();
        });
    }
    // 场景切换时的渐变效果（可以自定义场景切换效果）
    public void ChangeSceneWithFade(System.Action onComplete = null)
    {
        FadeOut(() =>
        {
            // 自定义场景切换效果
            onComplete?.Invoke();
            // 然后淡入新场景
            FadeIn();
        });
    }
    private void OnDestroy()
    {
        // 清理监听器
        if (dialogueImage != null)
        {
            dialogueImage.onClick.RemoveListener(OnImageClicked);
        }
    }
}
