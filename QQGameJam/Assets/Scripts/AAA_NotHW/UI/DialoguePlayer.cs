using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;

public class DialoguePlayer : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    [TextArea] public string[] lines;
    public PlayableDirector director;
    public float charInterval = 0.03f;

    int idx = 0;
    bool typing = false;
    Coroutine co;
    int currentTotalChars = 0;

    void Start()
    {
        if (!textUI || lines == null || lines.Length == 0) return;
        PlayCurrent();
    }

    public void OnClickDialogueBox()
    {
        Debug.Log("yes");
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

        idx++;
        if (idx < lines.Length) PlayCurrent();
        else EndDialogue();
    }

    void PlayCurrent()
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(TypeLine(lines[idx]));
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
        gameObject.SetActive(false);
        if (director) director.Play();
    }
}
