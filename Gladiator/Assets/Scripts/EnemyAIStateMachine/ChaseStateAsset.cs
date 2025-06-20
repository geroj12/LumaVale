using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Chase")]
public class ChaseStateAsset : EnemyState
{
    public float chaseSpeed = 5f;
    public float attackRange = 2f;
    public float playerNotInSightDuration = 2f;
    public override void Enter(StateMachineEnemy enemy)
    {
        enemy.SetRunning(true);
    }

    public override void Tick(StateMachineEnemy enemy)
    {
        if (enemy.vision.CanSeeTarget())
        {
            enemy.NotifyPlayerSeen();
            
        }
        else if (!enemy.HasRecentlySeenPlayer(playerNotInSightDuration))
        {
            enemy.TransitionTo(enemy.investigateState);
            return;
        }



        float distance = Vector3.Distance(enemy.transform.position, enemy.target.position);
        if (distance <= attackRange)
        {
            enemy.TransitionTo(enemy.attackState);
            //enemy.FaceTarget(enemy.target);

            return;
        }

        enemy.MoveTo(enemy.target.position, chaseSpeed);
    }

    public override void Exit(StateMachineEnemy enemy)
    {
        enemy.SetRunning(false);

    }
}