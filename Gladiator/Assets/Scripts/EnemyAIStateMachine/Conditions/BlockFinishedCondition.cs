using UnityEngine;
[CreateAssetMenu(menuName = "EnemyConditions/Block Finished")]
public class BlockFinishedCondition : Condition
{
    public override bool Evaluate(StateMachineEnemy enemy)
    {
        AnimatorStateInfo info = enemy.animator.GetCurrentAnimatorStateInfo(0);
        return !info.IsTag("Block");
    }
}
