using UnityEngine;

public class Player : MonoBehaviour
{
    private Movement movement;
    [SerializeField] private Animator animator;
    [SerializeField] private Combat combat;
    [SerializeField] private State state;

    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP;

    [Header("Hit Reaction Settings")]
    public float hitReactionCooldown = 0.5f;
    private float lastHitTime = -999f;

    [Header("Shield System")]
    public bool hasShield = true;
    public float maxShieldDurability = 100f;
    public float currentShieldDurability;
    void Start()
    {
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
        currentHP = maxHP;
        currentShieldDurability = maxShieldDurability;

    }

    void Update()
    {
        state.RegenerateEnergy();
        movement.HandleMovement();
    }


    public void InterruptAttack()
    {
        combat = GetComponent<Combat>();
        if (combat != null)
        {

            combat.StartCoroutine(combat.ResetAttackBools());

        }
        animator.SetTrigger("HitBlocked"); // z. B. kleine Rückzuck-Animation

    }

    public void TakeDamage(float amount, Vector3 attackerPosition, bool hitShield = false)
    {
        InterruptAnimations();

        if (hitShield)
        {
            animator.SetTrigger("BlockImpact");
            currentShieldDurability -= amount;
            if (currentShieldDurability <= 0f)
            {
                //BreakShield();
            }

            return;
        }

        currentHP -= amount;
        if (Time.time - lastHitTime >= hitReactionCooldown)
        {
            lastHitTime = Time.time;
            PlayHitReaction(attackerPosition);
        }
    }


    private void PlayHitReaction(Vector3 attackerPosition)
    {
        Vector3 toAttacker = (attackerPosition - transform.position).normalized;
        toAttacker.y = 0;

        float angle = Vector3.SignedAngle(transform.forward, toAttacker, Vector3.up);

        // Normalisieren
        angle = Mathf.DeltaAngle(0, angle);

        string trigger = Mathf.Abs(angle) <= 90f ? "HitFront" : "HitBack";
        animator.SetTrigger(trigger);
    }


    public void InterruptAnimations()
    {
        if (animator == null) return;

        string[] attackTriggers = { "Attack_LEFT_OneHanded01", "Attack_RIGHT_OneHanded01","Attack_Thrust_OneHanded01","Attack_LEFT_TwoHanded01",
            "Attack_RIGHT_TwoHanded01","AttackThrust_TwoHanded01","AttackUp_TwoHanded01" };
        foreach (string trigger in attackTriggers)
        {
            animator.ResetTrigger(trigger);
        }

        animator.SetBool("Attack_UP_OneHanded01", false);

        animator.Play("Empty", 0); // ersetzt aktuelle Base-Layer-Animation sofort
    }
}
