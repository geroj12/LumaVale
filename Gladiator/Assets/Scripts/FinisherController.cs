using System.Collections;
using System.Linq;
using UnityEngine;
/// <summary>
/// Steuert Finisher-Moves des Spielers, abhängig von Gegnerstatus und aktueller Waffe.
/// </summary>
public class FinisherController : MonoBehaviour
{
    [Header("References")]

    public Animator playerAnimator;
    [SerializeField] private WeaponHolster weaponHolster;
    [SerializeField] private State playerState;

    [Header("Finisher Animationen")]
    public WeaponFinishers twoHandedFinishers;
    public WeaponFinishers oneHandedFinishers;
    public WeaponFinishers unarmedFinishers;
    public bool isFinishing = false;
    private Enemy currentFinisherTarget;
    public LayerMask enemyLayerMask; // Im Inspector zuweisen (z. B. nur "Enemy" aktivieren)
    [SerializeField] private float finisherRange = 1.8f;
    [SerializeField] private float finisherAngle = 60f;
    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        weaponHolster = GetComponent<WeaponHolster>();
        playerState = GetComponent<State>();
    }

    private void Update()
    {
        if (isFinishing) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
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
                    TryStartFinisher(enemy);
                    break;
                }
            }

            Debug.DrawRay(transform.position, transform.forward * finisherRange, Color.green, 1f);
        }
    }

    public void TryStartFinisher(Enemy enemy)
    {
        isFinishing = true;
        currentFinisherTarget = enemy;
        playerState.canMove = false;

        string chosenFinisher = ChooseFinisher(enemy);
        playerAnimator.SetTrigger(chosenFinisher);
        enemy.StartFinisher(chosenFinisher);

        SnapToFinisherAnchor(enemy.finisherAnchor);

        if (!IsFatalFinisher(enemy))
        {
            WeaponDamage weaponDamage = weaponHolster.GetCurrentWeaponDamage();
            float fullDamage = weaponDamage != null ? weaponDamage.damage : 15f;

            float reducedDamage = fullDamage * 0.6f;
            enemy.TakeDamage(reducedDamage, transform.position);
        }
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

    }
    /// <summary>
    /// Wählt den Finisher abhängig von Waffe und Gegnerzustand.
    /// </summary>
    private string ChooseFinisher(Enemy enemy)
    {
        float hpPercent = enemy.GetHealthPercent();

        WeaponFinishers selectedSet = GetCurrentWeaponFinishers();

        if (hpPercent <= enemy.fatalFinisherThreshold)
        {
            return selectedSet.fatalFinishers[Random.Range(0, selectedSet.fatalFinishers.Length)];
        }
        else
        {
            return selectedSet.nonFatalFinishers[Random.Range(0, selectedSet.nonFatalFinishers.Length)];
        }
    }
    public bool IsFinishing()
    {
        return isFinishing;
    }
    /// <summary>
    /// Gibt das passende Finisher-Set für die ausgerüstete Waffe zurück.
    /// </summary>
    private WeaponFinishers GetCurrentWeaponFinishers()
    {
        switch (weaponHolster.currentWeaponType)
        {
            case 2: return twoHandedFinishers;
            case 1: return oneHandedFinishers;
            case 0:
            default: return unarmedFinishers;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, finisherRange);

        // Richtungs-Kegel visualisieren
        Vector3 forward = transform.forward * finisherRange;
        Vector3 left = Quaternion.Euler(0, -finisherAngle / 2, 0) * forward;
        Vector3 right = Quaternion.Euler(0, finisherAngle / 2, 0) * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + left);
        Gizmos.DrawLine(transform.position, transform.position + right);
    }
}
