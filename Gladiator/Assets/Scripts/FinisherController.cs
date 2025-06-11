using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

/// <summary>
/// Steuert Finisher-Moves des Spielers, abhängig von Gegnerstatus und aktueller Waffe.
/// Fatale & Nicht-Fatale Finisher (Letztere lösen nur eine Reaktion aus).
/// </summary>
public class FinisherController : MonoBehaviour
{
    [Header("References")]
    public Animator playerAnimator;
    [SerializeField] private WeaponHolster weaponHolster;
    [SerializeField] private State playerState;
    [SerializeField] private GameObject cinemachineFreelook;
    [SerializeField] private BoxCollider weaponCollider;
    [SerializeField] private Player player;

    [Header("Finisher Animationen")]
    public WeaponFinishers twoHandedFinishers;
    public WeaponFinishers oneHandedFinishers;
    public WeaponFinishers unarmedFinishers;

    [Header("Finisher Settings")]
    public LayerMask enemyLayerMask;
    [SerializeField] private float finisherRange = 1.8f;
    [SerializeField] private float finisherAngle = 60f;

    private bool isFinishing = false;
    private Enemy currentFinisherTarget;

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        weaponHolster = GetComponent<WeaponHolster>();
        playerState = GetComponent<State>();
    }

    private void Update()
    {
        if (isFinishing || !Input.GetKeyDown(KeyCode.F)) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, finisherRange, enemyLayerMask);
        foreach (Collider col in hits)
        {
            Enemy enemy = col.GetComponentInParent<Enemy>();
            if (enemy == null) continue;

            Vector3 toEnemy = (enemy.transform.position - transform.position).normalized;
            toEnemy.y = 0;

            float angle = Vector3.Angle(transform.forward, toEnemy);
            if (angle <= finisherAngle * 0.5f)
            {
                StartFinisher(enemy);
                break;
            }
        }
    }

    private void StartFinisher(Enemy enemy)
    {
        isFinishing = true;
        currentFinisherTarget = enemy;

        playerState.canMove = false;
        cinemachineFreelook.SetActive(false);
        weaponCollider.isTrigger = true;
        player.enabled = false;

        string chosenFinisher = ChooseFinisher(enemy);
        playerAnimator.SetTrigger(chosenFinisher);
        enemy.StartFinisher(chosenFinisher);

        SnapToFinisherAnchor(enemy.finisherAnchor);

       

    }
    
    private bool IsFatalFinisher(Enemy enemy)
    {
        return enemy.GetHealthPercent() <= enemy.fatalFinisherThreshold;
    }

    private void SnapToFinisherAnchor(Transform anchor)
    {
        if (anchor == null) return;
        transform.position = anchor.position;
        transform.rotation = anchor.rotation;
    }

    public void ResetFinisherState()
    {
        isFinishing = false;
        currentFinisherTarget = null;
        playerState.canMove = true;
        cinemachineFreelook.SetActive(true);
        weaponCollider.isTrigger = true;
        player.enabled = true;
    }

    private string ChooseFinisher(Enemy enemy)
    {
        WeaponFinishers selectedSet = GetCurrentWeaponFinishers();

        if (IsFatalFinisher(enemy) && selectedSet.fatalFinishers.Length > 0)
        {
            return selectedSet.fatalFinishers[Random.Range(0, selectedSet.fatalFinishers.Length)];
        }
        else if (selectedSet.nonFatalFinishers.Length > 0)
        {
            return selectedSet.nonFatalFinishers[Random.Range(0, selectedSet.nonFatalFinishers.Length)];
        }
        else
        {
            Debug.LogWarning("Kein passender Finisher gefunden!");
            return "DefaultFinisher"; // Fallback, damit kein Crash passiert
        }
    }

    private WeaponFinishers GetCurrentWeaponFinishers()
    {
        switch (weaponHolster.currentWeaponType)
        {
            case 2: return twoHandedFinishers;
            case 1: return oneHandedFinishers;
            default: return unarmedFinishers;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, finisherRange);

        Vector3 forward = transform.forward * finisherRange;
        Vector3 left = Quaternion.Euler(0, -finisherAngle / 2, 0) * forward;
        Vector3 right = Quaternion.Euler(0, finisherAngle / 2, 0) * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + left);
        Gizmos.DrawLine(transform.position, transform.position + right);
    }

    public bool IsFinishing()
    {
        return isFinishing;
    }
}
