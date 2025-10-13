using System;
using System.Collections;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private EnemyWeapon weapon;
    [SerializeField] private Animator anim;
    [SerializeField] private EnemyCounterWindow counterWindow;
    [SerializeField] private StateMachineEnemy stateMachineEnemy;

    void Start()
    {
        if (counterWindow != null)
            counterWindow.OnCounterTriggered += HandleCounterTriggered;
    }

    public void TelegraphAttackUI()
    {
        counterWindow.TryActivate();
    }
    public void StartAttack()
    {
        weapon.EnableDamage();
    }

    public void EndAttack()
    {
        weapon.DisableDamage();
    }
    private void HandleCounterTriggered()
    {
        // Wenn Konter erfolgreich: Stun oder Animation spielen
        anim.SetTrigger("Countered");
        stateMachineEnemy.TemporarilyDisableFSM(2f);
    }
    public IEnumerator ResetAttackBools()
    {
        yield return new WaitForSeconds(1f);

        anim.SetBool("Attack_Right", false);
        anim.SetBool("Attack_Left", false);
        anim.SetBool("Attack_Overhead", false);


    }

    internal void ApplyStun()
    {
        Debug.Log("Enemy stunned");
        anim.SetTrigger("Stunned");
        //stateMachineEnemy.TemporarilyDisableFSM(5f);

    }
}
