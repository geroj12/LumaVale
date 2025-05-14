using UnityEngine;

public class CombatDirectionHandler : MonoBehaviour
{
    [Header("UI Images")]
    [SerializeField] private GameObject rightImg, leftImg, topImg, midImg, blockImg;

    [Header("References")]
    [SerializeField] private State playerState;

    [Header("Settings")]
    [Range(5f, 45f)] public float directionToleranceAngle = 30f;
    [Range(0.1f, 1f)] public float blockDelay = 0.5f;
    
    private float blockTimer = 0f;
    private Vector2 smoothedMouseDelta;

    void Update()
    {
       
        HandleBlocking();
    }

   

    public void HandleAttackDirectionUI()
    {
        if (!playerState.Strafe || playerState.blocking || !Input.GetMouseButton(0))
        {
            DeactivateAttackDirectionImages();
            return;
        }

        Vector2 rawMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        smoothedMouseDelta = Vector2.Lerp(smoothedMouseDelta, rawMouseDelta, Time.fixedDeltaTime * 10f);

        if (smoothedMouseDelta.magnitude < 0.1f)
        {
            DeactivateAttackDirectionImages();
            return;
        }

        float angle = Vector2.SignedAngle(Vector2.up, smoothedMouseDelta);

        ResetMouseStates();

        // Determine direction
        if (Mathf.Abs(angle) <= directionToleranceAngle)
        {
            ActivateUI(topImg);
            playerState.mouseOnTopSide = true;
        }
        else if (Mathf.Abs(angle - 180f) <= directionToleranceAngle || Mathf.Abs(angle + 180f) <= directionToleranceAngle)
        {
            ActivateUI(midImg);
            playerState.mouseOnDownSide = true;
        }
        else if (angle < -90f + directionToleranceAngle && angle > -90f - directionToleranceAngle)
        {
            ActivateUI(leftImg);
            playerState.mouseOnLeftSide = true;
        }
        else if (angle > 90f - directionToleranceAngle && angle < 90f + directionToleranceAngle)
        {
            ActivateUI(rightImg);
            playerState.mouseOnRightSide = true;
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
        topImg.SetActive(uiElement == topImg);
        midImg.SetActive(uiElement == midImg);
    }

    public void DeactivateAttackDirectionImages()
    {
        rightImg.SetActive(false);
        leftImg.SetActive(false);
        topImg.SetActive(false);
        midImg.SetActive(false);
        ResetMouseStates();
    }

    private void ResetMouseStates()
    {
        playerState.mouseOnLeftSide = false;
        playerState.mouseOnRightSide = false;
        playerState.mouseOnTopSide = false;
        playerState.mouseOnDownSide = false;
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
