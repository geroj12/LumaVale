using UnityEngine;

public abstract class Condition : ScriptableObject
{
    public virtual void Initialize(StateMachineEnemy enemy) { }

    public abstract bool Evaluate(StateMachineEnemy enemy);

}