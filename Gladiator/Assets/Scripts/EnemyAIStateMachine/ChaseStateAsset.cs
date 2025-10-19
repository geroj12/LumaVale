using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Chase")]
public class ChaseStateAsset : EnemyState
{
    public float chaseSpeed = 5f;

    public override void Enter()
    {
        enemy.SetRunning(true);
    }

    public override void Tick()
    {
        base.Tick(); // prüft Transitions

        if (enemy.target != null)
            enemy.MoveTo(enemy.target.position, chaseSpeed);
    }

    public override void Exit()
    {
        enemy.SetRunning(false);
    }
}