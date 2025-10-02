
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("Setup")]
    public Animator animator;
    [SerializeField] private GameObject swordObject;
    [SerializeField] private EnemyCombat combat;

    [Header("Shield System")]
    public bool hasShield = true;
    public float maxShieldDurability = 100f;
    public float currentShieldDurability;

    public GameObject shieldObject;
    public Rigidbody shieldRigidbody;
    public Collider shieldCollider;

    public GameObject damageTextPrefab;
    public Transform damageTextSpawnPoint;
    [SerializeField] private CharacterController controller;
    private Vector3 velocity;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private StateMachineEnemy statemachine;

    [Header("Settings")]
    private bool isFatalFinisher = false;
    [SerializeField] private GameObject[] smallBloodEmitters;
    [SerializeField] private GameObject[] finisherBloodEmitters;
    private GameObject activeBloodEmitter;

    [SerializeField] private GameObject attachedBloodDecals;
    [SerializeField] private Transform damageCanvasTransform;

    [Header("Finisher Einstellungen")]
    public float fatalFinisherThreshold = 10f; // Prozent
    public bool isDead = false;
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;
    public Player player;
    public Transform finisherAnchor;

    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP;

    [Header("Hit Reaction Settings")]
    public float hitReactionCooldown = 0.5f;
    private float lastHitTime = -999f;

    void Awake()
    {
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        ragdollColliders = GetComponentsInChildren<Collider>(true);
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }

    }
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        currentShieldDurability = maxShieldDurability;
        currentHP = maxHP;
    }
    void FixedUpdate()
    {
        ApplyGravity();
    }

    public void StartFinisher(string finisherTrigger)
    {
        if (isDead) return;
        InterruptAnimations();
        statemachine?.TemporarilyDisableFSM(1f);
        float hpPercent = GetHealthPercent();
        isFatalFinisher = hpPercent <= fatalFinisherThreshold;

        animator.SetTrigger(finisherTrigger);

    }


    public float GetHealthPercent()
    {
        return (float)currentHP / maxHP * 100f;
    }
    public void StartBloodEffect()
    {
        DeactivateBloodDecal();
        SpawnRandomBloodEmitter(smallBloodEmitters);
    }

    public void StartFinisherBloodEffect()
    {
        DeactivateBloodDecal();

        SpawnRandomBloodEmitter(finisherBloodEmitters);
    }

    private void SpawnRandomBloodEmitter(GameObject[] emitterPool)
    {
        if (emitterPool == null || emitterPool.Length == 0)
        {
            Debug.LogWarning("No blood emitter prefabs assigned for this pool!");
            return;
        }

        Transform bloodSpawnPoint = transform.Find("BloodPoint");
        if (bloodSpawnPoint == null)
        {
            Debug.LogWarning("Missing BloodPoint child on enemy!");
            return;
        }

        int randomIndex = Random.Range(0, emitterPool.Length);
        GameObject chosenEmitter = emitterPool[randomIndex];

        activeBloodEmitter = Instantiate(
            chosenEmitter,
            bloodSpawnPoint.position,
            bloodSpawnPoint.rotation
        );

        ActivateBloodDecal();
    }
    private void ActivateBloodDecal()
    {
        attachedBloodDecals.SetActive(true);
    }

    private void DeactivateBloodDecal()
    {
        attachedBloodDecals.SetActive(false);

    }

    public void EnableRagdoll()
    {
        DropWeapons();

        isDead = true;
        statemachine.enabled = false;
        animator.enabled = false;
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.interpolation = RigidbodyInterpolation.None;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }

        foreach (var col in ragdollColliders)
        {
            col.enabled = true;
        }


    }

    private void DropWeapons()
    {
        ApplyPhysics(swordObject);
        ApplyPhysics(shieldObject);

    }

    private void ApplyPhysics(GameObject obj)
    {
        obj.transform.parent = null; // Vom Gegner lösen
        if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        var collider = obj.GetComponent<Collider>();
        if (collider != null) collider.isTrigger = false;

    }

    public void TakeDamage(float amount, Vector3 attackerPosition, bool hitShield = false, WeaponDamage.AttackType attackType = WeaponDamage.AttackType.Normal)
    {
        if (isDead) return;
        statemachine?.TemporarilyDisableFSM(1f);
        InterruptAnimations();
        if (hitShield)
        {
            animator.SetTrigger("ShieldImpact");
            currentShieldDurability -= amount;
            if (currentShieldDurability <= 0f)
            {
                BreakShield();
            }

            return;
        }

        currentHP -= amount;

        if (Time.time - lastHitTime >= hitReactionCooldown)
        {
            lastHitTime = Time.time;
            PlayHitReaction(attackerPosition);
        }

        ShowDamageText(amount, attackType);

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    public void InterruptAttack()
    {
        combat = GetComponent<EnemyCombat>();
        if (combat != null)
        {

            combat.StartCoroutine(combat.ResetAttackBools());

        }
        animator.SetTrigger("HitBlocked"); // z. B. kleine Rückzuck-Animation

    }

    private void BreakShield()
    {
        if (!hasShield || shieldObject == null) return;

        shieldObject.transform.SetParent(null); // Deparenten
        ApplyPhysics(shieldObject);
        hasShield = false;
        currentShieldDurability = 0;

        // 3. Eventuell Audio oder VFX abspielen
        // shieldBreakVFX.Play(); etc. (optional)

        //animator.SetTrigger("ShieldBreak");
    }

    private void ShowDamageText(float amount, WeaponDamage.AttackType attackType)
    {
        if (damageTextPrefab == null || damageTextSpawnPoint == null) return;

        GameObject go = Instantiate(damageTextPrefab, damageTextSpawnPoint.position, Quaternion.identity, damageCanvasTransform);
        DamageText dmg = go.GetComponent<DamageText>();
        // Farbe abhängig vom Angriffstyp
        Color color = Color.white;
        switch (attackType)
        {
            case WeaponDamage.AttackType.HeavyOverhead:
                color = Color.blue;
                break;
            case WeaponDamage.AttackType.Thrust:
                color = Color.yellow;
                break;
            case WeaponDamage.AttackType.Normal:
            default:
                color = Color.white;
                break;
        }

        dmg.ShowDamage(amount, color);
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


    private void Die()
    {
        controller.enabled = false;

        EnableRagdoll();

        // ggf. weitere Tod-Logik
    }

    private void ApplyGravity()
    {
        if (controller == null || !controller.enabled || isDead) return;
        if (controller.isGrounded)
        {
            velocity.y = -1f; // leichter Druck nach unten für Bodenkontakt
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    public void InterruptAnimations()
    {
        if (animator == null) return;

        string[] attackTriggers = { "Attack_Left", "Attack_Right", "Attack_Overhead" };
        foreach (string trigger in attackTriggers)
        {
            animator.ResetTrigger(trigger);
        }

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
        animator.SetBool("InvestigateWalk", false);
        animator.SetBool("isRetreating", false);


        animator.SetBool("DoAttack", false);

        animator.Play("Empty", 0); // ersetzt aktuelle Base-Layer-Animation sofort
    }
}
