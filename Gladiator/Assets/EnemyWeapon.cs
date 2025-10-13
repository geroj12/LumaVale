using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 20f;

    [Header("References")]
    public Enemy enemy;
    [SerializeField] private TrailRenderer weaponTrail;
    [SerializeField] private BoxCollider weaponCollider;

    private EnemyCounterWindow enemyCounterWindow;
    private bool canDealDamage = false;
    private void Awake()
    {
        if (enemy == null)
            enemy = GetComponentInParent<Enemy>();

        if (enemy != null)
            enemyCounterWindow = enemy.GetComponent<EnemyCounterWindow>();
    }
    public void EnableDamage()
    {
        canDealDamage = true;


        if (weaponTrail != null)
        {
            weaponTrail.Clear();          // Trail-Cache löschen (verhindert Artefakte)
            weaponTrail.emitting = true; // ⬅️ Trail aktivieren
        }

    }

    public void DisableDamage()
    {
        canDealDamage = false;
        if (weaponTrail != null)
            weaponTrail.emitting = false; // ⬅️ Trail deaktivieren
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage) return;

        if (other.CompareTag("PlayerHitbox"))
        {
            HandlePlayerHit(other);
        }
        else if (other.CompareTag("PlayerShield"))
        {

            HandlePlayerBlockHit(other);
        }
    }

    private void HandlePlayerHit(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        player.TakeDamage(damage, enemy.transform.position, false);
        weaponCollider.isTrigger = true; //um eventuelle doppel kollisionen zu vermeiden
        Debug.Log("Player hit");
    }

    private void HandlePlayerBlockHit(Collider other)
    {
        if (enemy == null) return;
        Player player = other.GetComponentInParent<Player>();
        if (player == null) return;

        // Zugriff auf das PlayerCounterWindow
        var playerCounterWindow = player.GetComponent<PlayerCounterWindow>();

        bool playerCanCounter = playerCounterWindow != null && playerCounterWindow.IsActive;
        bool enemyIsInCounterWindow = enemyCounterWindow != null && enemyCounterWindow.IsActive;

        // Perfekter Counter: Beide Fenster aktiv
        if (playerCanCounter && enemyIsInCounterWindow)
        {
            enemyCounterWindow.TriggerCounter();
            player.TakeDamage(damage, enemy.transform.position, true);
            enemy.InterruptAttack();

            if (enemy.TryGetComponent(out EnemyCombat combat))
                combat.ApplyStun();

            Debug.Log("<color=lime>PERFECT COUNTER!</color>");
        }
        else
        {
            // Normales Blocken (kein Counter)
            player.TakeDamage(damage, enemy.transform.position, true);
            enemy.InterruptAttack();
            Debug.Log("Player shield hit (no counter)");
        }

        weaponCollider.isTrigger = true;
    }

    void OnTriggerExit(Collider other)
    {
        weaponCollider.isTrigger = false;

    }
}
