using UnityEngine;


[CreateAssetMenu(menuName = "EnemyStates/CombatIdle")]
public class EnemyCombatIdleState : EnemyState
{
    public float decisionInterval = 1.5f;
    private float nextDecisionTime;

    public override void Enter(StateMachineEnemy enemy)
    {
        nextDecisionTime = Time.time + decisionInterval;
        enemy.SetRunning(false);
        enemy.animator.SetBool("IsBlocking", false);
    }

    public override void Tick(StateMachineEnemy enemy)
    {
        if (Time.time >= nextDecisionTime)
        {
            DecideNextCombatAction(enemy);
            nextDecisionTime = Time.time + decisionInterval;
        }

        enemy.FaceTarget(enemy.target);
    }

    private void DecideNextCombatAction(StateMachineEnemy enemy)
    {
        float dist = Vector3.Distance(enemy.transform.position, enemy.target.position);
        float rand = Random.value;

        if (dist > 2f)
        {
            enemy.TransitionTo(enemy.chaseState); // Repositionieren
        }
        else if (rand < 0.3f)
            enemy.TransitionTo(enemy.attackState);
        else if (rand < 0.55f)
            enemy.TransitionTo(enemy.blockState);
        else if (rand < 0.8f)
            enemy.TransitionTo(enemy.dodgeState);
        else
            enemy.TransitionTo(enemy.combatRetreatState);
    }

    public override void Exit(StateMachineEnemy enemy)
    {

    }
}
