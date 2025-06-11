using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("Setup")]
    public Animator animator;
    [SerializeField] private GameObject swordObject;

    [Header("Shield System")]
    public bool hasShield = true;
    public float maxShieldDurability = 100f;
    public float currentShieldDurability;

    public GameObject shieldObject;          // dein Schild als Kindobjekt
    public Rigidbody shieldRigidbody;        // am Schildobjekt
    public Collider shieldCollider;          // am Schildobjekt

    public GameObject damageTextPrefab;
    public Transform damageTextSpawnPoint;
    [SerializeField] private CharacterController controller;
    private Vector3 velocity;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private StateMachineEnemy statemachine;

    [Header("Settings")]
    public float finisherDuration = 3f;
    public bool IsFinisherReady = false;
    private bool isFatalFinisher = false;
    public GameObject bloodEmitterPrefab;
    private GameObject activeBloodEmitter;
    [SerializeField] private GameObject attachedBloodDecals;

    public bool isStunned = false;
    [SerializeField] private Transform damageCanvasTransform;


    [Header("Finisher Einstellungen")]
    public float fatalFinisherThreshold = 10f; // Prozent
    public bool isDead = false;
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;
    public Player player;
    public Transform finisherAnchor;


    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP = 100f;

    [Header("Hit Reaction Settings")]
    public float hitReactionCooldown = 0.5f;
    private float lastHitTime = -999f;

    void Awake()
    {
        // Suche alle Rigidbodies & Collider INKLUSIVE deaktivierter, untergeordneten Bones
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        ragdollColliders = GetComponentsInChildren<Collider>(true);
        // Initial Ragdoll deaktivieren
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }
    }
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        currentShieldDurability = maxShieldDurability;

    }
    void FixedUpdate()
    {
        ApplyGravity();
    }
    // =====================
    // === FINISHER LOGIK ==
    // =====================
    public void StartFinisher(string finisherTrigger)
    {
        if (isDead) return;
        InterruptAnimations();
        statemachine?.TemporarilyDisableFSM(1f);
        float hpPercent = GetHealthPercent();
        isFatalFinisher = hpPercent <= fatalFinisherThreshold;

        animator.SetTrigger(finisherTrigger);

        StartCoroutine(HandleFinisher());
    }

    private IEnumerator HandleFinisher()
    {
        IsFinisherReady = false;

        // Spieler anschauen
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        yield return new WaitForSeconds(finisherDuration);

    }
    public float GetHealthPercent()
    {
        return (float)currentHP / maxHP * 100f;
    }

    //Wird über Animation Event aufgerufen
    public void StartBloodEffect()
    {
        if (bloodEmitterPrefab != null)
        {
            // Blutemitter instanziieren an gewünschter Stelle (z.B. Hals)
            Transform bloodSpawnPoint = transform.Find("BloodPoint"); // Leeres GameObject z. B. am Hals
            if (bloodSpawnPoint == null)
            {
                Debug.LogWarning("Missing BloodPoint child on enemy!");
                return;
            }

            activeBloodEmitter = Instantiate(bloodEmitterPrefab, bloodSpawnPoint.position, bloodSpawnPoint.rotation, bloodSpawnPoint);
            ActivateBloodDecal();
        }
    }
    private void ActivateBloodDecal()
    {
        attachedBloodDecals.SetActive(true);
    }
    // =============================
    // === RAGDOLL & WAFFEN DROP ===
    // =============================
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

    // ==========================
    // === DAMAGE & REACTIONS ===
    // ==========================
    public void TakeDamage(float amount, Vector3 attackerPosition, bool hitShield = false, WeaponDamage.AttackType attackType = WeaponDamage.AttackType.Normal)
    {
        if (isDead) return;
        statemachine?.TemporarilyDisableFSM(1f);
        InterruptAnimations();

        if (hitShield)
        {
            animator.SetTrigger("ShieldImpact");

            // ↓↓↓ Schild-Durability reduzieren
            currentShieldDurability -= amount;

            // Optional Schild-Break prüfen
            if (currentShieldDurability <= 0f)
            {
                BreakShield();
            }

            return;
        }
        // ↓↓↓ Echter Schaden an Leben
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

    private void BreakShield()
    {
        if (!hasShield || shieldObject == null) return;

        // 1. Physisches Schild aktivieren
        shieldObject.transform.SetParent(null); // Deparenten
        ApplyPhysics(shieldObject);


        // 2. Zustand ändern
        hasShield = false;
        currentShieldDurability = 0;

        // 3. Eventuell Audio oder VFX abspielen
        // shieldBreakVFX.Play(); etc. (optional)

        // 4. State-Machine zu aggressiverem Verhalten wechseln
        if (statemachine != null && statemachine.aggressiveIdleState)
        {
            statemachine.TransitionTo(statemachine.aggressiveIdleState);
        }

        // 5. Optionale Animation triggern
        animator.SetTrigger("ShieldBreak");
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
                color = Color.red; // z. B. von oben
                break;
            case WeaponDamage.AttackType.Thrust:
                color = Color.yellow; // z. B. von unten
                break;
            case WeaponDamage.AttackType.Normal:
            default:
                color = Color.white; // links/rechts oder Standard
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

        // 🧼 Alle potenziellen Angriffstrigger zurücksetzen
        string[] attackTriggers = { "Attack_Left", "Attack_Right", "Attack_Overhead" };
        foreach (string trigger in attackTriggers)
        {
            animator.ResetTrigger(trigger);
        }

        // 💨 Bewegungsbooleans zurücksetzen
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
        animator.SetBool("InvestigateWalk", false);
        animator.SetBool("isRetreating", false);



        // 💣 Angriff abbrechen (optional – falls du ein "isAttacking"-Bool verwendest)
        animator.SetBool("DoAttack", false);

        // Optional: Override-Layer-Safety (z. B. falls du blendest)
        animator.Play("Empty", 0); // ersetzt aktuelle Base-Layer-Animation sofort
    }
}
