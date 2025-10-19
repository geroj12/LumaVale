using UnityEngine;

[CreateAssetMenu(menuName = "EnemyConditions/Health Low")]
public class HealthLowCondition : Condition
{
    [SerializeField] private float lowHealthThreshold = 30f;

    public override bool Evaluate(StateMachineEnemy enemy)
    {
        var health = enemy.GetComponent<EnemyHealth>();
        if (health == null) return false;
        return health.HealthPercent <= lowHealthThreshold;
    }
}
