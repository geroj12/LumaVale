using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/AggressiveAttack")]
public class AggressiveAttackStateAsset : EnemyState
{
    public float attackRange = 1.8f;
    public float attackCooldown = 0.75f;  // schneller als normal
    public float closeDistanceThreshold = 1f;

    public string[] aggressiveAttackAnimations = {
        "Aggressive_Stab",
        "Aggressive_Slash",
        "Aggressive_Kick"
    };

    private float lastAttackTime = -Mathf.Infinity;

    public override void Enter(StateMachineEnemy enemy)
    {
        lastAttackTime = Time.time - attackCooldown; // sofortiger Angriff möglich
    }

    public override void Tick(StateMachineEnemy enemy)
    {
        if (!enemy.vision.CanSeeTarget())
        {
            enemy.TransitionTo(enemy.investigateState);
            return;
        }

        float dist = Vector3.Distance(enemy.transform.position, enemy.target.position);
        enemy.FaceTarget(enemy.target);

        // Optional aggressives Nachrücken
        if (dist > attackRange)
        {
            enemy.MoveTo(enemy.target.position, 1f); // z. B. aggressives Zuschreiten
            return;
        }
        else
        {
            enemy.MoveTo(enemy.transform.position, 0f); // stehen bleiben
        }

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            // Animation zufällig auswählen
            int index = Random.Range(0, aggressiveAttackAnimations.Length);
            string anim = aggressiveAttackAnimations[index];
            enemy.animator.SetTrigger(anim);
        }
    }

    public override void Exit(StateMachineEnemy enemy)
    {
        // ggf. Reset
    }
}
