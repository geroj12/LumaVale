using UnityEngine;
using UnityEngine.UI;

public class CombatDirectionHandler : MonoBehaviour
{
    public Image strafeHoldImage;
    [SerializeField] private float holdDuration = 1.5f;

    private float holdTimer = 0f;
    private bool isHolding = false;
    private bool isSpawned = false;


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
    [SerializeField] private Transform visualAnchor;

    [Header("Radial UI")]
    [SerializeField] private GameObject radialUI;
    [SerializeField] private Image radialImage;

    [SerializeField] private Color leftColor = Color.yellow;
    [SerializeField] private Color rightColor = Color.blue;
    

    private Vector2 startPos;
    private float currentAngle;
    void Update()
    {
        HandleSwipeInput();
        HandleHoldInput();
        HandleRadialUI();

    }
    private void HandleHoldInput()
    {
        if (Input.GetKey(KeyCode.V))
        {
            holdTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(holdTimer / holdDuration);

            if (strafeHoldImage) strafeHoldImage.fillAmount = progress;

            if (!isHolding && holdTimer >= holdDuration)
            {
                // Toggle Logik
                if (!isSpawned)
                {
                    SpawnVisualFollower();
                    isSpawned = true;
                }
                else
                {
                    DespawnVisualFollower();
                    isSpawned = false;
                }

                isHolding = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.V))
        {
            holdTimer = 0f;
            isHolding = false;
            if (strafeHoldImage) strafeHoldImage.fillAmount = 0f;
        }
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
                spawnedVisual.BeginCharge();
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            isSwiping = false;
            EndSwipe(Input.mousePosition);

            if (spawnedVisual != null)
                spawnedVisual.EndCharge();
        }
    }
    private void HandleRadialUI()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            if (radialUI != null) radialUI.SetActive(true);
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 currentPos = Input.mousePosition;
            Vector2 direction = currentPos - startPos;

            if (direction.sqrMagnitude > 10f) // Deadzone
            {
                currentAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Sprite rotieren
                radialUI.transform.rotation = Quaternion.Euler(0, currentAngle, 0);

                // Farbe setzen
                UpdateColor(currentAngle);
            }
        }

       
    }
    private void UpdateColor(float angle)
    {
        if (angle < 0) angle += 360f;

        if (angle > 135 && angle <= 225)
            radialImage.color = leftColor;
        else
            radialImage.color = rightColor;
    }

    
    public void SpawnVisualFollower()
    {
        if (spawnedVisual == null)
        {

            spawnedVisual = Instantiate(visualFollowerPrefab, visualAnchor.position, Quaternion.identity);
            spawnVfxInstance = Instantiate(spawnVfxPrefab, visualAnchor.position, Quaternion.identity);
            spawnedVisual.anchorTransform = visualAnchor;
        }

        // Optional: Blickrichtung übergeben
        bool facingRight = visualAnchor.localScale.x > 0f;
        spawnedVisual.SetFacingDirection(facingRight);
    }

    private void DespawnVisualFollower()
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

    private void ResetMouseStates()
    {
        playerState.mouseOnLeftSide = false;
        playerState.mouseOnRightSide = false;
    }


}
