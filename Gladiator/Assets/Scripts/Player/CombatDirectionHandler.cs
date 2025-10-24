using UnityEngine;

public class CombatDirectionHandler : MonoBehaviour
{

    public float minSwipeDistance = 50f;
    private Vector2 swipeStartPos;
    private bool isSwiping = false;
    [Header("Visual Prefab")]


    [Header("References")]
    [SerializeField] private State playerState;


    void Update()
    {

        HandleSwipeInput();

    }


    public void StartSwipe(Vector2 startPosition)
    {

        swipeStartPos = startPosition;
        isSwiping = true;
    }

    public void EndSwipe(Vector2 endPosition)
    {
        if (!playerState.Strafe || playerState.blocking)
        {
            ResetMouseStates();
            return;
        }

        Vector2 swipeDelta = endPosition - swipeStartPos;
        isSwiping = false;

        if (swipeDelta.magnitude < minSwipeDistance)
        {
            ResetMouseStates();
            return;
        }
        ResetMouseStates();

        bool fromRight = swipeDelta.x < 0f;

        if (fromRight)
            playerState.mouseOnRightSide = true;
        else
            playerState.mouseOnLeftSide = true;
    }

    private void HandleSwipeInput()
    {
        if (!playerState.Strafe || playerState.blocking) return;

        if (Input.GetMouseButtonDown(0))
        {
            isSwiping = true;
            swipeStartPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            isSwiping = false;
            EndSwipe(Input.mousePosition);

        }
    }


    private void ResetMouseStates()
    {
        playerState.mouseOnLeftSide = false;
        playerState.mouseOnRightSide = false;
    }


}
