using System.Collections;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyWeapon weapon;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyCounterWindow counterWindow;
    [SerializeField] private StateMachineEnemy stateMachine;

    [Header("Counter Settings")]
    [SerializeField, Range(0f, 1f)] private float counterChance = 0.25f;
    [SerializeField] private float stunDuration = 2f;

    private static readonly int CounteredTrigger = Animator.StringToHash("Countered");
    private static readonly int PrepareCounterTrigger = Animator.StringToHash("PrepareCounter");
    private static readonly int StunnedTrigger = Animator.StringToHash("Stunned");

    private bool isAttacking;
    private bool isAttackFinished;

    public bool IsAttacking => isAttacking;
    public bool IsAttackFinished => isAttackFinished;


    private bool isDodging;
    private bool isDodgingFinished;

    public bool IsDodging => isDodging;
    public bool IsDodgingFinished => isDodgingFinished;
    private void Awake()
    {
        CacheComponents();
    }

    private void Start()
    {
        if (counterWindow != null)
            counterWindow.OnCounterTriggered += HandleCounterTriggered;
    }
    private void CacheComponents()
    {
        if (!weapon) weapon = GetComponentInChildren<EnemyWeapon>();
        if (!animator) animator = GetComponent<Animator>();
        if (!stateMachine) stateMachine = GetComponent<StateMachineEnemy>();
        if (!counterWindow) counterWindow = GetComponent<EnemyCounterWindow>();
    }

    public void TelegraphAttackUI() => counterWindow?.TryActivate();

    public void StartAttack()
    {
        isAttacking = true;
        weapon.EnableDamage();
    }


    public void EndAttack()
    {
        isAttacking = false;
        weapon.DisableDamage();


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
        // sorgt dafür, dass bei neuem Angriff wieder auf false gesetzt wird
        StartCoroutine(ResetAttackFlag());

    }
    private IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(1f);
        isAttackFinished = false;
    }

    private void HandleCounterTriggered()
    {
        animator.SetTrigger(CounteredTrigger);
        stateMachine?.TemporarilyDisableFSM(stunDuration);
    }

    public void TryCounterPlayerAttack()
    {
        if (counterWindow == null || counterWindow.IsActive || counterWindow.IsOnCooldown)
            return;

        if (Random.value > counterChance)
            return;

        counterWindow.TryActivate();
        animator.SetTrigger(PrepareCounterTrigger);
    }

    public IEnumerator ResetAttackBools()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("Attack_Right", false);
        animator.SetBool("Attack_Left", false);
        animator.SetBool("Attack_Overhead", false);
    }
    public void ApplyStun()
    {
        animator.SetTrigger(StunnedTrigger);

    }
}
