using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Return")]
public class ReturnStateAsset : EnemyState
{
    public float returnSpeed = 3f;
    public float closeEnoughDistance = 1f;

    public override void Enter()
    {
        enemy.animator.SetBool("IsRunning", true);
    }

    public override void Tick()
    {
        base.Tick(); // prüft Transitions

        enemy.MoveTo(enemy.startPosition, returnSpeed);

        if (Vector3.Distance(enemy.transform.position, enemy.startPosition) < closeEnoughDistance)
        {
            enemy.TransitionTo(enemy.initialState);
        }
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsRunning", false);
    }
}