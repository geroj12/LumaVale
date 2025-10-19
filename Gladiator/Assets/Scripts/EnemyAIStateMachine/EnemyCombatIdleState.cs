using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "EnemyStates/CombatIdle")]
public class EnemyCombatIdleState : EnemyState
{

    [SerializeField] private CombatDecision decisionLogic;
    [SerializeField] private float minDecisionDelay = 0.5f;
    [SerializeField] private float maxDecisionDelay = 1.2f;
    private float nextDecisionTime;

    public override void Enter()
    {
        enemy.StopMovement();
        ScheduleNextDecision();
    }

    public override void Tick()
    {

        if (!enemy.target || enemy.isTurning)
            return;

        if (enemy.TryPlayTurnAnimation(enemy.target))
            return;

        if (Time.time < nextDecisionTime)
            return;

        // interne Entscheidungslogik
        decisionLogic?.Evaluate(enemy);
        ScheduleNextDecision();

        // Transitions prüfen
        base.Tick();
    }

    private void ScheduleNextDecision()
    {
        nextDecisionTime = Time.time + Random.Range(minDecisionDelay, maxDecisionDelay);
    }

    public override void Exit() { }
}
