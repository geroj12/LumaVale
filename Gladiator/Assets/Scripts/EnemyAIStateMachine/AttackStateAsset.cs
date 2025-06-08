using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Attack")]
public class AttackStateAsset : EnemyState
{
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    private float lastAttackTime = -Mathf.Infinity;

    public override void Enter(StateMachineEnemy enemy)
    {
        lastAttackTime = Time.time - attackCooldown; // Sofortiger Angriff möglich
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

        enemy.FaceTarget(enemy.target);
        enemy.MoveTo(enemy.transform.position, 0f); // Stehen bleiben

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            Debug.Log("Trigger Angriff");
            enemy.animator.ResetTrigger("DoAttack"); // Optional, zur Sicherheit
            enemy.animator.SetTrigger("DoAttack");
        }
    }


    public override void Exit(StateMachineEnemy enemy)
    {
        //isAttacking = false; // Sicherheit, falls unterbrochen

    }
}