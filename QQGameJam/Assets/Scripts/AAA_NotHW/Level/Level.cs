using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("初始动画")]
    public GameObject musicBattleScene; // 音乐战斗预制体
    public List<Transform> spawnPoints; // 音符生成点
    [SerializeField]
    private List<IPart> parts = new List<IPart>();

    public int CurFailDialogue_ID;
    public int CurFinishDialogue_ID;
    private void OnEnable()
    {
        IniMsg();

    }

    private void OnDisable()
    {
        ClearMsg();
    }

    /// <summary>
    /// 每次加载一个新Level时调用，不是Part
    /// </summary>
    public void Init()
    {
        parts.Clear();
        parts = GetComponentsInChildren<IPart>(true).ToList();
        Debug.Log(parts.Count);

        ChangePart(0);  // Part初始化为第一个

        // 初始化当前场景中的NoteSpawner
        NoteSpawner.Instance.InitNoteSpawn(spawnPoints);
    }

    private void IniMsg()
    {
        Send.RegisterMsg(SendType.MusicBattleEnd, OnMusicBattleEnd);
    }
    private void ClearMsg()
    {
        Send.UnregisterMsg(SendType.MusicBattleEnd, OnMusicBattleEnd);
    }
    public void OnMusicBattleEnd(params object[] objects)
    {
        bool isFail = (bool)objects[0];
        EndMusicBattleScene(isFail);
    }

    public void ChangePart(int index)
    {
        if (index < 0 || index > parts.Count)
        {
            Debug.LogError("Index out of range: " + index);
            Debug.Log(parts.Count);
            return;
        }

        PartManager.Instance.SwitchTo(parts[index]);
    }

    /// <summary>
    /// 当战斗开始时
    /// </summary>
    /// <param name="battleID">战斗数据ID</param>
    /// <param name="battleFailDialogue_ID">战斗失败时对话的ID</param>
    /// <param name="battleFinishDialogue_ID">战斗成功时对话的ID</param>
    public void StartMusicBattleScene(int battleID, int battleFailDialogue_ID, int battleFinishDialogue_ID)
    {
        BattleMgr.Instance.state = BattleState.MusicBattle;

        PartManager.Instance.currentPart.OnExit();

        musicBattleScene.SetActive(true); // 激活音乐战斗场景

        // 设置场景中摄像机的位置
        CameraMgr.Instance.transform.position = musicBattleScene.transform.position + new Vector3(0, 0, -10); // 确保摄像机在正确位置
        CameraMgr.Instance.target = musicBattleScene.transform;

        BGMController.Instance.Start_BGMBattle(battleID); // 开始战斗

        CurFailDialogue_ID = battleFailDialogue_ID;
        CurFinishDialogue_ID = battleFinishDialogue_ID;

        Debug.Log("音乐战斗开始");
    }

    public void EndMusicBattleScene(bool isFail)
    {
        Debug.Log("音乐战斗结束");
        BattleMgr.Instance.state = BattleState.Game;
        BattleWindow.Instance.ShowScenePanel();
        musicBattleScene.SetActive(false); // 停用音乐战斗场景

        // ChangePart(0);  // 回到开始世界

        // if (isFail)
        //     Send.SendMsg(SendType.Into_Conversation, 4);
        // else
        //     Send.SendMsg(SendType.Into_Conversation, 3);
        if (isFail)
        {
            // Send.SendMsg(SendType.Into_Conversation, 4);
            DialogueCtrl.Instance.OpenDialogueScene(CurFailDialogue_ID, () => ChangePart(1));
        }
        else
        {
            DialogueCtrl.Instance.OpenDialogueScene(CurFinishDialogue_ID, () => ChangePart(1));
        }
    }

}
