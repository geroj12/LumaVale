using System.Collections;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Setup")]
    public Animator animator;

    public CapsuleCollider mainCollider;
    public Rigidbody mainRigidbody;     // z.B. das Rigidbody des Root GameObjects


    [Header("Settings")]
    public float finisherDuration = 3f;
    public bool IsFinisherReady = false;
    private bool isFatalFinisher = false;

    public GameObject bloodEmitterPrefab;
    private GameObject activeBloodEmitter;

    [Header("Finisher Einstellungen")]
    public float fatalFinisherThreshold = 10f; // Prozent
    private bool isDead = false;
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;


    public Player player;

    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP = 100f;

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

        //Rotation zum spieler
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        // Continue blood after death
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


}
