using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Combat/Block")]
public class EnemyCombatBlockState : EnemyState
{
    public float blockDuration = 1.2f;
    private float exitTime;

    public override void Enter()
    {
        exitTime = Time.time + blockDuration;
        enemy.animator.SetBool("IsBlocking", true);

        // Richtung ermitteln
        Vector3 toPlayer = (enemy.target.position - enemy.transform.position).normalized;
        float dot = Vector3.Dot(enemy.transform.right, toPlayer); // rechts = +, links = -
        if (dot > 0)
            enemy.animator.SetTrigger("BlockRight");
        else
            enemy.animator.SetTrigger("BlockLeft");
    }

    public override void Tick()
    {
       
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsBlocking", false);
    }
}
