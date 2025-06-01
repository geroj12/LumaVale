using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FemalePlayerMovement : MonoBehaviour
{
    public Animator animator;
    public Transform cameraTransform;
    public Transform rightFoot;
    public Transform leftFoot;

    public float moveSpeed = 2.0f;
    public float rotationSpeed = 10.0f;

    private CharacterController controller;
    private Vector2 input;
    private float inputMagnitude;
    private float rotationDirection;
    private float legSwitch = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
        UpdateAnimatorParameters();
    }

    void HandleInput()
    {
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        inputMagnitude = Mathf.Clamp01(input.magnitude);
    }

    void HandleMovement()
    {
        if (inputMagnitude > 0.1f)
        {
            // Kamera-Vektoren berechnen
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0;
            camForward.Normalize();

            Vector3 camRight = cameraTransform.right;
            camRight.y = 0;
            camRight.Normalize();

            // Richtungsvektor relativ zur Kamera
            Vector3 moveDir = camForward * input.y + camRight * input.x;
            moveDir.Normalize();

            // Bewegung anwenden
            controller.Move(moveDir * moveSpeed * Time.deltaTime);

            // Rotation
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Input auf lokalen Raum des Charakters übertragen (für Horizontal/Vertical)
            Vector3 localInput = transform.InverseTransformDirection(moveDir);
            animator.SetFloat("Horizontal", localInput.x);
            animator.SetFloat("Vertical", localInput.z);

            // Winkel nur für Debug oder Spezial-Übergänge
            rotationDirection = Vector3.SignedAngle(transform.forward, moveDir, Vector3.up);
        }
        else
        {
            animator.SetFloat("Horizontal", 0f);
            animator.SetFloat("Vertical", 0f);
            rotationDirection = 0f;
        }
    }

    void UpdateAnimatorParameters()
    {
        animator.SetFloat("InputMagnitude", inputMagnitude);
        animator.SetFloat("RotationDirection", rotationDirection);

        bool isRightLegUp = rightFoot.position.y > leftFoot.position.y;
        animator.SetBool("IsRightLegUp", isRightLegUp);

        // Testweise LegSwitch deaktivieren
        animator.SetFloat("LegSwitch", 0f);
    }
}
