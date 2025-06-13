using UnityEngine;

public class FinisherDetector : MonoBehaviour
{
    [SerializeField] private float finisherRange = 1.8f;
    [SerializeField] private float finisherAngle = 60f;
    [SerializeField] private LayerMask enemyLayer;

    public bool TryGetFinisherTarget(out Enemy enemy)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, finisherRange, enemyLayer);
        foreach (var hit in hits)
        {
            Vector3 dir = (hit.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dir) < finisherAngle / 2f)
            {
                if (hit.TryGetComponent(out enemy)) return true;
            }
        }
        enemy = null;
        return false;
    }
}
