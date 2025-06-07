using UnityEngine;

public abstract class EnemyState : ScriptableObject
{
    public abstract void Enter(StateMachineEnemy enemy);
    public abstract void Tick(StateMachineEnemy enemy);
    public abstract void Exit(StateMachineEnemy enemy);
}