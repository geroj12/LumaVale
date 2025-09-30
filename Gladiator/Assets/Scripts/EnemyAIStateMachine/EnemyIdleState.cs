using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Idle")]

public class EnemyIdleState : EnemyState
{
    public override void Enter(StateMachineEnemy enemy)
    {
      

    }

    public override void Tick(StateMachineEnemy enemy)
    {
        if (enemy.vision.CanSeeTarget())
        {
            enemy.NotifyPlayerSeen();
            enemy.TransitionTo(enemy.chaseState);

        }
    }

    public override void Exit(StateMachineEnemy enemy)
    {

    }
}
