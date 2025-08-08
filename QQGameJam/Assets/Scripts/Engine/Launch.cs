#define DIRECT_READING //需要延迟读表注释这行

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Globalization;

/// <summary>
/// 游戏启动
/// </summary>
public class Launch : SingletonMonoBehavior<Launch> {

    protected override void Awake() {
        base.Awake();
        Debug.Log(" Launch Awake Begin......");
        Debug.Log(" HWFrameWork Version:0.3.5");
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        gameObject.AddMissingComponent<CoDelegator>();
        gameObject.AddMissingComponent<SoundMgr>();
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
#if DIRECT_READING
        InitClient();
#else
        CoDelegator.Coroutine(InitClient());
#endif
    }

#if DIRECT_READING
    private void InitClient() {
        GameStateMgr.Instance.SwitchState(GameState.Loading);
        //直读表模块
        RefDataMgr.Instance.InitBasic();
        InitLogic();
    }
#else
    //延迟读表模块
    IEnumerator InitClient() {
        GameStateMgr.Instance.SwitchState(GameState.Loading);
        yield return CoDelegator.Coroutine(RefDataMgr.Instance.Init());
        InitLogic();
    }
#endif

    private void InitLogic() {
        //逻辑模块的初始化
        LanguageMgr.Instance.Init();//语言模块要最早初始化
        ObjectPool.Instance.Init();
        BattleMgr.Instance.Init();
        GradeMgr.Instance.Init();
        CurrencyMgr.Instance.Init();
        ScoreMgr.Instance.Init();
        ShopMgr.Instance.Init();
        TaskMgr.Instance.Init();
        SignMgr.Instance.Init();
        SettingsMgr.Instance.Init();

        // 后来添加的Mgr类
        LevelMgr.Instance.Init();
        
        //初始化完成切换到主界面
        GameStateMgr.Instance.SwitchState(GameState.Main);
        //初始化完成切换到主场景
        //LoadSceneMgr.Instance.LoadScene("Main", GameState.Main);
    }

    private void Update()
    {
#if UNITY_EDITOR
        //空格暂停游戏功能
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EditorApplication.isPaused = true;
        }
#endif
        //玩家控制器的Update函数
        PlayerMgr.Instance.OnUpdate();

    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    private void OnApplicationQuit() {
        LocalSave.SaveAll();
    }

    /// <summary>
    /// 游戏焦点
    /// </summary>
    private void OnApplicationFocus(bool focus) {
        if (!focus) {
            LocalSave.SaveAll();
        }
    }
}