using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Chase")]
public class ChaseStateAsset : EnemyState
{
    public float chaseSpeed = 5f;
    public float attackRange = 2f;

    public override void Enter(StateMachineEnemy enemy)
    {
        Debug.Log("Enter CHASE");
    }

    public override void Tick(StateMachineEnemy enemy)
    {
        if (!enemy.vision.CanSeeTarget())
        {
            enemy.TransitionTo(enemy.investigateState);
            return;
        }

        float dist = Vector3.Distance(enemy.transform.position, enemy.target.position);
        if (dist <= attackRange)
        {
            enemy.TransitionTo(enemy.attackState);
            return;
        }

        enemy.MoveTo(enemy.target.position, chaseSpeed);
    }

    public override void Exit(StateMachineEnemy enemy)
    {
        Debug.Log("Exit CHASE");
    }
}