public enum State{
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