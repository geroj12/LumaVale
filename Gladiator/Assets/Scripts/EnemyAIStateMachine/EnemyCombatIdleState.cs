using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "EnemyStates/CombatIdle")]
public class EnemyCombatIdleState : EnemyState
{
    [Header("Decision Settings")]
    [Tooltip("Wie oft der Gegner überlegt, eine neue Aktion zu machen (Sekunden).")]
    [SerializeField] private float decisionInterval = 1.5f;

    [Tooltip("Maximale Distanz, ab der der Gegner in Chase geht.")]
    [SerializeField] private float attackRange = 2f;

    private float nextDecisionTime;

    [System.Serializable]
    public class ActionOption
    {
        [Tooltip("Wie stark diese Aktion bevorzugt wird (relatives Gewicht).")]
        [Range(0, 10)] public int weight = 1;

        [Tooltip("Mindestzeit (Sekunden) zwischen zwei Einsätzen dieser Aktion.")]
        public float cooldown = 2f;

        [HideInInspector] public float nextAvailableTime = 0f;
        public System.Action<StateMachineEnemy> Execute;
    }

    [Header("Action Weights & Cooldowns")]
    [SerializeField] private ActionOption attack = new ActionOption { weight = 3, cooldown = 2f };
    [SerializeField] private ActionOption block = new ActionOption { weight = 2, cooldown = 2f };
    [SerializeField] private ActionOption dodge = new ActionOption { weight = 2, cooldown = 3f };
    [SerializeField] private ActionOption retreat = new ActionOption { weight = 1, cooldown = 5f };
    private List<ActionOption> actions;

    public override void Enter(StateMachineEnemy enemy)
    {
        enemy.StopMovement();
        nextDecisionTime = Time.time + decisionInterval;

        // Initialisiere Aktionen (nur einmal)
        if (actions == null || actions.Count == 0)
        {
            actions = new List<ActionOption> { attack, block, dodge, retreat };

            attack.Execute = e => e.TransitionTo(e.attackState);
            block.Execute = e => e.TransitionTo(e.blockState);
            dodge.Execute = e => e.TransitionTo(e.dodgeState);
            retreat.Execute = e => e.TransitionTo(e.combatRetreatState);
        }
    }

    public override void Tick(StateMachineEnemy enemy)
    {
        if (enemy.target == null || enemy.isTurning)
            return;

        if (enemy.isTurning)
            return;

        if (enemy.TryPlayTurnAnimation(enemy.target))
        {
            enemy.isTurning = true;
            return;
        }

        if (Time.time < nextDecisionTime)
            return;

        DecideNextCombatAction(enemy);
        nextDecisionTime = Time.time + decisionInterval;
    }

    private void DecideNextCombatAction(StateMachineEnemy enemy)
    {
        float currentTime = Time.time;
        float dist = Vector3.Distance(enemy.transform.position, enemy.target.position);

        if (dist > attackRange)
        {
            enemy.TransitionTo(enemy.chaseState);
            return;
        }

        // Gefilterte Liste verfügbarer Aktionen
        var availableActions = actions
            .Where(a => currentTime >= a.nextAvailableTime && a.weight > 0)
            .ToList();

        if (availableActions.Count == 0)
        {
            Debug.Log("⚠ Keine Aktion verfügbar, bleibe idle.");
            return;
        }

        int totalWeight = availableActions.Sum(a => a.weight);
        int roll = Random.Range(0, totalWeight);

        // Auswahl per kumulativer Wahrscheinlichkeit
        int cumulative = 0;
        foreach (var action in availableActions)
        {
            cumulative += action.weight;
            if (roll < cumulative)
            {
                action.Execute?.Invoke(enemy);
                action.nextAvailableTime = currentTime + action.cooldown;
                return;
            }
        }
    }


    public override void Exit(StateMachineEnemy enemy)
    {

    }
}
