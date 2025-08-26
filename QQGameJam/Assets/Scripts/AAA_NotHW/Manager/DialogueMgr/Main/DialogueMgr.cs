using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMgr : SingletonMonoBehavior<DialogueMgr>
{
    public List<GameObject> Dialogues;
    public bool isDialogueEnd = false;
    private int curIndex;
    public System.Action onDialogueEnd;
    private void OnEnable()
    {
        Send.RegisterMsg(SendType.Into_Conversation, OnIntoConversation);
    }
    private void OnDisable()
    {
        // 可以在这里清理消息注册
        Send.UnregisterMsg(SendType.Into_Conversation, OnIntoConversation);
    }

    private void Update()
    {
        Send.SendMsg(SendType.Over_Conversation, curIndex);
        isDialogueEnd = false;
    }

    public void OnIntoConversation(params object[] data)
    {
        int index = (int)data[0];
        if (index < 0 || index >= Dialogues.Count)
        {
            Debug.LogError("Invalid dialogue index: " + index);
            return;
        }
        isDialogueEnd = false; // 重置对话结束状态

        OpenDialogue(index);

        curIndex = index;
    }

    public void OpenDialogue(int index)
    {
        Debug.Log($"打开对话 {index}, 对象={Dialogues[index].name}");

        Transform parent = Dialogues[index].transform.parent;
        while (parent != null)
        {
            parent.gameObject.SetActive(true);
            parent = parent.parent;
        }

        Dialogues[index].SetActive(true);
    }

    public void EndDialogue()
    {
        isDialogueEnd = true;

        if (curIndex >= 0 && curIndex < Dialogues.Count)
        {
            Dialogues[curIndex].SetActive(false);
        }

        // 通知监听者（比如 InteractableItemController）
        onDialogueEnd?.Invoke();
    }
}
