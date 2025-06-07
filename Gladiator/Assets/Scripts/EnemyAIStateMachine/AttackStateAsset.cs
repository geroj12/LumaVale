using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Attack")]
public class AttackStateAsset : EnemyState
{
    public float attackRange = 2f;
    private float lastAttackTime = -Mathf.Infinity;
    public float attackCooldown = 1.5f; // Sek.

    public override void Enter(StateMachineEnemy enemy)
    {
        enemy.animator.SetBool("Attack", true);
        lastAttackTime = Time.time; // Optional: Sofortiger Angriff beim Start verhindern

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

        enemy.FaceTarget(enemy.target); // Stoppen

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            // Hier wird die tatsächliche Angriffshandlung ausgelöst
            PerformAttack(enemy);
        }
    }
    private void PerformAttack(StateMachineEnemy enemy)
    {
        // Angriff ausführen – z. B. Schadenslogik, Audio, Partikeleffekte
        Debug.Log("Enemy führt Angriff aus!");
        // Optional: Animation spielt durch Loop oder eingebettete Events
    }
    public override void Exit(StateMachineEnemy enemy)
    {
        enemy.animator.SetBool("Attack", false);

    }
}