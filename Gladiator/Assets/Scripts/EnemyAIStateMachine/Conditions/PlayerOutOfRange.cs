using UnityEngine;

[CreateAssetMenu(menuName = "EnemyConditions/PlayerOutOfRangeCondition")]
public class PlayerOutOfRangeCondition : Condition
{
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float chaseRange = 10f;

    public override bool Evaluate(StateMachineEnemy enemy)
    {
        if (enemy.vision == null || enemy.vision.target == null)
            return false;

        float dist = Vector3.Distance(enemy.transform.position, enemy.vision.target.position);

        // Spieler ist zu weit für Attacke, aber noch sichtbar → Chasen
        return dist > attackRange && dist < chaseRange;
    }
}