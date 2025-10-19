using UnityEngine;

[CreateAssetMenu(menuName = "EnemyConditions/PlayerTooFar")]

public class PlayerTooFarCondition : Condition
{
    [SerializeField] private float maxDistance = 6f;

    public override bool Evaluate(StateMachineEnemy enemy)
    {
        if (enemy.target == null) return true;
        float distance = Vector3.Distance(enemy.transform.position, enemy.target.position);
        return distance > maxDistance;
    }
}
