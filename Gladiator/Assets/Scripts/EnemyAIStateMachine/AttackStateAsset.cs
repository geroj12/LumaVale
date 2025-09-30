using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Attack")]
public class AttackStateAsset : EnemyState
{
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float playerNotInSightDuration = 2f;
    private float lastAttackTime = -Mathf.Infinity;
    public string[] attackAnimations = { "Attack_Left", "Attack_Right", "Attack_Overhead" };
    public override void Enter(StateMachineEnemy enemy)
    {
        lastAttackTime = Time.time - attackCooldown;
    }

    public override void Tick(StateMachineEnemy enemy)
    {

        if (enemy.vision.CanSeeTarget())
        {
            enemy.NotifyPlayerSeen(); // Spieler wurde gesehen → Zeit merken
        }
        else if (!enemy.HasRecentlySeenPlayer(playerNotInSightDuration)) // 2s Kulanz
        {
            enemy.TransitionTo(enemy.investigateState);
            return;
        }

        float dist = Vector3.Distance(enemy.transform.position, enemy.target.position);
        if (dist > attackRange)
        {
            enemy.TransitionTo(enemy.combatIdleState);
            return;
        }

        enemy.StopMovement();

        if (Time.time - lastAttackTime >= attackCooldown)
        {

            PerformAttack(enemy);
            lastAttackTime = Time.time;

        }
    }

    private void PerformAttack(StateMachineEnemy enemy)
    {

        int index = Random.Range(0, attackAnimations.Length);
        string anim = attackAnimations[index];
        enemy.animator.SetTrigger(anim);


    }


    public override void Exit(StateMachineEnemy enemy)
    {


    }
}