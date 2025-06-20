using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/AggressiveAttack")]
public class AggressiveAttackStateAsset : EnemyState
{
    public float attackRange = 1.8f;
    public float attackCooldown = 0.75f;  // schneller als normal
    private float lastAttackTime = -Mathf.Infinity;
    public float playerNotInSightDuration = 3f;
    public string[] aggressiveAttackAnimations = {
        "Aggressive_Stab",
        "Aggressive_Slash",
        "Aggressive_Kick"
    };


    public override void Enter(StateMachineEnemy enemy)
    {
        lastAttackTime = Time.time - attackCooldown; 
    }

    public override void Tick(StateMachineEnemy enemy)
    {
         if (enemy.vision.CanSeeTarget())
        {
            enemy.NotifyPlayerSeen(); 
        }
        else if (!enemy.HasRecentlySeenPlayer(playerNotInSightDuration))
        {
            enemy.TransitionTo(enemy.investigateState);
            return;
        }

        float dist = Vector3.Distance(enemy.transform.position, enemy.target.position);

        // Optional aggressives Nachrücken
        if (dist > attackRange)
        {
            enemy.TransitionTo(enemy.combatIdleState);
            return;
        }
        enemy.StopMovement(); // stehen bleiben, wenn Abstand passt


        if (Time.time - lastAttackTime >= attackCooldown)
        {
            PerformAttack(enemy);

            lastAttackTime = Time.time;
        }
    }

    private void PerformAttack(StateMachineEnemy enemy)
    {
        int index = Random.Range(0, aggressiveAttackAnimations.Length);
        string anim = aggressiveAttackAnimations[index];

        enemy.animator.SetTrigger(anim);
    }

    public override void Exit(StateMachineEnemy enemy)
    {

        // ggf. Reset
    }
}
