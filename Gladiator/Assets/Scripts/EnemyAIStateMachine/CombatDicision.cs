
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Decisions/Combat Decision")]
public class CombatDecision : ScriptableObject
{
    [Header("Settings")]
    [SerializeField] private float attackRange = 2f;

    [Header("Action States")]
    [SerializeField] private EnemyState attackState;
    [SerializeField] private EnemyState blockState;
    [SerializeField] private EnemyState dodgeState;

    [Header("Action Chances")]
    [Range(0, 100)]
    [SerializeField] private int attackChance = 60;

    [Range(0, 100)]
    [SerializeField] private int blockChance = 25;

    [Range(0, 100)]
    [SerializeField] private int dodgeChance = 15;

    public void Evaluate(StateMachineEnemy enemy)
    {
        if (enemy == null || enemy.target == null)
            return;

        if (!(enemy.CurrentState is EnemyCombatIdleState))
            return;

        float distance = Vector3.Distance(
            enemy.transform.position,
            enemy.target.position
        );

        if (distance > attackRange)
        {
            enemy.TransitionTo(enemy.chaseState);
            return;
        }

        int randomNumber = Random.Range(0, 100);

        if (randomNumber < attackChance)
        {
            if (attackState != null)
                enemy.TransitionTo(attackState);
        }
        else if (randomNumber < attackChance + blockChance)
        {
            if (blockState != null)
                enemy.TransitionTo(blockState);
        }
        else
        {
            if (dodgeState != null)
                enemy.TransitionTo(dodgeState);
        }
    }
}
