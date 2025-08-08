//枚举定义类 大功能的枚举定义统一写在这里

public enum WindowType {
    Normal = 0,                 //--普通窗体，总是在最下层
    Modal = 1,                  //--模态窗体，总是处于普通窗体之上，提示窗体之下
    Tips = 2,                   //--提示窗体，总是处于系统窗体之下
    System = 3,                 //--系统窗体，总是在最上层
}

public enum OpenAnimType {
    None,                       //--无
    Position,                   //--位置
    Scale,                      //--缩放
    Alpha,                      //--渐变
    ScaleAndAlpha,              //--缩放和渐变 
    Custom,                     //自定义
}

//任务类型
public enum TaskType {
    Nothing,                     //--无
    SignInDay,                   //--签到天数
    GradeNum,                    //--玩家等级
    BestScore,                   //--最佳分数
    GameTimes,                   //--游戏次数
    ReliveTimes                  //--复活次数
}

/// <summary>
/// 任务状态 如有需求 可快速拓展一个领奖状态
/// </summary>
public enum TaskState {
    Doing,
    Complete,
}

//签到状态
public enum SignState {
    CanSign,                     //--可签到
    Signed,                      //--已签
    SignLock,                    //--不可签
}

//奖励种类
public enum RewardType {
    Coin,                        //金币
    Item,
}

//商品状态
public enum ShopItemState {
    InUse,                        //使用中
    CanUse,                       //已解锁
    Lock                          //未解锁
}

//商品解锁类型
public enum UnLockType {
    None,                         //默认解锁
    Gold,                         //金币购买
    Task,                         //任务解锁
}

public enum UnLockItemResult {
    Unlocked, //已解锁
    Fail,//未解锁
    Success,//可解锁
}