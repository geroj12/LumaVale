using UnityEngine;

public class State : MonoBehaviour
{
    public bool Strafe;
    public bool blockedAttack;
    public bool equipped = false;
    public bool blocking = false;
    public bool isAttacking = false;
    public bool canMove = false;

    [Header("Energy & Combat")]
    public float currentEnergy = 100f;
    public float maxEnergy = 100f;

    public float normalAttackCost = 5f;
    public float heavyAttackCost = 15f;

    public bool mouseOnLeftSide = false;
    public bool mouseOnRightSide = false;
    public bool attackUp = false;
    public bool attackThrust = false;

    [Header("Energy Regeneration")]
    public float energyRegenRate = 10f;
    public void UseEnergy(float amount)
    {
        currentEnergy = Mathf.Max(0f, currentEnergy - amount);
    }

    public void RegenerateEnergy()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        }
    }
    public void ResetMouseDirections()
    {
        mouseOnLeftSide = false;
        mouseOnRightSide = false;
        attackThrust = false;
        attackUp = false;

    }
}
