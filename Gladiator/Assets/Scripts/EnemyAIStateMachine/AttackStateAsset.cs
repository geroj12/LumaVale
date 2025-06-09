using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Attack")]
public class AttackStateAsset : EnemyState
{
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    private float lastAttackTime = -Mathf.Infinity;
    public string[] attackAnimations = { "Attack_Left", "Attack_Right", "Attack_Overhead" };
    public float tooCloseDistance = 1.0f;     // Wenn er zurückweichen soll
    public override void Enter(StateMachineEnemy enemy)
    {
        lastAttackTime = Time.time - attackCooldown; // Sofortiger Angriff möglich
        enemy.animator.applyRootMotion = false;

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
            enemy.TransitionTo(enemy.combatIdleState);
            return;
        }

        enemy.FaceTarget(enemy.target);
        enemy.StopMovement(); // stehen bleiben, wenn Abstand passt
        enemy.animator.applyRootMotion = false;


        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            int index = Random.Range(0, attackAnimations.Length);
            string anim = attackAnimations[index];
            enemy.animator.SetTrigger(anim);
        }
    }


    public override void Exit(StateMachineEnemy enemy)
    {
        //isAttacking = false; // Sicherheit, falls unterbrochen
        enemy.animator.applyRootMotion = true;

    }
}