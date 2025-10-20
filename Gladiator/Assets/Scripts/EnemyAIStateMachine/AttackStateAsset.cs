using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Attack")]
public class AttackStateAsset : EnemyState
{
    public override void Enter()
    {
        int index = Random.Range(0, 3);
        string trigger = index switch
        {
            0 => "Attack_Right",
            1 => "Attack_Left",
            _ => "Attack_Overhead"
        };


        enemy.animator.SetTrigger(trigger);

        enemy.StartCoroutine(WaitForAttackEnd());
    }

    private IEnumerator WaitForAttackEnd()
    {
        yield return new WaitUntil(() => enemy.Combat.IsAttackFinished);

        if (enemy.combatIdleState != null)
        {
            enemy.TransitionTo(enemy.combatIdleState);

        }

    }

    public override void Exit(){}
    
}