using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Attack")]
public class AttackStateAsset : EnemyState
{
    public override void Enter()
    {
        enemy.Combat.ResetAttackFinished();

        int index = Random.Range(0, 3);

        string trigger = "Attack_Overhead";

        if (index == 0)
        {
            trigger = "Attack_Right";
        }
        else if (index == 1)
        {
            trigger = "Attack_Left";
        }

        enemy.animator.SetTrigger(trigger);

        enemy.StartCoroutine(WaitForAttackEnd());
    }

    private IEnumerator WaitForAttackEnd()
    {
        while (!enemy.Combat.IsAttackFinished)
        {
            yield return null;
        }

        if (enemy.combatIdleState != null)
        {
            enemy.TransitionTo(enemy.combatIdleState);
        }

    }

    public override void Exit() { }

}