using UnityEngine;

[CreateAssetMenu(menuName = "EnemyConditions/LostPlayerSight")]
public class LostPlayerSightCondition : Condition
{
    [SerializeField] private float memoryDuration = 2f;

    public override bool Evaluate(StateMachineEnemy enemy)
    {
        return !enemy.HasRecentlySeenPlayer(memoryDuration);
    }
}