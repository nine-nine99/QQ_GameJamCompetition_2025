public enum State
{
    Idle,
    Patrol,
    Escape,
    DashEscape,
    TeleportBack,
    Stun,
    Chase,
    ChaseCoin,// 追逐金币
    Walk,
    Run,
    Jump,
    Attack,
    AttackTarget,// 敌人向目标点逼近 
    CollectCoin,
    Summon01, // 召唤
    FallDown,
    Dead
}

public enum SyllableType
{
    Normal, //WORKFLOW: 普通音节. 要是有音节记得在这里添加
    Hold//长按音节
}
