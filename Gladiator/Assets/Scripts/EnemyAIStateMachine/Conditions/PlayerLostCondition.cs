using UnityEngine;

[CreateAssetMenu(menuName = "EnemyConditions/PlayerLostCondition")]
public class PlayerLostCondition : Condition
{
    [SerializeField] private float lostDelay = 2f;
    private float lastSeenTime;

    public override void Initialize(StateMachineEnemy enemy)
    {
        base.Initialize(enemy);
        lastSeenTime = Time.time;
    }

    public override bool Evaluate(StateMachineEnemy enemy)
    {
        if (enemy.vision != null && enemy.vision.CanSeeTarget())
        {
            lastSeenTime = Time.time;
            return false;
        }

        return Time.time - lastSeenTime > lostDelay;
    }
}
