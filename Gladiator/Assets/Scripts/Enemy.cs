using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Setup")]
    public Animator animator;

    public GameObject mainCollider;

    [Header("Settings")]
    public float finisherDuration = 3f;
    public bool IsFinisherReady = false;
    public GameObject bloodEmitterPrefab;
    private GameObject activeBloodEmitter;
    [HideInInspector]
    private bool isDead = false;
    public Rigidbody[] ragdollRigidbodies;

    public Player player;
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Call this when enemy becomes vulnerable to finisher
    public void MakeFinisherReady()
    {
        IsFinisherReady = true;
    }

    public void StartFinisher()
    {
        StartCoroutine(HandleFinisher());
    }

    private IEnumerator HandleFinisher()
    {
        IsFinisherReady = false;
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);
        // Trigger animation
        animator.SetTrigger("Death");

        // Continue blood after death
        yield return new WaitForSeconds(finisherDuration);

    }
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
        if (mainCollider != null)
            mainCollider.SetActive(false);

        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }
    }


}
