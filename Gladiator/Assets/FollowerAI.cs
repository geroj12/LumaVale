using UnityEngine;

public class FollowerAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player; // Referenz zum Spieler
    [SerializeField] private float followRadius = 2.5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask energyLayer; // Layer für Energieobjekte

    private FollowerAnimator followerAnimator;
    private FollowerState state = FollowerState.Idle;
    private Transform targetEnergy;

    private void Awake()
    {
        followerAnimator = GetComponent<FollowerAnimator>();
    }

    private void Update()
    {
        switch (state)
        {
            case FollowerState.Idle:
                IdleBehaviour();
                break;

            case FollowerState.Collecting:
                CollectBehaviour();
                break;

            case FollowerState.Returning:
                ReturnBehaviour();
                break;
        }
    }

    private void IdleBehaviour()
    {
        // Kreist locker um den Spieler
        OrbitAroundPlayer();

        // Suche nach Energie
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, energyLayer);
        if (hits.Length > 0)
        {
            targetEnergy = hits[0].transform;
            state = FollowerState.Collecting;
            followerAnimator.SetMoving(true);
        }
    }

    private void CollectBehaviour()
    {
        if (targetEnergy == null)
        {
            state = FollowerState.Returning;
            return;
        }

        MoveTowards(targetEnergy.position);

        float distance = Vector3.Distance(transform.position, targetEnergy.position);
        if (distance < 1f)
        {
            // Energie eingesammelt (du kannst hier Effekte abspielen)
            Destroy(targetEnergy.gameObject);
            targetEnergy = null;
            state = FollowerState.Returning;
        }
    }

    private void ReturnBehaviour()
    {
        if (player == null)
            return;

        MoveTowards(player.position);

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < followRadius)
        {
            state = FollowerState.Idle;
            followerAnimator.SetMoving(false);
        }
    }

    private void MoveTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.forward = dir;
    }

    private void OrbitAroundPlayer()
    {
        if (player == null) return;

        // einfache Kreisbewegung um den Spieler
        float orbitSpeed = 30f;
        transform.RotateAround(player.position, Vector3.up, orbitSpeed * Time.deltaTime);

        // leichte Hoverbewegung
        float hover = Mathf.Sin(Time.time * 2f) * 0.5f;
        Vector3 desiredPos = (transform.position - player.position).normalized * followRadius + player.position + new Vector3(0, hover, 0);
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * 2f);
    }
}
