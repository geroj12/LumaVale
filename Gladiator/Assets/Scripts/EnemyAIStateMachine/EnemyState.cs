using UnityEngine;

public abstract class EnemyState : ScriptableObject
{
    protected StateMachineEnemy enemy;

    [Header("Transitions")]
    [SerializeField] protected Transition[] transitions;

    public virtual void Initialize(StateMachineEnemy stateMachine)
    {
        enemy = stateMachine;

        if (transitions == null || transitions.Length == 0)
        {
            Debug.Log($"[{name}] hat keine Transitions.");
            return;
        }

        foreach (var t in transitions)
            t.Initialize(stateMachine);
    }

    public virtual void Enter()
    {
        Debug.Log($"[State Enter] {enemy.name} entered {name}");
    }

    public virtual void Exit()
    {
        Debug.Log($"[State Exit] {enemy.name} exited {name}");
    }

    public virtual void Tick()
    {
        // Standardverhalten: prüfe jede Transition
        CheckTransitions();
    }

    /// <summary>
    /// Prüft, ob eine Transition ausgelöst werden soll.
    /// </summary>
    protected void CheckTransitions()
    {
        if (transitions == null || transitions.Length == 0)
            return;

        foreach (var t in transitions)
        {
            bool should = t.ShouldTransition();

            if (should)
            {
                Debug.Log($"[Transition Triggered] {enemy.name} -> {t.TargetState.name} (from {name})");
                enemy.TransitionTo(t.TargetState);
                return;
            }
        }
    }
}