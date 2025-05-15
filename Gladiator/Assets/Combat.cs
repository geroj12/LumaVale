using System.Collections;
using UnityEngine;

public class Combat : MonoBehaviour
{
    private Animator anim;
    private State state;
    [SerializeField] private CombatDirectionHandler directionHandler;
    private bool isAttacking = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        state = GetComponent<State>();
        directionHandler = GetComponent<CombatDirectionHandler>();
    }
    void Update()
    {
        HandleAttack();
    }
    
    public void HandleAttack()
    {
        if (state.blocking)
        {
            state.ResetMouseDirections();
        }

        if (isAttacking) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            state.attackThrust = true;
            anim.SetBool("attackThrust", true);
            StartCoroutine(ResetAttackBools());
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            state.attackUp = true;
            anim.SetBool("attackUp", true);
            StartCoroutine(ResetAttackBools());
        }
        
        if (Input.GetMouseButtonDown(0))
            directionHandler.StartSwipe(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
        {
            directionHandler.EndSwipe(Input.mousePosition);

            // Dann in Combat.cs:
            if (state.mouseOnLeftSide)
                anim.SetBool("attackLeft", true);

            else if (state.mouseOnRightSide)
                anim.SetBool("attackRight", true);
            StartCoroutine(ResetAttackBools());
        }



    }
    public void StartAttack()
    {
        isAttacking = true;
    }
    public void EndAttack()
    {
        isAttacking = false;
    }
    IEnumerator ResetAttackBools()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("attackLeft", false);
        anim.SetBool("attackThrust", false);
        anim.SetBool("attackRight", false);
        anim.SetBool("attackUp", false);

        state.ResetMouseDirections();
    }
}
