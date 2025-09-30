using UnityEngine;

public class Player : MonoBehaviour
{
    private Movement movement;
    [SerializeField] private Animator animator;
    [SerializeField] private Combat combat;
    [SerializeField] private State state;

    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP;
    void Start()
    {
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
        currentHP = maxHP;
    }

    void Update()
    {
        state.RegenerateEnergy();
        movement.HandleMovement();
    }


    public void InterruptAttack()
    {
        combat = GetComponent<Combat>();
        if (combat != null)
        {

            combat.StartCoroutine(combat.ResetAttackBools());

        }
        animator.SetTrigger("HitBlocked"); // z. B. kleine Rückzuck-Animation

    }

    public void TakeDamage(float amount, bool hitShield = false)
    {
        currentHP -= amount;

    }
}
