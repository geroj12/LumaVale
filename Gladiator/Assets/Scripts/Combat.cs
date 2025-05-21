using System.Collections;
using UnityEngine;

public class Combat : MonoBehaviour
{
    #region Fields and References

    private Animator anim;
    private State state;

    [SerializeField] private CombatDirectionHandler directionHandler;
    [SerializeField] private WeaponHolster weaponHolster;
    [SerializeField] private FinisherController finisherController;

    [Header("Settings")]
    [Range(0.1f, 1f)] public float blockDelay = 0.5f;

    private float blockTimer = 0f;
    private float lastScrollValue = 0f;
    private WeaponDamage weaponDamage;


    #endregion

    #region Unity Methods

    void Start()
    {
        anim = GetComponent<Animator>();
        state = GetComponent<State>();
        directionHandler = GetComponent<CombatDirectionHandler>();
        weaponHolster = GetComponent<WeaponHolster>();
        weaponDamage = weaponHolster.GetCurrentWeaponDamage();

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
        if (weaponHolster.IsBusy() || finisherController.IsFinishing()) return;
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

        // Directional Swipe Attack (mouse click & drag)
        if (Input.GetMouseButtonDown(0))
            directionHandler.StartSwipe(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
        {
            directionHandler.EndSwipe(Input.mousePosition);
            TriggerSwipeAttack();
            StartCoroutine(ResetAttackBools());
        }
    }

    private void TriggerThrustAttack()
    {
        state.attackThrust = true;
        switch (weaponHolster.currentWeaponType)
        {
            case 2: anim.SetBool("AttackThrust_TwoHanded01", true); break;
            case 1: anim.SetBool("Attack_Thrust_OneHanded01", true); break;
            case 0: anim.SetBool("Attack_Thrust_Unarmed", true); break;
        }
        StartCoroutine(ResetAttackBools());
    }

    private void TriggerOverheadAttack()
    {
        state.attackUp = true;
        switch (weaponHolster.currentWeaponType)
        {
            case 2: anim.SetBool("AttackUp_TwoHanded01", true); break;
            case 1: anim.SetBool("Attack_UP_OneHanded01", true); break;
            case 0: anim.SetBool("Attack_UP_Unarmed", true); break;
        }
        StartCoroutine(ResetAttackBools());
    }

    private void TriggerSwipeAttack()
    {
        int weapon = weaponHolster.currentWeaponType;
        if (state.mouseOnLeftSide)
        {
            switch (weapon)
            {
                case 2: anim.SetBool("Attack_LEFT_TwoHanded01", true); break;
                case 1: anim.SetBool("Attack_LEFT_OneHanded01", true); break;
                case 0: anim.SetBool("Attack_LEFT_Unarmed", true); break;
            }
        }
        else if (state.mouseOnRightSide)
        {
            switch (weapon)
            {
                case 2: anim.SetBool("Attack_RIGHT_TwoHanded01", true); break;
                case 1: anim.SetBool("Attack_RIGHT_OneHanded01", true); break;
                case 0: anim.SetBool("Attack_RIGHT_Unarmed", true); break;
            }
        }
    }

    #endregion


    #region Combat - Blocking

    /// <summary>
    /// Behandelt das Blockverhalten mit Verzögerung.
    /// </summary>
    private void HandleBlocking()
    {
        if (!state.equipped) return;

        bool isHoldingBlock = Input.GetAxis("Fire2") > 0.1f;

        if (isHoldingBlock)
        {
            blockTimer += Time.deltaTime;
            if (blockTimer > blockDelay)
            {
                ActivateBlock();
                blockTimer = 0;
            }
        }
        else
        {
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
        weaponDamage.EnableDamage();
    }

    /// <summary>
    /// Wird durch Animation Event oder Timing ausgelöst, um Angriffsstatus zurückzusetzen.
    /// </summary>
    public void EndAttack()
    {
        state.isAttacking = false;
        weaponDamage.DisableDamage();

    }
    public void UpdateWeaponDamage(WeaponDamage newDamage)
    {
        weaponDamage = newDamage;
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

    #endregion
}
