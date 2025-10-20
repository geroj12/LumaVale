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
            return;
        }

        foreach (var t in transitions)
            t.Initialize(stateMachine);
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void Tick()
    {
        CheckTransitions();
    }

    protected void CheckTransitions()
    {
        if (transitions == null || transitions.Length == 0)
            return;

        foreach (var t in transitions)
        {
            bool should = t.ShouldTransition();

            if (should)
            {
                enemy.TransitionTo(t.TargetState);
                return;
            }
        }
    }
}