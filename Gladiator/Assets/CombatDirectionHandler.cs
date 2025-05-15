using UnityEngine;

public class CombatDirectionHandler : MonoBehaviour
{
    [Header("UI Images")]
    [SerializeField] private GameObject rightImg, leftImg, blockImg;


    [Header("References")]
    [SerializeField] private State playerState;

    [Header("Settings")]
    [Range(5f, 45f)] public float directionToleranceAngle = 30f;
    [Range(0.1f, 1f)] public float blockDelay = 0.5f;

    private float blockTimer = 0f;
    private Vector2 smoothedMouseDelta;
    public float minSwipeDistance = 50f;
    private Vector2 swipeStartPos;
    private bool isSwiping = false;
    void Update()
    {

        HandleBlocking();
    }

    public void StartSwipe(Vector2 startPosition)
    {
        swipeStartPos = startPosition;
        isSwiping = true;
    }

    public void EndSwipe(Vector2 endPosition)
    {
        if (!isSwiping || !playerState.Strafe || playerState.blocking)
        {
            ResetMouseStates();
            return;
        }

        Vector2 swipeDelta = endPosition - swipeStartPos;
        isSwiping = false;

        if (swipeDelta.magnitude < minSwipeDistance)
        {
            ResetMouseStates();
            DeactivateAttackDirectionImages();
            return;
        }

        float angle = Vector2.SignedAngle(Vector2.up, swipeDelta.normalized);
        ResetMouseStates();

        if (angle < -90f + directionToleranceAngle && angle > -90f - directionToleranceAngle)
        {
            ActivateUI(leftImg);
            playerState.mouseOnLeftSide = true;
        }
        else if (angle > 90f - directionToleranceAngle && angle < 90f + directionToleranceAngle)
        {
            ActivateUI(rightImg);
            playerState.mouseOnRightSide = true;
        }
        else
        {
            DeactivateAttackDirectionImages();
        }
    }

    private void HandleBlocking()
    {
        if (!playerState.equipped) return;

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

    private void ActivateUI(GameObject uiElement)
    {
        rightImg.SetActive(uiElement == rightImg);
        leftImg.SetActive(uiElement == leftImg);

    }

    public void DeactivateAttackDirectionImages()
    {
        rightImg.SetActive(false);
        leftImg.SetActive(false);

        ResetMouseStates();
    }

    private void ResetMouseStates()
    {
        playerState.mouseOnLeftSide = false;
        playerState.mouseOnRightSide = false;

    }

    private void ActivateBlock()
    {
        //playerState.anim.SetBool("Block", true);
        //playerState.hurtBoxPlayer.GetComponent<BoxCollider>().enabled = false;
        playerState.blocking = true;
        blockImg.SetActive(true);
        //playerState.BlockCollider.GetComponent<BoxCollider>().enabled = true;
        DeactivateAttackDirectionImages();
    }

    private void DeactivateBlock()
    {
        //playerState.anim.SetBool("Block", false);
        //playerState.hurtBoxPlayer.GetComponent<BoxCollider>().enabled = true;
        playerState.blocking = false;
        blockImg.SetActive(false);
        //playerState.BlockCollider.GetComponent<BoxCollider>().enabled = false;
    }
}
