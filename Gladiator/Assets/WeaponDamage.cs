using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public float damage = 20f;
    public Player player;
    [SerializeField] private BoxCollider weaponCollider;
    [SerializeField] private TrailRenderer weaponTrail;

    private bool canDealDamage = false;
    private HashSet<Enemy> damagedEnemies = new HashSet<Enemy>();
    public enum AttackType { Normal, HeavyOverhead, Thrust }

    private AttackType currentAttackType = AttackType.Normal;
    [SerializeField] private float heavyAttackDmgModifier;

    public void SetAttackType(AttackType type)
    {
        currentAttackType = type;
    }
    public void EnableDamage()
    {
        canDealDamage = true;
        damagedEnemies.Clear();

        weaponCollider.isTrigger = true;
        if (weaponTrail != null)
        {
            weaponTrail.Clear();          // Trail-Cache löschen (verhindert Artefakte)
            weaponTrail.emitting = true; // ⬅️ Trail aktivieren
        }

        switch (currentAttackType)
        {
            case AttackType.HeavyOverhead:
                damage *= heavyAttackDmgModifier;
                break;
            case AttackType.Thrust:
                damage *= heavyAttackDmgModifier; 
               
                break;
            case AttackType.Normal:
                
                break;
        }
    }

    public void DisableDamage()
    {
        canDealDamage = false;
        weaponCollider.isTrigger = false;
        if (weaponTrail != null)
            weaponTrail.emitting = false; // ⬅️ Trail deaktivieren
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
                enemy.TakeDamage(damage, player.transform.position);
                canDealDamage = false; // Nur 1x pro Schlag
            }
        }
        else if (other.CompareTag("EnemyShield"))
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(0f, player.transform.position, true);
                player.InterruptAttack(); // Bricht Angriffsanimation ab
                DisableDamage();
            }
        }
    }
}
