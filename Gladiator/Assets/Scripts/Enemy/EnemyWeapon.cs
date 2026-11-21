
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 20f;

    [Header("References")]
    public Enemy enemy;
    [SerializeField] private TrailRenderer weaponTrail;

    private EnemyCounterWindow enemyCounterWindow;

    private bool canDealDamage = false;

    private void Awake()
    {
        if (enemy == null)
        {
            enemy = GetComponentInParent<Enemy>();
        }

        if (enemy != null)
        {
            enemyCounterWindow = enemy.GetComponent<EnemyCounterWindow>();
        }
    }

    // Damage Control

    public void EnableDamage()
    {
        canDealDamage = true;

        if (weaponTrail != null)
        {
            weaponTrail.Clear();
            weaponTrail.emitting = true;
        }
    }

    public void DisableDamage()
    {
        canDealDamage = false;

        if (weaponTrail != null)
        {
            weaponTrail.emitting = false;
        }
    }

    // Collision Detection

    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage)
            return;

        if (other.CompareTag("PlayerHitbox"))
        {
            HandlePlayerHit(other);
        }

        if (other.CompareTag("PlayerShield"))
        {
            HandlePlayerBlockHit(other);
        }
    }

    // Player Hit

    private void HandlePlayerHit(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();

        if (player == null || enemy == null)
            return;

        player.TakeDamage(damage, enemy.transform.position, false);
    }

    // Player Block

    private void HandlePlayerBlockHit(Collider other)
    {
        if (enemy == null)
            return;

        Player player = other.GetComponentInParent<Player>();

        if (player == null)
            return;

        PlayerCounterWindow playerCounterWindow =
            player.GetComponent<PlayerCounterWindow>();

        bool playerCanCounter = false;
        bool enemyCanBeCountered = false;

        if (playerCounterWindow != null)
        {
            playerCanCounter = playerCounterWindow.IsActive;
        }

        if (enemyCounterWindow != null)
        {
            enemyCanBeCountered = enemyCounterWindow.IsActive;
        }

        // Player successfully counters enemy

        if (playerCanCounter && enemyCanBeCountered)
        {
            enemyCounterWindow.TriggerCounter();

            enemy.InterruptAttack();

            EnemyCombat combat = enemy.GetComponent<EnemyCombat>();

            if (combat != null)
            {
                combat.ApplyStun();
            }
        }
        else
        {
            // Player blocks the attack

            player.TakeDamage(damage, enemy.transform.position, true);

            enemy.InterruptAttack();
        }
    }
}
