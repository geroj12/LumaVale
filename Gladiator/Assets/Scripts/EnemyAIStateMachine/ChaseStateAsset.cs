using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Chase")]
public class ChaseStateAsset : EnemyState
{
    public float chaseSpeed = 5f;
    public float attackRange = 2f;
    public override void Enter(StateMachineEnemy enemy)
    {
        enemy.animator.SetBool("IsRunning", true);
    }

    public override void Tick(StateMachineEnemy enemy)
    {
        if (!enemy.vision.CanSeeTarget())
        {
            enemy.TransitionTo(enemy.investigateState); // z. B. zurück zur Patrouille
            return;
        }

        float distance = Vector3.Distance(enemy.transform.position, enemy.target.position);
        if (distance <= attackRange)
        {
            enemy.animator.SetBool("IsRunning", false);
            enemy.TransitionTo(enemy.attackState);
            return;
        }

        enemy.MoveTo(enemy.target.position, chaseSpeed);
    }

    public override void Exit(StateMachineEnemy enemy)
    {
        enemy.animator.SetBool("IsRunning", false);

    }
}