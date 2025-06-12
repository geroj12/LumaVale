using UnityEngine;


[CreateAssetMenu(menuName = "EnemyStates/CombatIdle")]
public class EnemyCombatIdleState : EnemyState
{
    public float decisionInterval = 1.5f;
    private float nextDecisionTime;

    public override void Enter(StateMachineEnemy enemy)
    {
        enemy.StopMovement();
        nextDecisionTime = Time.time + decisionInterval;

    }

    public override void Tick(StateMachineEnemy enemy)
    {
        if (enemy.target == null)
            return;

        if (enemy.isTurning)
            return;

        // Versuche Turn auszuführen
        if (enemy.TryPlayTurnAnimation(enemy.target))
        {
            enemy.isTurning = true;
            return;
        }

        // Warte auf nächste Entscheidungsphase
        if (Time.time < nextDecisionTime)
            return;

        // Entscheide nächste Aktion
        DecideNextCombatAction(enemy);
        nextDecisionTime = Time.time + decisionInterval;

    }

    private void DecideNextCombatAction(StateMachineEnemy enemy)
    {
        float dist = Vector3.Distance(enemy.transform.position, enemy.target.position);
        float rand = Random.value;

        if (dist > 2f)
        {
            enemy.TransitionTo(enemy.chaseState);
        }
        else if (rand < 0.3f)
        {
            enemy.TransitionTo(enemy.attackState);
        }
        else if (rand < 0.55f)
        {
            enemy.TransitionTo(enemy.blockState);
        }
        else if (rand < 0.8f)
        {
            enemy.TransitionTo(enemy.dodgeState);
        }
        else
        {
            enemy.TransitionTo(enemy.combatRetreatState);
        }
    }

    public override void Exit(StateMachineEnemy enemy)
    {

    }
}
