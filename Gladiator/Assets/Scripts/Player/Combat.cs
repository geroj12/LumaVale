using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Combat : MonoBehaviour
{
    #region Fields and References

    [SerializeField] private Animator anim;
    private State state;
    [SerializeField] private Player player;
    [SerializeField] private CombatDirectionHandler directionHandler;
    [SerializeField] private PlayerCounterWindow playerCounterWindow;
    [SerializeField] private WeaponHolster weaponHolster;
    [SerializeField] private FinisherController finisherController;

    [Header("Settings")]
    [Range(0.1f, 1f)] public float blockDelay = 0.5f;

    private float blockTimer = 0f;
    private float lastScrollValue = 0f;
    private PlayerWeapon weaponDamage;

    [SerializeField] private float attackCooldown = 1f;
    private float nextAttackTime = 0f;
    #endregion

    [SerializeField] private CapsuleCollider blockCollider;
    [SerializeField] private CapsuleCollider hitCollider;

    [Header("Hold Attack Settings")]
    public float maxHoldTime = 5f;
    public float holdAttackTimer = 0f;
    public bool isOvercharged = false;
    
    #region Unity Methods

    private void Start()
    {
        state = GetComponent<State>();
        directionHandler = GetComponent<CombatDirectionHandler>();
        weaponHolster = GetComponent<WeaponHolster>();

        weaponHolster.OnWeaponChanged += UpdateWeaponDamage;
        weaponDamage = weaponHolster.GetCurrentWeaponDamage();
    }

    private void Update()
    {
        HandleAttackInput();
        HandleBlocking();
        UpdateHoldAttackTimer();
    }

    #endregion

    #region Combat - Attacking

    private void HandleAttackInput()
    {
        if (Time.time < nextAttackTime || state.isAttacking || weaponHolster.IsBusy())
            return;

        if (state.blocking)
            state.ResetMouseDirections();

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll < 0f && lastScrollValue == 0f)
            TriggerAttack("Thrust", state.heavyAttackCost, PlayerWeapon.AttackType.Thrust);

        if (scroll > 0f && lastScrollValue == 0f)
            TriggerAttack("Overhead", state.heavyAttackCost, PlayerWeapon.AttackType.HeavyOverhead);

        lastScrollValue = scroll;

        if (Input.GetMouseButtonDown(0))
            directionHandler.StartSwipe(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
        {
            directionHandler.EndSwipe(Input.mousePosition);
            TriggerSwipeAttack();
            ResetHoldAttack();
            StartCoroutine(ResetAttackBools());
        }
    }

    private void TriggerAttack(string animType, float energyCost, PlayerWeapon.AttackType attackType)
    {
        if (state.currentEnergy < energyCost)
            return;

        state.UseEnergy(energyCost);
        weaponDamage.SetAttackType(attackType);
        PlayAttackAnimation(animType);

        nextAttackTime = Time.time + attackCooldown;
        StartCoroutine(ResetAttackBools());
    }
    private void UpdateHoldAttackTimer()
    {
        if (Input.GetMouseButton(0))
        {
            state.holdingAttack = true;
            holdAttackTimer += Time.deltaTime;

            if (holdAttackTimer >= maxHoldTime && !isOvercharged)
            {
                holdAttackTimer = maxHoldTime;
                isOvercharged = true;
                Debug.Log("⚡ Overcharged!");
            }
        }
        else
        {
            ResetHoldAttack();
        }
    }

    private void ResetHoldAttack()
    {
        if (isOvercharged)
            Debug.Log("Overcharge Reset");

        state.holdingAttack = false;
        holdAttackTimer = 0f;
        isOvercharged = false;
    }

    private void TriggerSwipeAttack()
    {
        if (state.currentEnergy < state.normalAttackCost) return;

        state.UseEnergy(state.normalAttackCost);
        weaponDamage.SetAttackType(PlayerWeapon.AttackType.Normal);

        if (state.mouseOnLeftSide)
            PlayAttackAnimation("Left");
        else if (state.mouseOnRightSide)
            PlayAttackAnimation("Right");

        nextAttackTime = Time.time + attackCooldown;
    }

    #endregion


    #region Combat - Blocking


    private void HandleBlocking()
    {
        if (!state.equipped) return;

        bool blockPressed = Input.GetMouseButtonDown(1);
        bool blockHeld = Input.GetMouseButton(1);
        bool blockReleased = Input.GetMouseButtonUp(1);

        if (blockPressed)
        {
            playerCounterWindow.TryActivate();
            ActivateBlock();
            blockTimer = 0f;
        }

        if (blockHeld)
        {
            blockCollider.enabled = true;
            hitCollider.enabled = false;
            player.hasShield = true;

            blockTimer += Time.deltaTime;
        }

        if (blockReleased)
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
    #region Animation Helpers



    public void StartAttack()
    {
        state.isAttacking = true;

        // Gegner haben die Chance zu kontern, wenn Spieler angreift
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
            enemy.GetComponent<EnemyCombat>()?.TryCounterPlayerAttack();


        weaponDamage?.EnableDamage(state);

    }

    /// <summary>
    /// Wird durch Animation Event oder Timing ausgelöst, um Angriffsstatus zurückzusetzen.
    /// </summary>
    public void EndAttack()
    {
        state.isAttacking = false;


        weaponDamage?.DisableDamage();


    }
    public void UpdateWeaponDamage(PlayerWeapon newDamage)
    {
        weaponDamage = newDamage;
    }

    public IEnumerator ResetAttackBools()
    {
        yield return new WaitForSeconds(1f);
        ResetAllAttackAnimationBools();
        state.ResetMouseDirections();
    }
    private void ResetAllAttackAnimationBools()
    {
        string[] bools =
        {
            "Attack_LEFT_TwoHanded01", "AttackThrust_TwoHanded01", "Attack_RIGHT_TwoHanded01", "AttackUp_TwoHanded01",
            "Attack_LEFT_OneHanded01", "Attack_Thrust_OneHanded01", "Attack_RIGHT_OneHanded01", "Attack_UP_OneHanded01",
            "Attack_LEFT_Unarmed", "Attack_Thrust_Unarmed", "Attack_RIGHT_Unarmed", "Attack_UP_Unarmed"
        };

        foreach (var b in bools)
            anim.SetBool(b, false);
    }

    private void PlayAttackAnimation(string type)
    {
        int weapon = weaponHolster.currentWeaponType;

        switch (type)
        {
            case "Thrust":
                if (weapon == 2) anim.SetBool("AttackThrust_TwoHanded01", true);
                else if (weapon == 1) anim.SetBool("Attack_Thrust_OneHanded01", true);
                else anim.SetBool("Attack_Thrust_Unarmed", true);
                break;

            case "Overhead":
                if (weapon == 2) anim.SetBool("AttackUp_TwoHanded01", true);
                else if (weapon == 1) anim.SetBool("Attack_UP_OneHanded01", true);
                else anim.SetBool("Attack_UP_Unarmed", true);
                break;

            case "Left":
                if (weapon == 2) anim.SetBool("Attack_LEFT_TwoHanded01", true);
                else if (weapon == 1) anim.SetBool("Attack_LEFT_OneHanded01", true);
                else anim.SetBool("Attack_LEFT_Unarmed", true);
                break;

            case "Right":
                if (weapon == 2) anim.SetBool("Attack_RIGHT_TwoHanded01", true);
                else if (weapon == 1) anim.SetBool("Attack_RIGHT_OneHanded01", true);
                else anim.SetBool("Attack_RIGHT_Unarmed", true);
                break;
        }
    }

    #endregion
}
