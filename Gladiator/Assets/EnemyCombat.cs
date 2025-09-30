using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField]private EnemyWeapon weaponDamage;
    [SerializeField] private Animator anim;


    public void StartAttack()
    {
        weaponDamage.EnableDamage();
    }

    public void EndAttack()
    {
        weaponDamage.DisableDamage();
    }
}
