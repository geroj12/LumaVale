using UnityEngine;

[CreateAssetMenu(menuName = "EnemyConditions/CanSeeTarget")]
public class CanSeeTargetCondition : Condition
{
    public override bool Evaluate(StateMachineEnemy enemy)
    {
        return enemy.vision.CanSeeTarget();
    }
}
