using Assets.SimpleSpinner;
using UnityEngine;

public class HandleSwipeUI : MonoBehaviour
{
    [SerializeField] private State playerState;
    [SerializeField] private GameObject leftSwipeUI;
    [SerializeField] private GameObject rightSwipeUI;
    [SerializeField] private GameObject blockUI;
    [SerializeField] private GameObject holdAttackUI;

    private Camera mainCamera;

    [Header("Hold Attack Settings")]
    [SerializeField] private float maxHoldTime = 5f;
    [SerializeField] private float minSpinnerSpeed = .5f;
    [SerializeField] private float maxSpinnerSpeed = 1f;
    private float holdTimer = 0f;
    private SpinnerSwipeUI spinner;
    void Start()
    {
        mainCamera = Camera.main;
        if (holdAttackUI != null)
            spinner = holdAttackUI.GetComponent<SpinnerSwipeUI>();
    }
    void Update()
    {
        if (mainCamera == null) return;

        if (playerState.blocking)
        {
            blockUI.SetActive(true);
        }
        else
        {
            blockUI.SetActive(false);
        }

        if (playerState.mouseOnLeftSide)
        {
            leftSwipeUI.SetActive(true);
            rightSwipeUI.SetActive(false);

        }
        else if (playerState.mouseOnRightSide)
        {
            leftSwipeUI.SetActive(false);
            rightSwipeUI.SetActive(true);
        }
        else
        {
            leftSwipeUI.SetActive(false);
            rightSwipeUI.SetActive(false);
        }


        if (playerState.holdingAttack)
        {
            holdAttackUI.SetActive(true);

            // Timer hochzählen, max. maxHoldTime
            holdTimer += Time.deltaTime;
            holdTimer = Mathf.Min(holdTimer, maxHoldTime);

            // Spinner-Geschwindigkeit dynamisch anpassen
            if (spinner != null)
            {
                float t = holdTimer / maxHoldTime; // 0..1
                float speed = Mathf.Lerp(minSpinnerSpeed, maxSpinnerSpeed, t);
                spinner.SetRotationSpeed(speed);
            }
        }
        else
        {
            holdAttackUI.SetActive(false);
            holdTimer = 0f;

            // Spinner zurücksetzen
            if (spinner != null)
            {
                spinner.SetRotationSpeed(minSpinnerSpeed);
            }
        }


    }


}
