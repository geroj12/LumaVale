using UnityEngine;

[CreateAssetMenu(menuName = "EnemyConditions/Idle Timeout")]
public class IdleTimeoutCondition : Condition
{
    [SerializeField] private float idleDuration = 3f;
    private float enterTime;

    public override bool Evaluate(StateMachineEnemy enemy)
    {
        if (enterTime == 0f)
            enterTime = Time.time;
        return Time.time - enterTime > idleDuration;
    }
}