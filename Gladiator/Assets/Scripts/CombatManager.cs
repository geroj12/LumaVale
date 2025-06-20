using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    private List<StateMachineEnemy> enemies = new();
    private StateMachineEnemy currentAttacker;
    private float lastAttackTime = -Mathf.Infinity;
    public float attackCooldown = 2f; // Cooldown, bevor der nächste angreifen darf

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Register(StateMachineEnemy enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void Unregister(StateMachineEnemy enemy)
    {
        enemies.Remove(enemy);
        if (currentAttacker == enemy)
        {
            currentAttacker = null;
        }
    }

    public bool CanAttack(StateMachineEnemy enemy)
    {
        if (currentAttacker == null && Time.time - lastAttackTime >= attackCooldown)
        {
            currentAttacker = enemy;
            lastAttackTime = Time.time;
            return true;
        }

        return currentAttacker == enemy;
    }

    public void NotifyAttackFinished(StateMachineEnemy enemy)
    {
        if (currentAttacker == enemy)
        {
            currentAttacker = null;
        }
    }

    public int GetEnemyIndex(StateMachineEnemy enemy)
    {
        return enemies.IndexOf(enemy);
    }

    public Vector3 GetFlankPosition(StateMachineEnemy enemy, Transform player)
    {
        int index = GetEnemyIndex(enemy);
        float angle = (index % 3 - 1) * 45f; // -45°, 0°, +45°
        Vector3 offset = Quaternion.Euler(0, angle, 0) * -player.forward;
        return player.position + offset.normalized * 2.5f;
    }
}
