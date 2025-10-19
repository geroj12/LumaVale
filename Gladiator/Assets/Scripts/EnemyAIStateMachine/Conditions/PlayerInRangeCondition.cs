using UnityEngine;

[CreateAssetMenu(menuName = "EnemyConditions/PlayerInRange")]
public class PlayerInRangeCondition : Condition
{
    [SerializeField] private float attackRange = 2f;

    public override bool Evaluate(StateMachineEnemy enemy)
    {
        float distance = Vector3.Distance(enemy.transform.position, enemy.target.position);
        return distance <= attackRange;
    }
}