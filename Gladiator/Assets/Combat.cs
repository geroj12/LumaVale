using System.Collections;
using UnityEngine;

public class Combat : MonoBehaviour
{
    private Animator anim;
    private State state;
    [SerializeField] private CombatDirectionHandler directionHandler;

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

        if (Input.GetMouseButtonUp(0))
        {
            if (state.mouseOnRightSide) anim.SetBool("attackRight", true);
            if (state.mouseOnLeftSide) anim.SetBool("attackLeft", true);
            if (state.mouseOnDownSide) anim.SetBool("attackThrust", true);
            if (state.mouseOnTopSide) anim.SetBool("attackUp", true);

            StartCoroutine(ResetAttackBools());
            directionHandler.DeactivateAttackDirectionImages();
        }
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
