using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Return")]
public class ReturnStateAsset : EnemyState
{
    public float returnSpeed = 3f;
    public float closeEnoughDistance = 1f;

    public override void Enter(StateMachineEnemy enemy)
    {
        Debug.Log("Enter RETURN");
        enemy.animator.SetBool("IsRunning", true);

    }

    public override void Tick(StateMachineEnemy enemy)
    {
        if (enemy.vision.CanSeeTarget())
        {
            enemy.TransitionTo(enemy.chaseState);
            return;
        }

        float dist = Vector3.Distance(enemy.transform.position, enemy.startPosition);
        if (dist < closeEnoughDistance)
        {
            enemy.TransitionTo(enemy.patrolState);
            return;
        }

        enemy.MoveTo(enemy.startPosition, returnSpeed);




    }

    public override void Exit(StateMachineEnemy enemy)
    {
        Debug.Log("Exit RETURN");
        enemy.animator.SetBool("IsRunning", false);

    }
}