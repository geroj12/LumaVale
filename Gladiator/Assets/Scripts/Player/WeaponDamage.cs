using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public float damage = 20f;
    private float finalDamage;

    public Player player;
    [SerializeField] private TrailRenderer weaponTrail;

    private bool canDealDamage = false;
    private HashSet<Enemy> damagedEnemies = new HashSet<Enemy>();
    public enum AttackType { Normal, HeavyOverhead, Thrust }

    private AttackType currentAttackType = AttackType.Normal;
    [SerializeField] private float heavyAttackDmgModifier;
    private State playerState;

    public void SetAttackType(AttackType type)
    {
        currentAttackType = type;
    }
    public void EnableDamage(State state)
    {
        canDealDamage = true;
        damagedEnemies.Clear();
        playerState = state;

        if (weaponTrail != null)
        {
            weaponTrail.Clear();          // Trail-Cache löschen (verhindert Artefakte)
            weaponTrail.emitting = true; // ⬅️ Trail aktivieren
        }

        CalculateDamage();
    }

    public void DisableDamage()
    {
        canDealDamage = false;
        if (weaponTrail != null)
            weaponTrail.emitting = false; // ⬅️ Trail deaktivieren
    }
    private void CalculateDamage()
    {
        if (playerState == null)
        {
            Debug.LogWarning("PlayerState ist null! Schaden basiert nur auf baseDamage.");
            finalDamage = damage;
            return;
        }

        float strength = playerState.strength;
        float multiplier = 1f;

        switch (currentAttackType)
        {
            case AttackType.Normal:
                multiplier = 1.0f;
                break;
            case AttackType.HeavyOverhead:
                multiplier = heavyAttackDmgModifier;
                break;
            case AttackType.Thrust:
                multiplier = heavyAttackDmgModifier;
                break;
        }

        float strengthFactor = 1f + (strength * 0.2f);
        finalDamage = damage * strengthFactor * multiplier;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage) return;
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !damagedEnemies.Contains(enemy))
            {
                damagedEnemies.Add(enemy);
                if (enemy.TryGetComponent(out EnemyHealth health))
                {
                    health.ApplyDamage(finalDamage, player.transform.position, false, currentAttackType);
                }
                canDealDamage = false; // Nur 1x pro Schlag
            }
        }
        else if (other.CompareTag("EnemyShield"))
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                float shieldDamage = finalDamage * 0.5f; // z.B. nur 50% Schaden auf Schild
                if (enemy.TryGetComponent(out EnemyHealth health))
                {
                    health.ApplyDamage(shieldDamage, player.transform.position, true, currentAttackType);
                }
                player.InterruptAttack(); // Bricht Angriffsanimation ab
                DisableDamage();
            }
        }
    }
}
