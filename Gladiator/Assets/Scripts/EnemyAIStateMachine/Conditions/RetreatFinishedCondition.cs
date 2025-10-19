using UnityEngine;

[CreateAssetMenu(menuName = "EnemyConditions/Retreat Finished")]
public class RetreatFinishedCondition : Condition
{
    [SerializeField] private float minDistanceFromPlayer = 4f;

    public override bool Evaluate(StateMachineEnemy enemy)
    {
        if (enemy.target == null) return true;
        float dist = Vector3.Distance(enemy.transform.position, enemy.target.position);
        return dist >= minDistanceFromPlayer;
    }
}