using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Investigate")]
public class InvestigateStateAsset : EnemyState
{
    public float investigateDuration = 3f;

    private float timer;
    private Vector3 lastKnownPosition;

    public override void Enter(StateMachineEnemy enemy)
    {
        Debug.Log("Enter INVESTIGATE");
        timer = 0f;
        lastKnownPosition = enemy.target.position;
        enemy.animator.SetBool("InvestigateWalk", true);
    }

    public override void Tick(StateMachineEnemy enemy)
    {
        timer += Time.deltaTime;

        if (enemy.vision.CanSeeTarget())
        {
            enemy.TransitionTo(enemy.chaseState);
            return;
        }

        enemy.MoveTo(lastKnownPosition, enemy.patrolState is PatrolStateAsset patrol ? patrol.patrolSpeed : 3f);

        if (timer >= investigateDuration)
        {
            enemy.TransitionTo(enemy.returnState);

        }
    }

    public override void Exit(StateMachineEnemy enemy)
    {
        Debug.Log("Exit INVESTIGATE");
        enemy.animator.SetBool("InvestigateWalk", false);

    }
}