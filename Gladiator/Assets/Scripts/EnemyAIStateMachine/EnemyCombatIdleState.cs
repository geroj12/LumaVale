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

        [HideInInspector] public float nextAvailableTime = 0f; // intern für Cooldown-Tracking
    }

    [Header("Action Weights & Cooldowns")]
    public ActionOption attack = new ActionOption { weight = 3, cooldown = 2f };
    public ActionOption block = new ActionOption { weight = 2, cooldown = 2f };
    public ActionOption dodge = new ActionOption { weight = 2, cooldown = 3f };
    public ActionOption retreat = new ActionOption { weight = 1, cooldown = 5f };

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
        float dist = Vector3.Distance(enemy.transform.position, enemy.target.position);

        if (dist > attackRange)
        {
            enemy.TransitionTo(enemy.chaseState);
            return;
        }

        int totalWeight = 0;

        if (Time.time >= attack.nextAvailableTime) totalWeight += attack.weight;
        if (Time.time >= block.nextAvailableTime) totalWeight += block.weight;
        if (Time.time >= dodge.nextAvailableTime) totalWeight += dodge.weight;
        if (Time.time >= retreat.nextAvailableTime) totalWeight += retreat.weight;

        if (totalWeight <= 0)
        {
            Debug.Log("⚠ Keine Aktion verfügbar, bleibe idle.");
            return;
        }

        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;

        // Attack
        if (Time.time >= attack.nextAvailableTime)
        {
            cumulative += attack.weight;
            if (roll < cumulative)
            {
                enemy.TransitionTo(enemy.attackState);
                attack.nextAvailableTime = Time.time + attack.cooldown;
                return;
            }
        }

        // Block
        if (Time.time >= block.nextAvailableTime)
        {
            cumulative += block.weight;
            if (roll < cumulative)
            {
                enemy.TransitionTo(enemy.blockState);
                block.nextAvailableTime = Time.time + block.cooldown;
                return;
            }
        }

        // Dodge
        if (Time.time >= dodge.nextAvailableTime)
        {
            cumulative += dodge.weight;
            if (roll < cumulative)
            {
                enemy.TransitionTo(enemy.dodgeState);
                dodge.nextAvailableTime = Time.time + dodge.cooldown;
                return;
            }
        }

        // Retreat
        if (Time.time >= retreat.nextAvailableTime)
        {
            cumulative += retreat.weight;
            if (roll < cumulative)
            {
                enemy.TransitionTo(enemy.combatRetreatState);
                retreat.nextAvailableTime = Time.time + retreat.cooldown;
                return;
            }
        }
    }


    public override void Exit(StateMachineEnemy enemy)
    {

    }
}
