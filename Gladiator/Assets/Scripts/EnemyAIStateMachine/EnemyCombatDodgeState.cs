using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Combat/Dodge")]
public class EnemyCombatDodgeState : EnemyState
{
    public float dodgeDistance = 2f;
    public float dodgeSpeed = 6f;
    public float dodgeDuration = 0.4f;

    private Vector3 dodgeDirection;
    private float exitTime;

    public override void Enter(StateMachineEnemy enemy)
    {
        exitTime = Time.time + dodgeDuration;

        // Zufällige Richtung: links, rechts, rückwärts
        int dir = Random.Range(0, 3);
        switch (dir)
        {
            case 0:
                dodgeDirection = -enemy.transform.right; enemy.animator.SetTrigger("DodgeLeft");
                break; // links
            case 1:
                dodgeDirection = enemy.transform.right; enemy.animator.SetTrigger("DodgeRight");
                break;  // rechts
            default: dodgeDirection = -enemy.transform.forward; enemy.animator.SetTrigger("DodgeBack"); break; // zurück
        }

        // Optionale Animation

        // sofort ein kleines Stück bewegen
        enemy.controller.Move(dodgeDirection * dodgeDistance * Time.deltaTime);
    }

    public override void Tick(StateMachineEnemy enemy)
    {
        // Weiterbewegen während Dodge
        enemy.controller.Move(dodgeDirection * dodgeSpeed * Time.deltaTime);

        if (Time.time >= exitTime)
        {
            enemy.TransitionTo(enemy.combatIdleState);
        }
    }

    public override void Exit(StateMachineEnemy enemy)
    {
        // Optionales Aufräumen
    }
}
