using System.Collections;
using UnityEngine;

public class FollowerVision : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float viewDistance = 10f;
    [SerializeField] private float viewAngle = 360f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private LayerMask energyMask;

    [Header("Feedback")]
    [SerializeField] private FMODAlert alertSound;
    [SerializeField] private ParticleSystem alertVFX;

    private bool alertTriggered = false;

    // Informationen, die die AI auslesen kann
    [HideInInspector] public Transform detectedEnemy;
    [HideInInspector] public Transform detectedEnergy;

    private void Start()
    {

        StartCoroutine(VisionRoutine());
    }

    private IEnumerator VisionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            ScanForTargets();
        }
    }

    private void ScanForTargets()
    {
        detectedEnemy = DetectClosest(enemyMask);
        detectedEnergy = DetectClosest(energyMask);

        if (detectedEnemy != null)
        {
            if (!alertTriggered)
            {
                alertTriggered = true;
                if (alertSound != null) alertSound.PlayAlertSound();
                if (alertVFX != null) alertVFX.Play();
            }
        }
        else
        {
            if (alertTriggered)
            {
                alertTriggered = false;
                if (alertVFX != null) alertVFX.Stop();
            }
        }
    }

    private Transform DetectClosest(LayerMask mask)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, viewDistance, mask);
        Transform closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            Vector3 dir = (hit.transform.position - transform.position).normalized;
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            float angle = Vector3.Angle(transform.forward, dir);

            if (angle <= viewAngle / 2)
            {
                if (!Physics.Raycast(transform.position, dir, dist, obstacleMask))
                {
                    if (dist < closestDist)
                    {
                        closest = hit.transform;
                        closestDist = dist;
                    }
                }
            }
        }

        return closest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }

}
