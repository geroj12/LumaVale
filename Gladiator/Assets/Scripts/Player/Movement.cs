using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float smoothValue = 0.2f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

    private Camera cam;
    private Animator anim;
    private CharacterController controller;
    private State playerState;

    private Vector3 velocity;
    private float inputX, inputY;
    private float inputAngle;

    public float inputMagnitude { get; private set; }

    void Start()
    {
        cam = Camera.main;
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerState = GetComponent<State>();
    }

    void FixedUpdate() => ApplyGravity();

    public void HandleMovement()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        Vector2 inputVector = new(inputX, inputY);
        inputMagnitude = Mathf.Clamp01(inputVector.magnitude);

        if (playerState.Strafe)
            HandleStrafeMovement();
        else
            HandleFreeLookMovement();
    }

    private void HandleStrafeMovement()
    {
        // Kameraausrichtung übernehmen
        float camY = cam.transform.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, camY, 0), rotationSpeed * Time.deltaTime);

        Vector3 move = (transform.right * inputX + transform.forward * inputY).normalized;
        MoveCharacter(move, movementSpeed);

        // Animation
        anim.SetFloat("InputX", inputX, smoothValue, Time.deltaTime);
        anim.SetFloat("InputY", inputY, smoothValue, Time.deltaTime);
        anim.SetFloat("InputMagnitude", inputMagnitude, smoothValue, Time.deltaTime);

        inputAngle = move.magnitude > 0.1f ? Vector3.SignedAngle(transform.forward, move, Vector3.up) : 0f;
        anim.SetFloat("InputAngle", inputAngle);
    }

    private void HandleFreeLookMovement()
    {
        bool isSprinting = Input.GetKey(sprintKey);
        float currentSpeed = movementSpeed * (isSprinting ? sprintMultiplier : 1f);

        Vector3 camForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
        Vector3 moveDir = (camForward * inputY + camRight * inputX).normalized;

        MoveCharacter(moveDir, currentSpeed);

        // Rotation nur bei Bewegung
        if (inputMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            inputAngle = Vector3.SignedAngle(transform.forward, moveDir, Vector3.up);
        }
        else inputAngle = 0f;

        // Animation
        Vector3 localMove = transform.InverseTransformDirection(moveDir);
        anim.SetFloat("Horizontal", localMove.x, smoothValue, Time.deltaTime);
        anim.SetFloat("Vertical", localMove.z, smoothValue, Time.deltaTime);
        anim.SetFloat("InputMagnitude", inputMagnitude * (isSprinting ? sprintMultiplier : 1f), smoothValue, Time.deltaTime);
        anim.SetFloat("InputAngle", inputAngle);
    }

    private void MoveCharacter(Vector3 direction, float speed)
    {
        Vector3 move = direction * speed;
        move.y = velocity.y;
        controller.Move(move * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        velocity.y = controller.isGrounded ? -1f : velocity.y - gravity * Time.deltaTime;
    }
}
