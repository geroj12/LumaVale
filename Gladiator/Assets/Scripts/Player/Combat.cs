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

    [SerializeField] private WeaponHolster weaponHolster;
    [SerializeField] private FinisherController finisherController;

    [Header("Settings")]
    [Range(0.1f, 1f)] public float blockDelay = 0.5f;

    private float blockTimer = 0f;
    private float lastScrollValue = 0f;
    private WeaponDamage weaponDamage;
    [SerializeField] private WeaponDamage unarmedWeaponDamageLeftFist;
    [SerializeField] private WeaponDamage unarmedWeaponDamageRightFist;

    [SerializeField] private float attackCooldown = 1f;
    private float nextAttackTime = 0f;
    #endregion

    [SerializeField] private CapsuleCollider blockCollider;
    [SerializeField] private CapsuleCollider hitCollider;


    #region Unity Methods

    void Start()
    {

        state = GetComponent<State>();
        directionHandler = GetComponent<CombatDirectionHandler>();
        weaponHolster = GetComponent<WeaponHolster>();
        weaponHolster.OnWeaponChanged += UpdateWeaponDamage;

        weaponDamage = weaponHolster.GetCurrentWeaponDamage() ?? unarmedWeaponDamageRightFist;
    }

    void Update()
    {
        HandleAttack();
        HandleBlocking();
    }

    #endregion

    #region Combat - Attacking

    /// <summary>
    /// Behandelt Angriffe basierend auf Mausklicks und Scrollrad.
    /// </summary>
    private void HandleAttack()
    {
        if (Time.time < nextAttackTime) return; // Cooldown aktiv
        if (weaponHolster.IsBusy()) return;
        if (state.blocking)
            state.ResetMouseDirections();

        if (state.isAttacking) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Thrust (scroll down)
        if (scroll < 0f && lastScrollValue == 0f)
            TriggerThrustAttack();

        // Overhead (scroll up)
        if (scroll > 0f && lastScrollValue == 0f)
            TriggerOverheadAttack();

        lastScrollValue = scroll;

        if (Input.GetMouseButton(0))
        {
            state.holdingAttack = true;

        }
        // Directional Swipe Attack (mouse click & drag)
        if (Input.GetMouseButtonDown(0))
            directionHandler.StartSwipe(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
        {
            directionHandler.EndSwipe(Input.mousePosition);
            TriggerSwipeAttack();
            state.holdingAttack = false;

            StartCoroutine(ResetAttackBools());

        }
    }

    private void TriggerThrustAttack()
    {

        if (state.currentEnergy < state.heavyAttackCost) return;

        state.UseEnergy(state.heavyAttackCost);
        state.attackThrust = true;

        weaponDamage.SetAttackType(WeaponDamage.AttackType.Thrust);
        PlayAttackAnimation("Thrust");

        nextAttackTime = Time.time + attackCooldown;
        StartCoroutine(ResetAttackBools());
    }

    private void TriggerOverheadAttack()
    {

        if (state.currentEnergy < state.heavyAttackCost) return;

        state.UseEnergy(state.heavyAttackCost);
        state.attackUp = true;

        weaponDamage.SetAttackType(WeaponDamage.AttackType.HeavyOverhead);
        PlayAttackAnimation("Overhead");

        nextAttackTime = Time.time + attackCooldown;
        StartCoroutine(ResetAttackBools());
    }

    private void TriggerSwipeAttack()
    {
        if (state.currentEnergy < state.normalAttackCost) return;

        state.UseEnergy(state.normalAttackCost);
        weaponDamage.SetAttackType(WeaponDamage.AttackType.Normal);

        if (state.mouseOnLeftSide)
            PlayAttackAnimation("Left");
        else if (state.mouseOnRightSide)
            PlayAttackAnimation("Right");

        nextAttackTime = Time.time + attackCooldown;
    }

    #endregion


    #region Combat - Blocking

    /// <summary>
    /// Behandelt das Blockverhalten mit Verzögerung.
    /// </summary>
    private void HandleBlocking()
    {
        if (!state.equipped) return;

        bool isHoldingBlock = Input.GetAxisRaw("Fire2") > 0.00000001f;

        if (isHoldingBlock)
        {
            blockCollider.enabled = true;
            hitCollider.enabled = false;
            player.hasShield = true;
            blockTimer += Time.deltaTime;
            if (blockTimer > blockDelay)
            {
                ActivateBlock();
                blockTimer = 0;
            }
        }
        else
        {
            blockCollider.enabled = false;
            hitCollider.enabled = true;

            player.hasShield = false;

            DeactivateBlock();
            blockTimer = 0;
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

    /// <summary>
    /// Wird durch Animation Event oder Timing ausgelöst, um Angriffsstatus zu setzen.
    /// </summary>
    public void StartAttack()
    {
        state.isAttacking = true;

        if (weaponHolster.currentWeaponType == 0)
        {
            if (state.mouseOnLeftSide)
                unarmedWeaponDamageLeftFist.EnableDamage(state);
            else if (state.mouseOnRightSide)
                unarmedWeaponDamageRightFist.EnableDamage(state);
        }
        else
        {
            weaponDamage?.EnableDamage(state);
        }
    }

    /// <summary>
    /// Wird durch Animation Event oder Timing ausgelöst, um Angriffsstatus zurückzusetzen.
    /// </summary>
    public void EndAttack()
    {
        state.isAttacking = false;

        if (weaponHolster.currentWeaponType == 0)
        {
            unarmedWeaponDamageLeftFist.DisableDamage();
            unarmedWeaponDamageRightFist.DisableDamage();
        }
        else
        {
            weaponDamage?.DisableDamage();
        }

    }
    public void UpdateWeaponDamage(WeaponDamage newDamage)
    {
        weaponDamage = newDamage ?? unarmedWeaponDamageRightFist;
    }
    /// <summary>
    /// Setzt alle Angriffs-Animation-Bools zurück.
    /// </summary>
    public IEnumerator ResetAttackBools()
    {
        yield return new WaitForSeconds(1f);

        // Zweihaendig
        anim.SetBool("Attack_LEFT_TwoHanded01", false);
        anim.SetBool("AttackThrust_TwoHanded01", false);
        anim.SetBool("Attack_RIGHT_TwoHanded01", false);
        anim.SetBool("AttackUp_TwoHanded01", false);

        // Einhaendig
        anim.SetBool("Attack_LEFT_OneHanded01", false);
        anim.SetBool("Attack_Thrust_OneHanded01", false);
        anim.SetBool("Attack_RIGHT_OneHanded01", false);
        anim.SetBool("Attack_UP_OneHanded01", false);

        // Unbewaffnet
        anim.SetBool("Attack_LEFT_Unarmed", false);
        anim.SetBool("Attack_Thrust_Unarmed", false);
        anim.SetBool("Attack_RIGHT_Unarmed", false);
        anim.SetBool("Attack_UP_Unarmed", false);

        state.ResetMouseDirections();
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
