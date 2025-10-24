using UnityEngine;

public class AttackAnimator : MonoBehaviour
{
    private readonly Animator anim;
    private readonly WeaponHolster holster;

    public AttackAnimator(Animator animator, WeaponHolster holster)
    {
        anim = animator;
        this.holster = holster;
    }

    public void PlayAttackAnimation(string type)
    {
        int weapon = holster.currentWeaponType;

        switch (type)
        {
            case "Thrust":
                SetBoolByWeapon("Attack_THRUST", weapon);
                break;
            case "Overhead":
                SetBoolByWeapon("Attack_UP", weapon);
                break;
            case "Left":
                SetBoolByWeapon("Attack_LEFT", weapon);
                break;
            case "Right":
                SetBoolByWeapon("Attack_RIGHT", weapon);
                break;
        }
    }

    private void SetBoolByWeapon(string baseName, int weapon)
    {
        string animName = weapon switch
        {
            2 => $"{baseName}_TwoHanded",
            1 => $"{baseName}_OneHanded",
            _ => $"{baseName}_Unarmed"
        };
        anim.SetBool(animName, true);
    }

    public void ResetAllAttackBools()
    {
        string[] bools =
        {
            "Attack_LEFT_TwoHanded", "Attack_THRUST_TwoHanded", "Attack_RIGHT_TwoHanded", "Attack_UP_TwoHanded",
            "Attack_LEFT_OneHanded", "Attack_THRUST_OneHanded", "Attack_RIGHT_OneHanded", "Attack_UP_OneHanded",
            "Attack_LEFT_Unarmed", "Attack_THRUST_Unarmed", "Attack_RIGHT_Unarmed", "Attack_UP_Unarmed"
        };

        foreach (var b in bools)
            anim.SetBool(b, false);
    }
}