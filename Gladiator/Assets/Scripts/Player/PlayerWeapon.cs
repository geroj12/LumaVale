using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float baseDamage = 20f;
    [SerializeField] private float heavyAttackModifier = 1.5f;

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private TrailRenderer weaponTrail;

    private readonly HashSet<Enemy> damagedEnemies = new HashSet<Enemy>();
    private bool canDealDamage = false;

    public enum AttackType { Normal, HeavyOverhead, Thrust }
    private AttackType currentAttackType = AttackType.Normal;
    private float finalDamage;
    private State playerState;

    // -----------------------------------
    // PUBLIC API
    // -----------------------------------
    public void SetAttackType(AttackType type) => currentAttackType = type;

    public void EnableDamage(State state)
    {
        canDealDamage = true;
        damagedEnemies.Clear();
        playerState = state;

        if (weaponTrail != null)
        {
            weaponTrail.Clear();
            weaponTrail.emitting = true;
        }

        CalculateFinalDamage();
    }

    public void DisableDamage()
    {
        canDealDamage = false;
        if (weaponTrail != null)
            weaponTrail.emitting = false;
    }
    // -----------------------------------
    // DAMAGE CALCULATION
    // -----------------------------------
    private void CalculateFinalDamage()
    {
        float strength = playerState != null ? playerState.strength : 0f;
        float strengthFactor = 1f + (strength * 0.2f);

        float modifier = currentAttackType switch
        {
            AttackType.HeavyOverhead => heavyAttackModifier,
            AttackType.Thrust => heavyAttackModifier,
            _ => 1f
        };

        finalDamage = baseDamage * strengthFactor * modifier;
    }

    // -----------------------------------
    // COLLISION LOGIC
    // -----------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage) return;

        if (other.CompareTag("Enemy"))
        {
            HandleEnemyHit(other);
        }
        else if (other.CompareTag("EnemyShield"))
        {
            HandleShieldHit(other);
        }
    }

    private void HandleEnemyHit(Collider other)
    {
        if (!other.TryGetComponent(out Enemy enemy) || damagedEnemies.Contains(enemy))
            return;

        damagedEnemies.Add(enemy);


        var enemyCombat = enemy.GetComponent<EnemyCombat>();
        var enemyCounter = enemy.GetComponent<EnemyCounterWindow>();

        enemyCombat.TryCounterPlayerAttack();
        bool enemyCanCounter = enemyCounter != null && enemyCounter.IsActive;


        if (enemyCanCounter)
        {
            // Enemy kontert erfolgreich!
            Debug.Log("<color=red>Enemy COUNTERED the player!</color>");
            var playerCounter = player.GetComponent<PlayerCounterWindow>();
            playerCounter.TriggerCounter();
            player.GetComponent<Animator>()?.SetTrigger("CounteredByEnemy");

            return;
        }

        if (enemy.TryGetComponent(out EnemyHealth health))
        {
            health.ApplyDamage(finalDamage, player.transform.position, false, currentAttackType);
        }

        canDealDamage = false;
    }

    private void HandleShieldHit(Collider other)
    {

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;


        float shieldDamage = finalDamage * 0.5f; // z. B. nur 50% Schaden auf Schilde

        if (enemy.TryGetComponent(out EnemyHealth health))
            health.ApplyDamage(shieldDamage, player.transform.position, true, currentAttackType);

        // Bricht Spielerangriff bei Schildblock ab
        player.InterruptAttack();

        DisableDamage();


    }
}
