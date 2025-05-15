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
    void FixedUpdate()
    {
        directionHandler.HandleAttackDirectionUI();
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


        if (Input.GetMouseButtonUp(0))
        {
            if (state.mouseOnRightSide) anim.SetBool("attackRight", true);
            if (state.mouseOnLeftSide) anim.SetBool("attackLeft", true);

            StartCoroutine(ResetAttackBools());
            directionHandler.DeactivateAttackDirectionImages();
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
