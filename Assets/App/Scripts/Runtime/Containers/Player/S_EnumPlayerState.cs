public enum S_EnumPlayerState
{
    None = 0,
    Idle = 1,
    Moving = 1 << 1,
    Attacking = 1 << 2,
    Dodging = 1 << 3,
    Parrying = 1 << 4,
    Healing = 1 << 5,
    Dying = 1 << 6,
    HitReact = 1 << 7,
    Running = 1 << 8,
}
