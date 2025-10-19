using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Attack")]
public class AttackStateAsset : EnemyState
{

    private bool attackFinished;

    public override void Enter()
    {
        attackFinished = false;

        // 🔹 Zufällige Attack auswählen
        int index = Random.Range(0, 3);
        string trigger = index switch
        {
            0 => "Attack_Right",
            1 => "Attack_Left",
            _ => "Attack_Overhead"
        };

        Debug.Log($"[AttackState] {enemy.name} startet Angriff: {trigger}");

        // Angriff aktivieren
        enemy.animator.SetTrigger(trigger);

        // Coroutine starten, die auf Attack-Ende wartet
        enemy.StartCoroutine(WaitForAttackEnd());
    }

    private System.Collections.IEnumerator WaitForAttackEnd()
    {
        // Warte bis Combat meldet, dass der Angriff vorbei ist
        yield return new WaitUntil(() => enemy.Combat.IsAttackFinished);

        attackFinished = true;
        Debug.Log($"[AttackState] {enemy.name} Angriff beendet ✅");

        // Danach zurück in CombatIdleState
        if (enemy.combatIdleState != null)
        {
            enemy.TransitionTo(enemy.combatIdleState);
        }
        else
        {
            Debug.LogWarning("[AttackState] Kein CombatIdleState gesetzt!");
        }
    }

    public override void Exit()
    {
        Debug.Log($"[AttackState] {enemy.name} verlässt AttackState");
    }
}