using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Investigate")]
public class InvestigateStateAsset : EnemyState
{
    public float investigateDuration = 3f;

    private Vector3 lastKnownPosition;
    public override void Enter(StateMachineEnemy enemy)
    {
        lastKnownPosition = enemy.target.position;
        enemy.animator.SetBool("InvestigateWalk", true);
    }

    public override void Tick(StateMachineEnemy enemy)
    {

        if (enemy.vision.CanSeeTarget())
        {
            enemy.NotifyPlayerSeen();
            enemy.TransitionTo(enemy.chaseState);

        }
        else if (!enemy.HasRecentlySeenPlayer(investigateDuration))
        {
            enemy.TransitionTo(enemy.returnState);

            return;
        }
        enemy.MoveTo(lastKnownPosition, enemy.patrolState is PatrolStateAsset patrol ? patrol.patrolSpeed : 3f);

    }

    public override void Exit(StateMachineEnemy enemy)
    {
        enemy.animator.SetBool("InvestigateWalk", false);

    }
}