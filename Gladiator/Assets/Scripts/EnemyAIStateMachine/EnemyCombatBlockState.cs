
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Combat/Block")]
public class EnemyCombatBlockState : EnemyState
{
    public float blockDuration = 1.2f;

    private float exitTime;

    public override void Enter()
    {
        enemy.ResetCombatTriggers();
        exitTime = Time.time + blockDuration;

        enemy.animator.SetBool("IsBlocking", true);

        Vector3 toPlayer = enemy.target.position - enemy.transform.position;
        toPlayer.Normalize();

        float dot = Vector3.Dot(enemy.transform.right, toPlayer);

        if (dot > 0)
            enemy.animator.SetTrigger("BlockRight");
        else
            enemy.animator.SetTrigger("BlockLeft");
    }

    public override void Tick()
    {
        if (Time.time >= exitTime)
        {
            enemy.TransitionTo(enemy.combatIdleState);
        }
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsBlocking", false);
    }
}
