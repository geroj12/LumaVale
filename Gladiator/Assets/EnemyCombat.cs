using System.Collections;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private EnemyWeapon weaponDamage;
    [SerializeField] private Animator anim;

    public void StartAttack()
    {
        weaponDamage.EnableDamage();
    }

    public void EndAttack()
    {
        weaponDamage.DisableDamage();
    }
    
    public IEnumerator ResetAttackBools()
    {
        yield return new WaitForSeconds(1f);

        // Zweihaendig
        anim.SetBool("Attack_Right", false);
        anim.SetBool("Attack_Left", false);
        anim.SetBool("Attack_Overhead", false);

      
    }
}
