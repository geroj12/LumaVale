using System.Collections;
using UnityEngine;

public class Combat : MonoBehaviour
{
    private Animator anim;
    private State state;
    [SerializeField] private CombatDirectionHandler directionHandler;
    [Range(0.1f, 1f)] public float blockDelay = 0.5f;

    private float blockTimer = 0f;
    private float lastScrollValue = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        state = GetComponent<State>();
        directionHandler = GetComponent<CombatDirectionHandler>();
    }
    void Update()
    {
        Debug.Log("Scroll Value: " + Input.GetAxis("Mouse ScrollWheel"));
        HandleAttack();
        HandleBlocking();
    }

    public void HandleAttack()
    {
        if (state.blocking)
        {
            state.ResetMouseDirections();
        }

        if (state.isAttacking) return;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll < 0f && lastScrollValue == 0f)
        {
            state.attackThrust = true;
            anim.SetBool("attackThrust", true);
            StartCoroutine(ResetAttackBools());
        }

        if (scroll > 0f && lastScrollValue == 0f)
        {
            state.attackUp = true;
            anim.SetBool("attackUp", true);
            StartCoroutine(ResetAttackBools());
        }
        lastScrollValue = scroll;

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

    private void HandleBlocking()
    {
        if (!state.equipped) return;

        bool isHoldingBlock = Input.GetAxis("Fire2") > 0.1f;

        if (isHoldingBlock)
        {
            blockTimer += Time.deltaTime;
            if (blockTimer > blockDelay)
            {
                ActivateBlock();
                blockTimer = 0;
            }
        }
        else
        {
            DeactivateBlock();
            blockTimer = 0;
        }
    }
    private void ActivateBlock()
    {
        anim.SetBool("Block", true);
        //playerState.hurtBoxPlayer.GetComponent<BoxCollider>().enabled = false;
        state.blocking = true;
        //playerState.BlockCollider.GetComponent<BoxCollider>().enabled = true;
    }

    private void DeactivateBlock()
    {
        anim.SetBool("Block", false);
        //playerState.hurtBoxPlayer.GetComponent<BoxCollider>().enabled = true;
        state.blocking = false;
        //playerState.BlockCollider.GetComponent<BoxCollider>().enabled = false;
    }
    public void StartAttack()
    {
        state.isAttacking = true;
    }
    public void EndAttack()
    {
        state.isAttacking = false;
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
