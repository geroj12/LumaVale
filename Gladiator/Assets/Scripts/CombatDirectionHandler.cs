using UnityEngine;
using UnityEngine.UI;

public class CombatDirectionHandler : MonoBehaviour
{
    public float minSwipeDistance = 50f;

    private Vector2 swipeStartPos;
    private bool isSwiping = false;
    [Header("Visual Prefab")]

    [SerializeField] private CombatVisualFollower visualFollowerPrefab;
    private CombatVisualFollower spawnedVisual;
    [SerializeField] private GameObject spawnVfxPrefab;
    private GameObject spawnVfxInstance;

    [Header("References")]
    [SerializeField] private State playerState;
    [SerializeField] private Transform visualAnchor; // ← setz im Inspector den Anchor vom Spieler

    void Update()
    {
        HandleSwipeInput();
        HandleVisualFollower();


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

        // ➕ Das Wesen auf den Swipe reagieren lassen
        if (spawnedVisual != null)
        {
            Vector2 swipeDir = swipeDelta.normalized;
            spawnedVisual.ReactToSwipe(swipeDir);
        }
    }

    private void HandleSwipeInput()
    {
        if (!playerState.Strafe || playerState.blocking) return;

        if (Input.GetMouseButtonDown(0))
        {
            isSwiping = true;
            swipeStartPos = Input.mousePosition;

            if (spawnedVisual != null)
                spawnedVisual.BeginCharge(); // → Ladeanzeige
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            isSwiping = false;
            EndSwipe(Input.mousePosition);

            if (spawnedVisual != null)
                spawnedVisual.EndCharge(); // → Ladeanzeige aus
        }
    }
    private void HandleVisualFollower()
    {
        if (playerState.Strafe && !playerState.blocking)
        {

            if (spawnedVisual == null)
            {

                spawnedVisual = Instantiate(visualFollowerPrefab, visualAnchor.position, Quaternion.identity);
                spawnVfxInstance = Instantiate(spawnVfxPrefab, visualAnchor.position, Quaternion.identity, visualAnchor);
                spawnedVisual.anchorTransform = visualAnchor;
            }

            // Optional: Blickrichtung übergeben
            bool facingRight = visualAnchor.localScale.x > 0f;
            spawnedVisual.SetFacingDirection(facingRight);
        }
        else
        {
            if (spawnedVisual != null)
            {
                Destroy(spawnedVisual.gameObject);
                spawnedVisual = null;
            }
            if (spawnVfxInstance != null)
            {
                Destroy(spawnVfxInstance);
                spawnVfxInstance = null;
            }
        }
    }
    private void ResetMouseStates()
    {
        playerState.mouseOnLeftSide = false;
        playerState.mouseOnRightSide = false;
    }


}
