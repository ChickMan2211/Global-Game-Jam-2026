using UnityEngine;

public enum ActorState
{
    Idle,
    Running,
    Dead,
    BrokenStand,
    FightStand,
    Focusing,
    Finisher
}

public static class AnimID
{
    public static readonly int Attacking = Animator.StringToHash("Next Attack");
    public static readonly int StopAttack = Animator.StringToHash("Stop Attack");
    public static readonly int TakeDamage = Animator.StringToHash("TakeDamage");
    public static readonly int Running = Animator.StringToHash("Running");
    public static readonly int Stopping = Animator.StringToHash("Stopping");
    public static readonly int FightStand = Animator.StringToHash("FightStand");
    public static readonly int Die = Animator.StringToHash("Die");
    public static readonly int BrokenStand = Animator.StringToHash("BrokenStand");
    public static readonly int Idle = Animator.StringToHash("Idle");
    public static readonly int Focus = Animator.StringToHash("Focus");
    public static readonly int Focusing = Animator.StringToHash("Focusing");
    public static readonly int Finisher = Animator.StringToHash("Finisher");
}
