using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMgr : SingletonMonoBehavior<DialogueMgr>
{
    public List<GameObject> Dialogues;
    public bool isDialogueEnd = false;
    private int curIndex;
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
        if (isDialogueEnd)
        {
            Send.SendMsg(SendType.Over_Conversation, curIndex);
            isDialogueEnd = false;
        }
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
        Debug.Log(index);
        Dialogues[index].SetActive(true);
    }
}
