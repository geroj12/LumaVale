using UnityEngine;

[System.Serializable]
public class Transition
{
    [SerializeField] private EnemyState targetState;
    [SerializeField] private Condition[] conditions; // <-- changed
    [SerializeField, Tooltip("Mindestabstand zwischen zwei Transition-Auslösungen.")]
    private float transitionCooldown = 0.3f;

    private StateMachineEnemy enemy;
    private float lastTriggerTime = -Mathf.Infinity;

    public EnemyState TargetState => targetState;

    public void Initialize(StateMachineEnemy stateMachine)
    {
        enemy = stateMachine;
        if (conditions == null) return;

        foreach (var condition in conditions)
        {
            condition?.Initialize(stateMachine);
        }
    }

    public bool ShouldTransition()
    {
        if (enemy == null || targetState == null) return false;

        if (Time.time - lastTriggerTime < transitionCooldown)
            return false;

        if (conditions == null || conditions.Length == 0)
            return false;

        foreach (var condition in conditions)
        {
            if (condition == null) continue;
            if (!condition.Evaluate(enemy)) return false;
        }

        Debug.Log($"[Transition] triggered: {enemy.name} -> {targetState.name}");
        lastTriggerTime = Time.time;
        return true;
    }
}