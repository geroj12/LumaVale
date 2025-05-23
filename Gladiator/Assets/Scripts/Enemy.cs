using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("Setup")]
    public Animator animator;
    public CapsuleCollider mainCollider;
    [SerializeField] private GameObject swordObject;
    [SerializeField] private GameObject shieldObject;
    public GameObject damageTextPrefab;
    public Transform damageTextSpawnPoint;
    [Header("Settings")]
    public float finisherDuration = 3f;
    public bool IsFinisherReady = false;
    private bool isFatalFinisher = false;
    public GameObject bloodEmitterPrefab;
    private GameObject activeBloodEmitter;
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

    }

    // =====================
    // === FINISHER LOGIK ==
    // =====================
    public void StartFinisher(string finisherTrigger)
    {
        if (isDead) return;

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
        }
    }

    // =============================
    // === RAGDOLL & WAFFEN DROP ===
    // =============================
    public void EnableRagdoll()
    {

        isDead = true;

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

        if (mainCollider != null) mainCollider.enabled = false;

    }

    public void DropWeapons()
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
        if (collider != null) collider.enabled = true;

    }

    // ==========================
    // === DAMAGE & REACTIONS ===
    // ==========================
    public void TakeDamage(float amount, Vector3 attackerPosition, bool hitShield = false)
    {
        if (isDead) return;

        if (hitShield)
        {
            animator.SetTrigger("ShieldImpact");
            return;
        }
        currentHP -= amount;

        // Optional: Cooldown für Reaktionsanimation
        if (Time.time - lastHitTime >= hitReactionCooldown)
        {
            lastHitTime = Time.time;
            PlayHitReaction(attackerPosition);
        }
        ShowDamageText(amount); // hinzufügen

        if (currentHP <= 0f)
        {
            Die();
        }
    }
    private void ShowDamageText(float amount)
    {
        if (damageTextPrefab == null || damageTextSpawnPoint == null) return;

        GameObject go = Instantiate(damageTextPrefab, damageTextSpawnPoint.position, Quaternion.identity,damageCanvasTransform);
        DamageText dmg = go.GetComponent<DamageText>();

        // Optional: Unterschiedliche Farben
        Color color = Color.white;
        if (amount > 30) color = Color.red;
        else if (amount < 10) color = Color.yellow;

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
        EnableRagdoll();
        DropWeapons();
        // ggf. weitere Tod-Logik
    }
}
