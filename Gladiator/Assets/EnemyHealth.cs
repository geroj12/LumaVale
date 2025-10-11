using UnityEngine;
using UnityEngine.Events;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP;
    public float fatalFinisherThreshold = 10f;

    [Header("Shield Settings")]
    [SerializeField] private bool hasShield = true;
    [SerializeField] private float maxShieldDurability = 30f;
    [SerializeField] private float currentShieldDurability;

    public float HealthPercent => (currentHP / maxHP) * 100f;
    public bool IsDead => currentHP <= 0f;

    // --- Events ---
    public event Action<float> OnDamaged;
    public event Action OnDeath;
    public event Action OnShieldBreak;
    public event Action OnShieldImpact;

    private Enemy enemy; // Referenz zur Hauptlogik
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        currentHP = maxHP;
        currentShieldDurability = maxShieldDurability;
    }
    public void ApplyDamage(float amount, Vector3 attackerPosition, bool hitShield = false, WeaponDamage.AttackType attackType = WeaponDamage.AttackType.Normal)
    {
        if (IsDead) return;

        if (hitShield && hasShield)
        {
            OnShieldImpact.Invoke();
            currentShieldDurability -= amount;
            if (currentShieldDurability <= 0f)
            {
                hasShield = false;
                OnShieldBreak?.Invoke();
            }
            return;
        }

        currentHP -= amount;
        currentHP = Mathf.Max(0f, currentHP);

        OnDamaged?.Invoke(amount);
        enemy.PlayHitReaction(attackerPosition);
        if (IsDead)
            OnDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        currentHP = Mathf.Min(currentHP + amount, maxHP);
    }
}
