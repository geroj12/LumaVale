using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Combat/Retreat")]
public class EnemyCombatRetreatState : EnemyState
{
    public float retreatDistance = 4f;
    public float retreatSpeed = 3.5f;
    public float maxRetreatTime = 2f;

    private Vector3 retreatTarget;
    private float exitTime;

    public override void Enter(StateMachineEnemy enemy)
    {
        Vector3 awayFromPlayer = (enemy.transform.position - enemy.target.position).normalized;
        retreatTarget = enemy.transform.position + awayFromPlayer * retreatDistance;
        retreatTarget.y = enemy.transform.position.y;

        exitTime = Time.time + maxRetreatTime;
        enemy.animator.SetBool("isRetreating",true);
        //enemy.animator.SetBool("IsWalking", true);

    }

    public override void Tick(StateMachineEnemy enemy)
    {
        enemy.RetreatMove(retreatTarget, retreatSpeed, enemy.target);

        float dist = Vector3.Distance(enemy.transform.position, retreatTarget);
        if (dist < 0.5f || Time.time >= exitTime)
        {
            enemy.TransitionTo(enemy.combatIdleState);
        }
    }

    public override void Exit(StateMachineEnemy enemy)
    {
        enemy.animator.SetBool("isRetreating", false);
        // Optionale Aufräumarbeiten
    }
}
