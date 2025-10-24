using System.Collections;
using UnityEngine;

[RequireComponent(typeof(State))]
public class Combat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] private Player player;
    [SerializeField] private CombatDirectionHandler directionHandler;
    [SerializeField] private PlayerCounterWindow playerCounterWindow;
    [SerializeField] private WeaponHolster weaponHolster;
    [SerializeField] private CapsuleCollider blockCollider;
    [SerializeField] private CapsuleCollider hitCollider;

    [Header("Settings")]
    [Range(0.1f, 1f)] public float blockDelay = 0.5f;
    [SerializeField] private float attackCooldown = 1f;
    public float maxHoldTime = 5f;

    private float nextAttackTime = 0f;
    private float blockTimer = 0f;

    private State state;
    private PlayerWeapon weaponDamage;
    private AttackAnimator attackAnimator;
    private CombatInputHandler inputHandler;

    private void Start()
    {
        state = GetComponent<State>();
        directionHandler = GetComponent<CombatDirectionHandler>();
        weaponHolster = GetComponent<WeaponHolster>();

        attackAnimator = new AttackAnimator(anim, weaponHolster);
        inputHandler = new CombatInputHandler();

        weaponHolster.OnWeaponChanged += UpdateWeaponDamage;
        weaponDamage = weaponHolster.GetCurrentWeaponDamage();
    }

    private void Update()
    {
        if (weaponHolster.IsBusy() || state.isAttacking) return;

        inputHandler.UpdateInput();

        if (Time.time >= nextAttackTime)
        {
            HandleAttacks();
        }

        HandleBlocking();
        HandleHoldAttack();
    }

    #region Attacking

    private void HandleAttacks()
    {
        if (inputHandler.ScrollDown)
            TryAttack("Thrust", state.heavyAttackCost, PlayerWeapon.AttackType.Thrust);

        if (inputHandler.ScrollUp)
            TryAttack("Overhead", state.heavyAttackCost, PlayerWeapon.AttackType.HeavyOverhead);

        if (inputHandler.MouseLeftDown)
            directionHandler.StartSwipe(Input.mousePosition);

        if (inputHandler.MouseLeftUp)
        {
            directionHandler.EndSwipe(Input.mousePosition);
            TrySwipeAttack();
            ResetHoldAttack();
            StartCoroutine(ResetAttackBools());
        }
    }

    private void TryAttack(string animType, float energyCost, PlayerWeapon.AttackType attackType)
    {
        if (state.currentEnergy < energyCost || state.isAttacking) return;

        state.UseEnergy(energyCost);
        weaponDamage.SetAttackType(attackType);
        attackAnimator.PlayAttackAnimation(animType);

        nextAttackTime = Time.time + attackCooldown;
        StartCoroutine(ResetAttackBools());
    }

    private void TrySwipeAttack()
    {
        if (state.currentEnergy < state.normalAttackCost) return;

        state.UseEnergy(state.normalAttackCost);
        weaponDamage.SetAttackType(PlayerWeapon.AttackType.Normal);

        if (state.mouseOnLeftSide)
            attackAnimator.PlayAttackAnimation("Left");
        else if (state.mouseOnRightSide)
            attackAnimator.PlayAttackAnimation("Right");

        nextAttackTime = Time.time + attackCooldown;
    }

    #endregion

    #region  Holding Attacks 

    public float holdAttackTimer = 0f;
    public bool isOvercharged { get; private set; }

    private void HandleHoldAttack()
    {
        if (inputHandler.MouseLeftHeld)
        {
            state.holdingAttack = true;
            holdAttackTimer += Time.deltaTime;

            if (holdAttackTimer >= maxHoldTime && !isOvercharged)
            {
                holdAttackTimer = maxHoldTime;
                isOvercharged = true;
                Debug.Log("Overcharged!");
            }
        }
        else
        {
            ResetHoldAttack();
        }
    }

    private void ResetHoldAttack()
    {
        if (isOvercharged) Debug.Log("Overcharge Reset");

        state.holdingAttack = false;
        holdAttackTimer = 0f;
        isOvercharged = false;
    }

    #endregion

    #region  Blocking 

    private void HandleBlocking()
    {
        if (!state.equipped) return;

        if (inputHandler.BlockPressed)
        {
            playerCounterWindow.TryActivate();
            ActivateBlock();
            blockTimer = 0f;
        }

        if (inputHandler.BlockHeld)
        {
            blockCollider.enabled = true;
            hitCollider.enabled = false;
            player.hasShield = true;
            blockTimer += Time.deltaTime;
        }

        if (inputHandler.BlockReleased)
        {
            blockCollider.enabled = false;
            hitCollider.enabled = true;
            player.hasShield = false;
            DeactivateBlock();
            blockTimer = 0f;
        }
    }

    private void ActivateBlock()
    {
        anim.SetBool("Block", true);
        state.blocking = true;
    }

    private void DeactivateBlock()
    {
        anim.SetBool("Block", false);
        state.blocking = false;
    }

    #endregion

    #region  Animation Helpers 

    public void StartAttack()
    {
        state.isAttacking = true;
        weaponDamage?.EnableDamage(state);
    }

    public void EndAttack()
    {
        state.isAttacking = false;
        weaponDamage?.DisableDamage();
    }

    public void UpdateWeaponDamage(PlayerWeapon newDamage) => weaponDamage = newDamage;

    public IEnumerator ResetAttackBools()
    {
        yield return new WaitForSeconds(1f);
        attackAnimator.ResetAllAttackBools();
        state.ResetMouseDirections();
    }

    #endregion
}