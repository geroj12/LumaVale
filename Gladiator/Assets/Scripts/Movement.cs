using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float smoothValue = 0.2f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float sprintMultiplier = 2f;
    private float inputAngle; // global verfügbar

    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    private Transform rightFoot;
    private Transform leftFoot;
    private float legSwitch;
    private float inputX, inputY;

    private Camera cam;
    private Animator anim;
    private State playerState;
    private CharacterController controller;
    private Vector3 velocity;


    void Start()
    {
        cam = Camera.main;
        anim = GetComponent<Animator>();
        playerState = GetComponent<State>();
        controller = GetComponent<CharacterController>();
        rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
    }

    void FixedUpdate()
    {
        ApplyGravity();
    }

    public void HandleMovement()
    {
        if (playerState.Strafe)
        {
            StrafeMovement();
        }
        else
        {
            FreeLookMovement(); // neue Methode ersetzt NormalMovement()
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            playerState.Strafe = !playerState.Strafe;
            anim.SetBool("Strafe", playerState.Strafe);
        }
    }

    private void StrafeMovement()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        float camRot = cam.transform.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, camRot, 0), 4f * Time.deltaTime);

        Vector3 move = (transform.right * inputX + transform.forward * inputY).normalized;

        // Magnitude berechnen (für Blendtree)
        Vector2 inputVector = new Vector2(inputX, inputY);
        float inputMagnitude = Mathf.Clamp01(inputVector.magnitude);

       
        float currentSpeed = movementSpeed;

        Vector3 finalMove = move * currentSpeed;
        finalMove.y = velocity.y;

        controller.Move(finalMove * Time.deltaTime);

        anim.SetFloat("InputX", inputX, smoothValue, Time.deltaTime);
        anim.SetFloat("InputY", inputY, smoothValue, Time.deltaTime);

        float animMagnitude = inputMagnitude;
        anim.SetFloat("InputMagnitude", animMagnitude, smoothValue, Time.deltaTime);

        if (move.magnitude > 0.1f)
        {
            inputAngle = Vector3.SignedAngle(transform.forward, move, Vector3.up);
            anim.SetFloat("InputAngle", inputAngle);
        }
        else
        {
            anim.SetFloat("InputAngle", 0f);
        }
    }

    private void FreeLookMovement()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        Vector2 inputVector = new Vector2(inputX, inputY);
        float inputMagnitude = Mathf.Clamp01(inputVector.magnitude);

        // Sprint-Check
        bool isSprinting = Input.GetKey(sprintKey);
        float currentSpeed = movementSpeed * (isSprinting ? sprintMultiplier : 1f);

        // Kamera-basiert bewegen
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cam.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 moveDir = camForward * inputY + camRight * inputX;
        moveDir.Normalize();

        Vector3 finalMove = moveDir * movementSpeed;
        finalMove.y = velocity.y;

        controller.Move(finalMove * Time.deltaTime);

        // Animation: Bewegung relativ zur eigenen Blickrichtung (local space)
        Vector3 localMoveDir = transform.InverseTransformDirection(moveDir);

        anim.SetFloat("Horizontal", localMoveDir.x, smoothValue, Time.deltaTime);
        anim.SetFloat("Vertical", localMoveDir.z, smoothValue, Time.deltaTime);
        anim.SetFloat("InputMagnitude", inputMagnitude, smoothValue, Time.deltaTime);
        // InputMagnitude für Blend Tree (z. B. 0–2 für Idle/Walk/Sprint)
        float animMagnitude = inputMagnitude * (isSprinting ? sprintMultiplier : 1f);
        anim.SetFloat("InputMagnitude", animMagnitude, smoothValue, Time.deltaTime);
        // Optional: Richtungswinkel für z. B. RotationDirection
        inputAngle = Vector3.SignedAngle(transform.forward, moveDir, Vector3.up);
        anim.SetFloat("InputDirection", inputAngle);
        anim.SetFloat("RotationDirection", inputAngle);
        anim.SetFloat("InputAngle", inputAngle); // für WalkStarts

        // Rotation des Charakters (nur bei Bewegung)
        if (inputMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        if (rightFoot != null && leftFoot != null)
        {
            // IsRightLegUp setzen (einfach: wer hat höheren y-Wert)
            bool isRightLegUp = rightFoot.position.y > leftFoot.position.y;
            anim.SetBool("IsRightLegUp", isRightLegUp);

            // LegSwitch steuern (nur bei Bewegung sinnvoll)
            if (inputMagnitude > 0.1f)
            {
                float targetLegSwitch = isRightLegUp ? 1f : 0f;
                // Nur wechseln, wenn Beine "auseinander" sind – einfache Bedingung:
                float legDistance = Mathf.Abs(rightFoot.position.x - leftFoot.position.x);
                if (legDistance > 0.1f) // Schwelle zum Wechsel
                {
                    legSwitch = Mathf.MoveTowards(legSwitch, targetLegSwitch, Time.deltaTime * 5f);
                }
            }

            anim.SetFloat("LegSwitch", legSwitch);
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            velocity.y = -1f;
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }
    }
}
