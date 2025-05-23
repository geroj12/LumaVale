using UnityEngine;

public class Player : MonoBehaviour
{
    private Movement movement;
    [SerializeField] private Animator animator;
    [SerializeField] private Combat combat;
    
    void Start()
    {
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {

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
}
