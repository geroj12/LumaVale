using UnityEngine;

[CreateAssetMenu(menuName = "EnemyConditions/Reached Start Position")]
public class ReachedStartPositionCondition : Condition
{
    [SerializeField] private float threshold = 1f;

    public override bool Evaluate(StateMachineEnemy enemy)
    {
        float dist = Vector3.Distance(enemy.transform.position, enemy.startPosition);
        return dist < threshold;
    }
}