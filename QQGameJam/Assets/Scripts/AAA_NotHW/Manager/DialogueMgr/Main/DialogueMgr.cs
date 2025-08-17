using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMgr : SingletonMonoBehavior<DialogueMgr>
{
    public List<GameObject> Dialogues;

    public void OpenDialogue(int index)
    {
        Dialogues[index].SetActive(true);
    }
}
