using UnityEngine;

[CreateAssetMenu(menuName = "EnemyConditions/PlayerRecentlySeenCondition")]
public class PlayerRecentlySeenCondition : Condition
{
    [SerializeField] private float memoryDuration = 3f;

    public override bool Evaluate(StateMachineEnemy enemy)
    {
        return enemy.HasRecentlySeenPlayer(memoryDuration);
    }
}
