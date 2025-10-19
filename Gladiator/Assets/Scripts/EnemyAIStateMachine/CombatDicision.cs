using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "Enemy Decisions/Combat Decision")]
public class CombatDecision : ScriptableObject
{

    [Header("General Settings")]
    [SerializeField] private float attackRange = 2f;

    [System.Serializable]
    public class ActionOption
    {
        public string name;
        [Range(0, 10)] public int baseWeight = 1;
        public float cooldown = 2f;
        public EnemyState targetState; // auf welchen State soll gewechselt werden?

        [HideInInspector] public float nextAvailableTime;
        [HideInInspector] public int currentWeight;
    }

    [Header("Action Options")]
    [SerializeField] private List<ActionOption> actions = new List<ActionOption>();

    /// <summary>
    /// Evaluates combat logic and triggers a weighted random action.
    /// </summary>
    public void Evaluate(StateMachineEnemy enemy)
    {
        float currentTime = Time.time;
        float distance = Vector3.Distance(enemy.transform.position, enemy.target.position);

        // Wenn Spieler zu weit weg ist → Chase
        if (distance > attackRange + 0.5f)
        {
            enemy.TransitionTo(enemy.chaseState);
            return;
        }

        // Gewichtung vorbereiten
        foreach (var action in actions)
            action.currentWeight = action.baseWeight;

        // Dynamische Gewichtsanpassung
        AdjustWeightsBasedOnDistance(distance);

        // Nur verfügbare Aktionen berücksichtigen
        var availableActions = actions
            .Where(a => currentTime >= a.nextAvailableTime && a.currentWeight > 0 && a.targetState != null)
            .ToList();

        if (availableActions.Count == 0)
        {
            enemy.PlayIdleAnimation();
            return;
        }

        // Gewichtete Zufallsauswahl
        int totalWeight = availableActions.Sum(a => a.currentWeight);
        int roll = Random.Range(0, totalWeight);

        int cumulative = 0;
        foreach (var action in availableActions)
        {
            cumulative += action.currentWeight;
            if (roll < cumulative)
            {
                // Wechsle zum Ziel-State
                enemy.TransitionTo(action.targetState);
                action.nextAvailableTime = currentTime + action.cooldown;
                return;
            }
        }
    }

    private void AdjustWeightsBasedOnDistance(float distance)
    {
        // Beispielhafte Logik
        foreach (var a in actions)
            a.currentWeight = a.baseWeight;

        // Enger Nahkampf
        if (distance < 1.2f)
        {
            var dodge = actions.FirstOrDefault(a => a.name.ToLower().Contains("Dodge"));
            var block = actions.FirstOrDefault(a => a.name.ToLower().Contains("Block"));
            if (dodge != null) dodge.currentWeight += 3;
            if (block != null) block.currentWeight += 2;
        }
        // Mittel-Distanz
        else if (distance < 2.5f)
        {
            var attack = actions.FirstOrDefault(a => a.name.ToLower().Contains("Attack"));
            if (attack != null) attack.currentWeight += 4;
        }
        // Weit weg
        else
        {
            var retreat = actions.FirstOrDefault(a => a.name.ToLower().Contains("Retreat"));
            if (retreat != null) retreat.currentWeight += 3;
        }
    }
}