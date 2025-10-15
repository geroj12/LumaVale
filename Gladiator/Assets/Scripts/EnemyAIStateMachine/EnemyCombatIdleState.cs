using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "EnemyStates/CombatIdle")]
public class EnemyCombatIdleState : EnemyState
{
    [Header("Decision Settings")]
    [SerializeField] private float minDecisionDelay = 0.5f;
    [SerializeField] private float maxDecisionDelay = 1.2f;
    [SerializeField] private float attackRange = 2f;

    private float nextDecisionTime;

    [System.Serializable]
    public class ActionOption
    {
        public string name;
        [Range(0, 10)] public int baseWeight = 1;
        public float cooldown = 2f;

        [HideInInspector] public float nextAvailableTime = 0f;
        public System.Action<StateMachineEnemy> Execute;
        [HideInInspector] public int currentWeight;
    }

    [Header("Action Options")]
    [SerializeField] private ActionOption attack = new ActionOption { name = "Attack", baseWeight = 3, cooldown = 2f };
    [SerializeField] private ActionOption block = new ActionOption { name = "Block", baseWeight = 2, cooldown = 2f };
    [SerializeField] private ActionOption dodge = new ActionOption { name = "Dodge", baseWeight = 2, cooldown = 3f };
    [SerializeField] private ActionOption retreat = new ActionOption { name = "Retreat", baseWeight = 1, cooldown = 5f };
    private List<ActionOption> actions;

    public override void Enter(StateMachineEnemy enemy)
    {
        enemy.StopMovement();

        if (actions == null)
        {
            actions = new List<ActionOption> { attack, block, dodge, retreat };

            attack.Execute = e => e.TransitionTo(e.attackState);
            block.Execute = e => e.TransitionTo(e.blockState);
            dodge.Execute = e => e.TransitionTo(e.dodgeState);
            retreat.Execute = e => e.TransitionTo(e.combatRetreatState);
        }

        ScheduleNextDecision();
    }

    public override void Tick(StateMachineEnemy enemy)
    {
        if (!enemy.target || enemy.isTurning)
            return;

        if (enemy.TryPlayTurnAnimation(enemy.target))
        {
            enemy.isTurning = true;
            return;
        }

        if (Time.time >= nextDecisionTime)
        {
            DecideNextAction(enemy);
            ScheduleNextDecision();
        }
    }
    private void ScheduleNextDecision()
    {
        nextDecisionTime = Time.time + Random.Range(minDecisionDelay, maxDecisionDelay);
    }
    private void DecideNextAction(StateMachineEnemy enemy)
    {
        float currentTime = Time.time;
        float dist = Vector3.Distance(enemy.transform.position, enemy.target.position);

        if (dist > attackRange + 0.5f)
        {
            enemy.TransitionTo(enemy.chaseState);
            return;
        }

        // Dynamische Gewichtsanpassung
        foreach (var action in actions)
            action.currentWeight = action.baseWeight;

        if (dist < 1.2f)
        {
            dodge.currentWeight += 3;
            block.currentWeight += 2;
        }
        else if (dist < 2.5f)
        {
            attack.currentWeight += 4;
        }
        else
        {
            retreat.currentWeight += 3;
        }

        var availableActions = actions
            .Where(a => currentTime >= a.nextAvailableTime && a.currentWeight > 0)
            .ToList();

        if (availableActions.Count == 0)
        {
            enemy.PlayIdleAnimation(); // kleine Idle Bewegung
            return;
        }

        int totalWeight = availableActions.Sum(a => a.currentWeight);
        int roll = Random.Range(0, totalWeight);

        int cumulative = 0;
        foreach (var action in availableActions)
        {
            cumulative += action.currentWeight;
            if (roll < cumulative)
            {
                action.Execute?.Invoke(enemy);
                action.nextAvailableTime = currentTime + action.cooldown;
                return;
            }
        }
    }

    public override void Exit(StateMachineEnemy enemy) { }
}
