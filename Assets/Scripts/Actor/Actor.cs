using System;
using UnityEngine;

/// <summary>
/// Base class cho tất cả các nhân vật (Player, Enemy).
/// Quản lý Animation state và trạng thái cơ bản.
/// </summary>
[RequireComponent(typeof(Animator)), RequireComponent(typeof(Rigidbody2D))]
public class Actor : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected Animator animator;
    
    [Header("Debug Info")]
    public ActorState currentState;

    public ActorState CurrentState => currentState;

    protected bool _hasNextAttack = false;

    protected virtual void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        Idle();
    }
    

    #region TriggerFlag
    /// <summary>
    /// Kích hoạt tấn công nếu cờ _hasNextAttack được bật.
    /// </summary>
    public virtual void Attack()
    {
        if (_hasNextAttack)
        {
            animator.SetTrigger(AnimID.Attacking);
            _hasNextAttack = false;
        }
    }
    /// <summary>
    /// Chuẩn bị cho đòn tấn công tiếp theo (Input buffer).
    /// </summary>
    public virtual void NextAttack()
    {
        _hasNextAttack = true;
    }

    /// <summary>
    /// Hủy bỏ lệnh tấn công đang chờ (dùng khi bị choáng hoặc vào trạng thái đặc biệt).
    /// </summary>
    public virtual void CancelNextAttack()
    {
        _hasNextAttack = false;
    }
    

    #endregion

    #region Anim

    public virtual void Idle()
    {
        animator.SetTrigger(AnimID.Idle);
        currentState = ActorState.Idle;
    }
    public virtual void TakeDamage()
    {
        animator.SetTrigger(AnimID.TakeDamage);
    }

    public virtual void Running()
    {
        animator.SetTrigger(AnimID.Running);
        currentState = ActorState.Running;
    }

    public virtual void Stopping()
    {
        animator.SetTrigger(AnimID.Stopping);
    }

    public virtual void FightStand()
    {
        animator.SetTrigger(AnimID.FightStand);
        currentState = ActorState.FightStand;
    }

    public virtual void OnFocus()
    {
        animator.SetTrigger(AnimID.Focus);
    }

    public virtual void OnFocusing()
    {
        animator.SetTrigger(AnimID.Focusing);
        currentState = ActorState.Focusing;
            Debug.Log(currentState);
    }

    public virtual void BrokenStand(bool isBroken)
    {
      if(isBroken) GameManager.Instance.audioManager.PlayBrokenSandSound();
            animator.SetBool(AnimID.BrokenStand,isBroken);
            currentState = ActorState.BrokenStand;
    }
    public virtual void Die()
    {
        BrokenStand(false);
        animator.SetTrigger(AnimID.Die);
        currentState = ActorState.Dead;
    }
    
    public virtual void OnFinisher()
    {
        animator.SetTrigger(AnimID.Finisher);
        currentState = ActorState.Finisher;
    }

    /// <summary>
    /// Animation Event: Gọi khi kết thúc animation tấn công.
    /// </summary>
    public virtual void OnAnimationFinishAttack()
    {
        if(currentState == ActorState.Focusing) return;
        animator.SetTrigger(AnimID.StopAttack);
        currentState = ActorState.FightStand;
    }
    /// <summary>
    /// Reset toàn bộ Trigger và Bool về mặc định (False).
    /// Dùng khi nhân vật chết, respawn hoặc bị hủy trạng thái.
    /// </summary>
    public virtual void ResetAnimatorParameters()
    {
        if (animator == null) return;

        // Duyệt qua tất cả các tham số có trong Animator Controller
        foreach (var param in animator.parameters)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Trigger:
                    animator.ResetTrigger(param.nameHash);
                    break;
                
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(param.nameHash, false);
                    break;
                
                // Nếu muốn reset cả Float/Int về 0 thì mở comment đoạn dưới
                /*
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(param.nameHash, 0f);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(param.nameHash, 0);
                    break;
                */
            }
        }
        
        // Reset luôn biến cờ logic trong code để đồng bộ
        _hasNextAttack = false;
    }
  
    #endregion
    

   
    
   
}