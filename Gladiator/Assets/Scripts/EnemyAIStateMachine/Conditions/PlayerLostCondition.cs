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
        // Wenn der Spieler sichtbar ist → Zeit zurücksetzen
        if (enemy.vision != null && enemy.vision.CanSeeTarget())
        {
            lastSeenTime = Time.time;
            return false;
        }

        // Wenn Spieler länger als lostDelay unsichtbar ist → true
        return Time.time - lastSeenTime > lostDelay;
    }
}
