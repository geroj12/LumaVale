using UnityEngine;
/// <summary>
/// Steuert Finisher-Moves des Spielers, abhängig von Gegnerstatus und aktueller Waffe.
/// </summary>
public class FinisherController : MonoBehaviour
{
    [Header("References")]
    public Animator playerAnimator;
    [SerializeField] private WeaponHolster weaponHolster;

    [Header("Finisher Animationen")]
    public WeaponFinishers twoHandedFinishers;
    public WeaponFinishers oneHandedFinishers;
    public WeaponFinishers unarmedFinishers;

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        weaponHolster = GetComponent<WeaponHolster>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1.3f))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    TryStartFinisher(enemy);
                }
            }
        }
    }

    /// <summary>
    /// Startet den passenden Finisher gegen den Gegner.
    /// </summary>
    public void TryStartFinisher(Enemy enemy)
    {
        string chosenFinisher = ChooseFinisher(enemy);
        playerAnimator.SetTrigger(chosenFinisher);
        enemy.StartFinisher(chosenFinisher);

        // Richte den Spieler zum Gegner aus
        Vector3 toEnemy = (enemy.transform.position - transform.position).normalized;
        toEnemy.y = 0;
        transform.forward = toEnemy;
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
}
