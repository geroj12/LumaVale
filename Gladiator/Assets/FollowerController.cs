using UnityEngine;

public class FollowerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;

    private FollowerAnimator followerAnimator;
    private Vector3 inputDirection;

    private void Awake()
    {
        followerAnimator = GetComponent<FollowerAnimator>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        inputDirection = new Vector3(h, 0, v).normalized;

        if (inputDirection.magnitude > 0.1f)
        {
            Vector3 move = inputDirection * moveSpeed * Time.deltaTime;
            transform.position += move;

            followerAnimator.SetMoving(true);
        }
        else
        {
            followerAnimator.SetMoving(false);
        }
    }
}
