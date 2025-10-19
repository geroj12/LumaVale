using UnityEngine;

[CreateAssetMenu(menuName = "EnemyConditions/Dodge Finished")]
public class DodgeFinishedCondition : Condition
{
    public override bool Evaluate(StateMachineEnemy enemy)
    {
        AnimatorStateInfo info = enemy.animator.GetCurrentAnimatorStateInfo(0);
        return !info.IsTag("Dodge");
    }
}
