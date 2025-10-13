using Unity.Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Movement movement;
    public Animator animator;
    [SerializeField] private Combat combat;
    [SerializeField] private State state;
    public GameObject damageTextPrefab;
    public Transform damageTextSpawnPoint;
    [SerializeField] private Transform damageCanvasTransform;
    [SerializeField] private GameObject[] smallBloodEmitters;
    private GameObject activeBloodEmitter;

    [SerializeField] private GameObject attachedBloodDecals;
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP;

    [Header("Hit Reaction Settings")]
    public float hitReactionCooldown = 0.5f;
    private float lastHitTime = -999f;

    [Header("Shield System")]
    public bool hasShield = true;
    public float maxShieldDurability = 100f;
    public float currentShieldDurability;

    [SerializeField] private CombatDirectionHandler visualFollower;
    [SerializeField] private CinemachineCamera cinemachineCamera;
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
    public void StartBloodEffect()
    {
        DeactivateBloodDecal();
        SpawnRandomBloodEmitter(smallBloodEmitters);
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
        ShowDamageText(amount);
        if (currentHP <= 0f)
        {
            animator.SetTrigger("isDead");
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

    public void OnPlayerDeath()
    {
        visualFollower.SpawnVisualFollower();
        Debug.Log("Player dead");
        cinemachineCamera.GetComponent<CinemachineCamera>().Priority = 0;
        gameObject.SetActive(false);

    }

    private void ShowDamageText(float amount)
    {
        if (damageTextPrefab == null || damageTextSpawnPoint == null) return;

        GameObject go = Instantiate(damageTextPrefab, damageTextSpawnPoint.position, Quaternion.identity, damageCanvasTransform);
        DamageText dmg = go.GetComponent<DamageText>();

        dmg.ShowDamage(amount, Color.red);
    }
}
