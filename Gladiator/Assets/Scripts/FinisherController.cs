using System.Collections;
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
    private bool isFinishing = false;
    private Enemy currentFinisherTarget;

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        weaponHolster = GetComponent<WeaponHolster>();
    }

    private void Update()
    {
        if (isFinishing) return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1.4f))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    TryStartFinisher(enemy);
                }
            }
        }
    }


    public void TryStartFinisher(Enemy enemy)
    {
        isFinishing = true;
        currentFinisherTarget = enemy;

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
