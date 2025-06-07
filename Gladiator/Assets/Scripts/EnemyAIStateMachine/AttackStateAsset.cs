using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Attack")]
public class AttackStateAsset : EnemyState
{
    public float attackRange = 2f;

    public override void Enter(StateMachineEnemy enemy)
    {
        Debug.Log("Enter ATTACK");
    }

    public override void Tick(StateMachineEnemy enemy)
    {
        if (!enemy.vision.CanSeeTarget())
        {
            enemy.TransitionTo(enemy.investigateState);
            return;
        }

        float dist = Vector3.Distance(enemy.transform.position, enemy.target.position);
        if (dist > attackRange)
        {
            enemy.TransitionTo(enemy.chaseState);
            return;
        }

        enemy.MoveTo(enemy.transform.position, 0f); // Stehen bleiben
        Debug.Log("Greife an!");
    }

    public override void Exit(StateMachineEnemy enemy)
    {
        Debug.Log("Exit ATTACK");
    }
}