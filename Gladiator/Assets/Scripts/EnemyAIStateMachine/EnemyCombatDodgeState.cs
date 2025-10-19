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
        exitTime = Time.time + dodgeDuration;

        // Zufällige Richtung: links, rechts, rückwärts
        int dir = Random.Range(0, 3);
        switch (dir)
        {
            case 0:
                dodgeDirection = -enemy.transform.right; enemy.animator.SetTrigger("DodgeLeft");
                break; 
            case 1:
                dodgeDirection = enemy.transform.right; enemy.animator.SetTrigger("DodgeRight");
                break;  
            default: dodgeDirection = -enemy.transform.forward; enemy.animator.SetTrigger("DodgeBack"); break; 
        }


        // sofort ein kleines Stück bewegen
        enemy.controller.Move(dodgeDirection * dodgeDistance * Time.deltaTime);
    }

    public override void Tick()
    {
       
    }

    public override void Exit()
    {
        // Optionales Aufräumen
    }
}
