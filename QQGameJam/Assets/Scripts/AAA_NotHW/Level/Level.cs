using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject musicBattleScene; // 音乐战斗预制体
    public List<Transform> spawnPoints; // 音符生成点
    private List<IPart> parts = new List<IPart>();
    public Part_Real realPart;

    private IPart curPart;
    private void OnEnable()
    {
        IniMsg();

    }

    private void OnDisable()
    {
        ClearMsg();
    }
    public void Init()
    {
        parts.Clear();
        parts = GetComponentsInChildren<IPart>(true).ToList();
        Debug.Log(parts.Count);

        ChangePart(0);

        // 开始时隐藏 Part_Real
        if (realPart != null)
            realPart.gameObject.SetActive(false);

        NoteSpawner.Instance.InitNoteSpawn(spawnPoints);

        if (LevelMgr.Instance.CurrentLevel == -1)
        {
            Send.SendMsg(SendType.Into_Conversation, 0);

            DialogueMgr.Instance.onDialogueEnd += () =>
            {
                Debug.Log("对话0结束，生成玩家 + 显示 Part_Real");

                if (realPart != null)
                {
                    realPart.gameObject.SetActive(true);
                    realPart.GenPlayer(realPart.PlayerSpawnTransform);
                }
            };
        }
    }




    private void IniMsg()
    {
        Send.RegisterMsg(SendType.Into_InsideWorld, OnIntoInsideWorld);
        Send.RegisterMsg(SendType.Over_Conversation, OnConversationOver);
        Send.RegisterMsg(SendType.MusicBattleEnd, OnMusicBattleEnd);

        // Send.RegisterMsg(SendType.Into_MusicBattle, OnMusicBattle);
    }
    private void ClearMsg()
    {
        Send.UnregisterMsg(SendType.Into_InsideWorld, OnIntoInsideWorld);
        Send.UnregisterMsg(SendType.Over_Conversation, OnConversationOver);
        Send.UnregisterMsg(SendType.MusicBattleEnd, OnMusicBattleEnd);

        // Send.UnregisterMsg(SendType.Into_MusicBattle, OnMusicBattle);
    }
    public void OnMusicBattleEnd(params object[] objects)
    {
        EndMusicBattleScene();
    }

    public void OnConversationOver(params object[] data)
    {
        int index = (int)data[0];
        // Debug.Log(index);
        if (index == 0) return;
        if (index == 1) // 当是index为1的对话结束
        {
            // 处理对话结束的逻辑
            // LevelMgr.Instance.StartMusicBattleScene(musicBattleScene); // 开始音乐战斗
            StartMusicBattleScene(musicBattleScene);
        }
    }

    public void OnIntoInsideWorld(params object[] data)
    {
        int key = (int)data[0];
        if (key < 0 || key >= parts.Count)
        {
            Debug.LogError("Invalid key: " + key);
            return;
        }

        ChangePart(key);
    }

    public void ChangePart(int index)
    {
        if (index < 0 || index >= parts.Count)
        {
            Debug.LogError("Index out of range: " + index);
            return;
        }

        if (curPart != null)
        {
            curPart.OnExit(); // 退出当前部分
        }
        curPart = parts[index];
        curPart.OnEnter();
    }
    /// <summary>
    /// 音游战斗开始
    /// </summary>
    public void StartMusicBattleScene(GameObject curMBattleScene)
    {
        BattleMgr.Instance.state = BattleState.MusicBattle;

        musicBattleScene = curMBattleScene;
        musicBattleScene.SetActive(true); // 激活音乐战斗场景

        // 设置摄像机的位置
        CameraMgr.Instance.transform.position = musicBattleScene.transform.position + new Vector3(0,0,-10); // 确保摄像机在正确位置
        CameraMgr.Instance.target = musicBattleScene.transform;

        BGMController.Instance.StartBGM(); // 开始背景音乐
        Debug.Log("音乐战斗开始");
    }

    public void EndMusicBattleScene()
    {
        Debug.Log("音乐战斗结束");
        BattleMgr.Instance.state = BattleState.Game;
        BattleWindow.Instance.ShowScenePanel();
        musicBattleScene.SetActive(false); // 停用音乐战斗场景

        ChangePart(0);
        Send.SendMsg(SendType.Into_Conversation, 2);
    }

}
