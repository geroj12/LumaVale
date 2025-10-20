using UnityEngine;

[System.Serializable]
public class Transition
{
    [SerializeField] private EnemyState targetState;
    [SerializeField] private Condition[] conditions;
    [SerializeField] private float transitionCooldown = 1f;

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

        string key = enemy.BuildTransitionKey(enemy.CurrentState, targetState);
        if (enemy.IsTransitionCooldownActive(key, transitionCooldown))
            return false;

        if (conditions == null || conditions.Length == 0)
            return false;

        foreach (var condition in conditions)
        {
            if (condition == null) continue;
            if (!condition.Evaluate(enemy)) return false;
        }

        // Wenn wir hier sind: Transition wird ausgelöst -> registriere Zeit
        enemy.RegisterTransitionTrigger(key);
        return true;
    }
}