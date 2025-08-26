using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;

public class DialoguePlayer : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public TextMeshProUGUI nameUI;
    public string[] speakers; //说话的人的text
    [TextArea] public string[] lines;
    public PlayableDirector director;
    public float charInterval = 0.03f;

    //颜色
    static readonly Color32 PINK = new Color32(255, 105, 180, 255);

    int index = 0;
    bool typing = false;
    Coroutine co;
    int currentTotalChars = 0;

    //animator
    public Animator animA; // 旧动画所在对象
    public Animator animB; // 新动画所在对象
    [SerializeField] public int changeAnimationInt; //在第几句话打断，从旧动画换新动画,数字-1

    void Start()
    {
        if (!textUI || lines == null || lines.Length == 0) return;
        PlayCurrent();
    }

    public void OnClickDialogueBox()
    {
        // Debug.Log("yes");
        if (!textUI)
        {
            Debug.Log("no text");
            return;
        }

        if (typing)
        {
            if (co != null) StopCoroutine(co);
            textUI.ForceMeshUpdate();
            textUI.maxVisibleCharacters = currentTotalChars;
            typing = false;
            return;
        }

        index++;
        if (index < lines.Length) PlayCurrent();
        else EndDialogue();
    }

    void PlayCurrent()
    {
        //如果是melody说话，变粉色。
        string spk = (speakers != null && index < speakers.Length) ? speakers[index] : "";
        if (nameUI)
        {
            nameUI.text = spk;
            nameUI.color = string.Equals(spk, "melody", System.StringComparison.OrdinalIgnoreCase)
                           ? (Color)PINK : Color.black;
        }

        if (index == changeAnimationInt)
        {
            if (animA) animA.gameObject.SetActive(false);
            if (animA) animB.gameObject.SetActive(true);
        }

        if (co != null) StopCoroutine(co);
        co = StartCoroutine(TypeLine(lines[index]));
    }

    IEnumerator TypeLine(string line)
    {
        typing = true;
        textUI.text = line;
        textUI.ForceMeshUpdate();
        currentTotalChars = textUI.textInfo.characterCount;
        textUI.maxVisibleCharacters = 0;

        for (int i = 0; i <= currentTotalChars; i++)
        {
            textUI.maxVisibleCharacters = i;
            yield return new WaitForSeconds(charInterval);
        }

        typing = false;
        co = null;
    }

    void EndDialogue()
    {
        // index = 0;
        DialogueMgr.Instance.isDialogueEnd = true;
        transform.parent.gameObject.SetActive(false);

        // 调用全局 DialogueMgr 的结束逻辑
        DialogueMgr.Instance.EndDialogue();
        // index = 0;
        // DialogueMgr.Instance.isDialogueEnd = true;

        // Debug.Log(transform.parent.gameObject);

        
        // if (director) director.Play();
    }
}
