using UnityEngine;
using UnityEngine.UI;

public class CombatDirectionHandler : MonoBehaviour
{
    [Header("Swipe Visuals")]
    [SerializeField] private RectTransform swipeVisual;
    [SerializeField] private Image swipeVisualImage;
    [SerializeField] private RectTransform swipeArrow;
    [SerializeField] private Canvas canvas;

    [SerializeField] private Gradient swipeColorByStrength;
    public float minSwipeDistance = 50f;

    private Vector2 swipeStartPos;
    private bool isSwiping = false;

    [Header("References")]
    [SerializeField] private State playerState;

    [Header("Settings")]
    [Range(5f, 45f)] public float directionToleranceAngle = 30f;

    void Update()
    {
        HandleSwipeVisual();

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

        float angle = Vector2.SignedAngle(Vector2.up, swipeDelta.normalized);
        ResetMouseStates();

        if (angle < -90f + directionToleranceAngle && angle > -90f - directionToleranceAngle)
        {
            playerState.mouseOnLeftSide = true;
        }
        else if (angle > 90f - directionToleranceAngle && angle < 90f + directionToleranceAngle)
        {
            playerState.mouseOnRightSide = true;
        }

    }

    private void HandleSwipeVisual()
    {
        if (!playerState.Strafe || playerState.blocking) return;

        if (Input.GetMouseButtonDown(0))
        {
            isSwiping = true;
            swipeStartPos = Input.mousePosition;

            swipeVisual.gameObject.SetActive(true);
            swipeArrow.gameObject.SetActive(true);

            swipeVisual.position = swipeStartPos;
            swipeArrow.position = swipeStartPos;
        }

        if (Input.GetMouseButton(0) && isSwiping)
        {
            Vector2 currentPos = Input.mousePosition;
            Vector2 delta = currentPos - swipeStartPos;
            float distance = delta.magnitude;

            // Scale visual based on distance
            swipeVisual.sizeDelta = new Vector2(distance, 12f);
            swipeVisual.position = swipeStartPos;
            swipeVisual.rotation = Quaternion.FromToRotation(Vector2.right, delta.normalized);

            // Move arrow to the end
            swipeArrow.position = swipeStartPos + delta.normalized * distance;

            // Color by strength
            float t = Mathf.InverseLerp(0f, 200f, distance);
            swipeVisualImage.color = swipeColorByStrength.Evaluate(t);
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            isSwiping = false;
            swipeVisual.gameObject.SetActive(false);
            swipeArrow.gameObject.SetActive(false);
        }
    }


    private void ResetMouseStates()
    {
        playerState.mouseOnLeftSide = false;
        playerState.mouseOnRightSide = false;

    }


}
