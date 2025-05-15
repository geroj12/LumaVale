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
    void Start()
    {
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

        // Trigger animation
        animator.SetTrigger("Death");

        StartBloodEffect();
        // Continue blood after death
        yield return new WaitForSeconds(finisherDuration);

    }
    private void StartBloodEffect()
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
    private void StopBloodEffect()
    {
        if (activeBloodEmitter != null)
        {
            Destroy(activeBloodEmitter, 5f); // optional: lasse Blut etwas länger laufen
            activeBloodEmitter = null;
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
