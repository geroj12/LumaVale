using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyVision : MonoBehaviour
{
    public Transform target;
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public LayerMask obstacleMask;
    public LayerMask targetMask;

    private bool targetInSight = false;
    private SphereCollider sensorCollider;

    private void Start()
    {
        sensorCollider = GetComponent<SphereCollider>();
        sensorCollider.isTrigger = true;
        sensorCollider.radius = viewDistance;

        StartCoroutine(CheckSightRoutine());
    }

    IEnumerator CheckSightRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);

            if (targetInSight)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                float distToTarget = Vector3.Distance(transform.position, target.position);
                float angle = Vector3.Angle(transform.forward, dirToTarget);

                if (angle <= viewAngle / 2)
                {
                    if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                    {
                        // Sicht frei
                        targetInSight = true;
                        continue;
                    }
                }

                targetInSight = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerStay(other);
    }
    private void OnTriggerStay(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetMask) != 0 && other.transform == target)
        {
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);

            if (angle <= viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    if (!targetInSight)
                    {
                        Debug.Log("Ziel  sichtbar");
                    }
                    targetInSight = true;
                    return;
                }
            }

            if (targetInSight)
            {
                Debug.Log("Ziel nicht  sichtbar");
            }
            targetInSight = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform == target)
        {
            targetInSight = false;
            Debug.Log("Ziel außer Reichweite");
        }
    }
    public bool CanSeeTarget()
    {
        return targetInSight;
    }
    private void OnDrawGizmosSelected()
    {
        // Sichtweite als gelbe Kugel
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        // Sichtfeldkegel als blaue Linien
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward * viewDistance;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward * viewDistance;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // Linie zum Ziel (grün wenn sichtbar, rot wenn verdeckt)
        if (target != null)
        {
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float distToTarget = Vector3.Distance(transform.position, target.position);
            float angle = Vector3.Angle(transform.forward, dirToTarget);

            if (distToTarget <= viewDistance && angle <= viewAngle / 2)
            {
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawLine(transform.position, target.position);
            }
        }
    }
}
