using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Idle")]

public class EnemyIdleState : EnemyState
{
    public override void Enter()
    {
      

    }

    public override void Tick()
    {
        if (enemy.vision.CanSeeTarget())
        {
            enemy.NotifyPlayerSeen();
            enemy.TransitionTo(enemy.chaseState);

        }
    }

    public override void Exit()
    {

    }
}
