using System.Collections;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyWeapon weapon;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyCounterWindow counterWindow;
    [SerializeField] private StateMachineEnemy stateMachine;
    [SerializeField] private Enemy enemy;

    [Header("Counter Settings")]
    [SerializeField, Range(0f, 1f)] private float counterChance = 0.25f;
    [SerializeField] private float stunDuration = 2f;

    private bool isAttacking;
    private bool isAttackFinished;

    private bool isStunned;

    private bool isDodging;
    private bool isDodgingFinished;

     public bool IsAttacking
    {
        get { return isAttacking; }
    }

    public bool IsAttackFinished
    {
        get { return isAttackFinished; }
    }

    public bool IsStunned
    {
        get { return isStunned; }
    }

     public bool IsDodging
    {
        get { return isDodging; }
    }

    public bool IsDodgingFinished
    {
        get { return isDodgingFinished; }
    }
    private void Awake()
    {
        CacheComponents();
    }

    private void Start()
    {
        if (counterWindow != null)
        {
            counterWindow.OnCounterTriggered += HandleCounterTriggered;
        }
    }
    private void CacheComponents()
    {
        if (enemy == null)
            enemy = GetComponent<Enemy>();

        if (weapon == null)
            weapon = GetComponentInChildren<EnemyWeapon>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (stateMachine == null)
            stateMachine = GetComponent<StateMachineEnemy>();

        if (counterWindow == null)
            counterWindow = GetComponent<EnemyCounterWindow>();
    }

  public void TelegraphAttackUI()
    {
        if (counterWindow != null)
        {
            counterWindow.TryActivate();
        }
    }
    public void ApplyStun()
    {
        if (isStunned)
            return;

        isStunned = true;
        isAttacking = false;

        if (weapon != null)
        {
            weapon.DisableDamage();
        }

        if (enemy != null)
        {
            enemy.InterruptAnimations();
        }

        animator.SetTrigger("Stunned");
    }


    private IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(1f);
        isAttackFinished = false;
    }

    private void HandleCounterTriggered()
    {
         animator.SetTrigger("Countered");

        if (stateMachine != null)
        {
            stateMachine.TemporarilyDisableFSM(stunDuration);
        }
    }

    public void TryCounterPlayerAttack()
    {
           if (counterWindow == null)
            return;

        if (counterWindow.IsActive || counterWindow.IsOnCooldown)
            return;

        if (Random.value > counterChance)
            return;

        counterWindow.TryActivate();
        animator.SetTrigger("PrepareCounter");
    }



    #region Animation Event Functions
    public void StartAttack()
    {
       isAttacking = true;

        if (weapon != null)
        {
            weapon.EnableDamage();
        }
    }


    public void EndAttack()
    {
       isAttacking = false;

        if (weapon != null)
        {
            weapon.DisableDamage();
        }

    }
    public void DodgeStart()
    {
        isDodging = true;
    }

    public void DodgeEnd()
    {
        isDodging = false;

    }
    public void AttackFinished()
    {
        isAttackFinished = true;
        StartCoroutine(ResetAttackFlag());

    }
    public void IsStunnedFinished()
    {
        isStunned = false;

    }
    #endregion
}
