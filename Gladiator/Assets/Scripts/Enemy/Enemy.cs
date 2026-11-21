using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private StateMachineEnemy statemachine;
    [SerializeField] private CharacterController controller;

    [Header("Equipment")]
    [SerializeField] private GameObject swordObject;
    [SerializeField] private GameObject shieldObject;
    [SerializeField] private ParticleSystem shieldBreakVFX;

    [Header("Visuals & Effects")]
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private Transform damageTextSpawnPoint;
    [SerializeField] private Transform damageCanvasTransform;
    [SerializeField] private GameObject attachedBloodDecals;
    [SerializeField] private GameObject[] smallBloodEmitters;
    [SerializeField] private GameObject[] finisherBloodEmitters;

    [Header("Finisher Einstellungen")]
    [SerializeField] private float fatalFinisherThreshold = 10f;
    public Transform finisherAnchor;
    [SerializeField] private Player player;

    [Header("Physics")]
    [SerializeField] private float gravity = 9.81f;

    private Vector3 velocity;

    // Internal State
    private bool isFatalFinisher = false;
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;


    private void Awake()
    {
        if (enemyHealth == null)
        {
            enemyHealth = GetComponent<EnemyHealth>();
        }

        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        ragdollColliders = GetComponentsInChildren<Collider>(true);

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }
    }


    private void Start()
    {
        player = FindFirstObjectByType<Player>();

        if (enemyHealth != null)
        {
            enemyHealth.OnDamaged += HandleDamage;
            enemyHealth.OnDeath += HandleDeath;
            enemyHealth.OnShieldBreak += HandleShieldBreak;
            enemyHealth.OnShieldImpact += HandleShieldImpact;
        }
    }


    private void Update()
    {
        if (enemyHealth == null)
        {
            return;
        }

        if (enemyHealth.IsDead)
        {
            if (controller != null)
            {
                controller.enabled = false;
            }
        }
    }


    private void FixedUpdate()
    {
        ApplyGravity();
    }


    // ------------------------------
    // DAMAGE & DEATH
    // ------------------------------

    private void HandleDamage(float amount)
    {
        if (enemyHealth.IsDead)
        {
            return;
        }

        ShowDamageText(amount);
    }


    private void HandleDeath()
    {
        if (enemyHealth.IsDead)
        {
            EnableRagdoll();
            SpawnRandomBloodEmitter(finisherBloodEmitters);
        }
    }


    private void HandleShieldImpact()
    {
        if (animator != null)
        {
            animator.SetTrigger("ShieldImpact");
        }
    }


    private void HandleShieldBreak()
    {
        if (shieldBreakVFX != null)
        {
            shieldBreakVFX.Play();
        }

        if (shieldObject != null)
        {
            ApplyPhysics(shieldObject);
            shieldObject.transform.SetParent(null);
        }
    }


    // ------------------------------
    // VISUALS
    // ------------------------------

    private void ShowDamageText(float amount)
    {
        if (damageTextPrefab == null || damageTextSpawnPoint == null)
        {
            return;
        }

        GameObject damageTextObject = Instantiate(
            damageTextPrefab,
            damageTextSpawnPoint.position,
            Quaternion.identity,
            damageCanvasTransform
        );

        DamageText damageText = damageTextObject.GetComponent<DamageText>();

        if (damageText != null)
        {
            damageText.ShowDamage(amount, Color.white);
        }
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


    private void SpawnRandomBloodEmitter(GameObject[] pool)
    {
        if (pool == null || pool.Length == 0)
        {
            return;
        }

        Transform spawnPoint = transform.Find("BloodPoint");

        if (spawnPoint == null)
        {
            return;
        }

        int index = Random.Range(0, pool.Length);

        Instantiate(
            pool[index],
            spawnPoint.position,
            spawnPoint.rotation
        );

        ActivateBloodDecal();
    }


    private void ActivateBloodDecal()
    {
        if (attachedBloodDecals != null)
        {
            attachedBloodDecals.SetActive(true);
        }
    }


    private void DeactivateBloodDecal()
    {
        if (attachedBloodDecals != null)
        {
            attachedBloodDecals.SetActive(false);
        }
    }


    // ------------------------------
    // FINISHER
    // ------------------------------

    public void StartFinisher(string trigger)
    {
        if (enemyHealth.IsDead)
        {
            return;
        }

        InterruptAnimations();

        if (statemachine != null)
        {
            statemachine.TemporarilyDisableFSM(1f);
        }

        isFatalFinisher = enemyHealth.HealthPercent <= fatalFinisherThreshold;

        if (animator != null)
        {
            animator.SetTrigger(trigger);
        }
    }


    // ------------------------------
    // COMBAT & ANIMATION CONTROL
    // ------------------------------

    public void InterruptAttack()
    {
        InterruptAnimations();

        if (animator != null)
        {
            animator.SetTrigger("HitBlocked");
        }
    }


    public void InterruptAnimations()
    {
        if (animator == null)
        {
            return;
        }

        // Angriffs-Trigger zurücksetzen
        animator.ResetTrigger("Attack_Left");
        animator.ResetTrigger("Attack_Right");
        animator.ResetTrigger("Attack_Overhead");

        // Bewegungs-Animationen stoppen
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
        animator.SetBool("InvestigateWalk", false);
        animator.SetBool("isRetreating", false);
        animator.SetBool("DoAttack", false);

        // Attack-Layer finden
        int attackLayer = animator.GetLayerIndex("Attack");

        // Attack-Animation abbrechen
        if (attackLayer != -1)
        {
            animator.Play("Empty", attackLayer, 0f);
        }
    }


    public void PlayHitReaction(Vector3 attackerPosition)
    {
        if (enemyHealth.IsDead)
        {
            return;
        }

        InterruptAnimations();

        Vector3 toAttacker = attackerPosition - transform.position;
        toAttacker.Normalize();
        toAttacker.y = 0f;

        float angle = Vector3.SignedAngle(
            transform.forward,
            toAttacker,
            Vector3.up
        );

        string trigger = "HitFront";

        if (Mathf.Abs(angle) > 90f)
        {
            trigger = "HitBack";
        }

        animator.SetTrigger(trigger);
    }


    // ------------------------------
    // RAGDOLL
    // ------------------------------

    public void EnableRagdoll()
    {
        DropWeapons();

        if (animator != null)
        {
            animator.enabled = false;
        }

        enemyHealth.currentHP = 0f;

        if (statemachine != null)
        {
            statemachine.enabled = false;
        }

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        foreach (Collider col in ragdollColliders)
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
        if (obj == null)
        {
            return;
        }

        obj.transform.SetParent(null);

        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Collider col = obj.GetComponent<Collider>();

        if (col != null)
        {
            col.isTrigger = false;
        }
    }


    // ------------------------------
    // MOVEMENT / GRAVITY
    // ------------------------------

    private void ApplyGravity()
    {
        if (enemyHealth == null || enemyHealth.IsDead)
        {
            return;
        }

        if (controller == null || !controller.enabled)
        {
            return;
        }

        if (controller.isGrounded)
        {
            velocity.y = -1f;
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }
}