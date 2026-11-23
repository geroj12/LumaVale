
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Combat/Dodge")]
public class EnemyCombatDodgeState : EnemyState
{
    public float dodgeDistance = 2f;
    public float dodgeDuration = 0.4f;

    private Vector3 dodgeDirection;
    private float exitTime;

    public override void Enter()
    {
        enemy.ResetCombatTriggers();
        exitTime = Time.time + dodgeDuration;

        int dir = Random.Range(0, 3);

        if (dir == 0)
        {
            dodgeDirection = -enemy.transform.right;
            enemy.animator.SetTrigger("DodgeLeft");
        }
        else if (dir == 1)
        {
            dodgeDirection = enemy.transform.right;
            enemy.animator.SetTrigger("DodgeRight");
        }
        else
        {
            dodgeDirection = -enemy.transform.forward;
            enemy.animator.SetTrigger("DodgeBack");
        }

        enemy.controller.Move(
            dodgeDirection * dodgeDistance * Time.deltaTime
        );
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
    }
}
