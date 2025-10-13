
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("Setup")]
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyCombat combat;
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


    void Awake()
    {
        if (!enemyHealth)
            enemyHealth = GetComponent<EnemyHealth>();

        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        ragdollColliders = GetComponentsInChildren<Collider>(true);

        foreach (var rb in ragdollRigidbodies)
            rb.isKinematic = true;

    }
    void Start()
    {
        player = FindFirstObjectByType<Player>();

        // Event subscriptions
        enemyHealth.OnDamaged += HandleDamage;
        enemyHealth.OnDeath += HandleDeath;
        enemyHealth.OnShieldBreak += HandleShieldBreak;
        enemyHealth.OnShieldImpact += HandleShieldImpact;
        
    }



    void FixedUpdate()
    {
        ApplyGravity();
    }

    // ------------------------------
    // EVENT HANDLERS
    // ------------------------------
    private void HandleDamage(float amount)
    {
        if (enemyHealth.IsDead) return;
        ShowDamageText(amount);
        StartBloodEffect();
    }

    private void HandleDeath()
    {
        if (enemyHealth.IsDead)
        {
            EnableRagdoll();
            statemachine.enabled = false;
            animator.enabled = false;
        }
    }
    private void HandleShieldImpact()
    {
        animator.SetTrigger("ShieldImpact");
    }
    private void HandleShieldBreak()
    {
        if (shieldBreakVFX != null)
            shieldBreakVFX.Play();

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
        if (!damageTextPrefab || !damageTextSpawnPoint) return;

        var go = Instantiate(damageTextPrefab, damageTextSpawnPoint.position, Quaternion.identity, damageCanvasTransform);
        if (go.TryGetComponent(out DamageText dmg))
            dmg.ShowDamage(amount, Color.white);
    }

    public void StartBloodEffect()
    {
        DeactivateBloodDecal();
        SpawnRandomBloodEmitter(smallBloodEmitters);
    }

    private void SpawnRandomBloodEmitter(GameObject[] pool)
    {
        if (pool == null || pool.Length == 0) return;

        Transform spawnPoint = transform.Find("BloodPoint");
        if (!spawnPoint) return;

        int index = Random.Range(0, pool.Length);
        Instantiate(pool[index], spawnPoint.position, spawnPoint.rotation);
        ActivateBloodDecal();
    }

    private void ActivateBloodDecal() => attachedBloodDecals?.SetActive(true);
    private void DeactivateBloodDecal() => attachedBloodDecals?.SetActive(false);

    // ------------------------------
    // FINISHER
    // ------------------------------
    public void StartFinisher(string trigger)
    {
        if (enemyHealth.IsDead) return;

        InterruptAnimations();
        statemachine?.TemporarilyDisableFSM(1f);

        isFatalFinisher = enemyHealth.HealthPercent <= fatalFinisherThreshold;

        animator.SetTrigger(trigger);
    }

    // ------------------------------
    // COMBAT & ANIMATION CONTROL
    // ------------------------------
    public void InterruptAttack()
    {
        if (!combat)
            combat = GetComponent<EnemyCombat>();

        if (combat)
            combat.StartCoroutine(combat.ResetAttackBools());

        animator.SetTrigger("HitBlocked");
    }

    public void InterruptAnimations()
    {
        if (!animator) return;

        string[] triggers = { "Attack_Left", "Attack_Right", "Attack_Overhead" };
        foreach (string t in triggers)
            animator.ResetTrigger(t);

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
        animator.SetBool("InvestigateWalk", false);
        animator.SetBool("isRetreating", false);
        animator.SetBool("DoAttack", false);

        animator.Play("Empty", 0);
    }

    public void PlayHitReaction(Vector3 attackerPosition)
    {
        if (enemyHealth.IsDead) return;

        Vector3 toAttacker = (attackerPosition - transform.position).normalized;
        toAttacker.y = 0;

        float angle = Vector3.SignedAngle(transform.forward, toAttacker, Vector3.up);
        string trigger = Mathf.Abs(angle) <= 90f ? "HitFront" : "HitBack";
        animator.SetTrigger(trigger);
    }

    // ------------------------------
    // RAGDOLL
    // ------------------------------
    public void EnableRagdoll()
    {
        DropWeapons();
        animator.enabled = false;

        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        foreach (var col in ragdollColliders)
            col.enabled = true;
    }

    private void DropWeapons()
    {
        ApplyPhysics(swordObject);
        ApplyPhysics(shieldObject);
    }

    private void ApplyPhysics(GameObject obj)
    {
        if (!obj) return;

        obj.transform.SetParent(null);
        if (obj.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        if (obj.TryGetComponent(out Collider col))
            col.isTrigger = false;
    }

    // ------------------------------
    // MOVEMENT / GRAVITY
    // ------------------------------
    private void ApplyGravity()
    {
        if (!controller || !controller.enabled || enemyHealth.IsDead) return;

        velocity.y = controller.isGrounded ? -1f : velocity.y - gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

}
