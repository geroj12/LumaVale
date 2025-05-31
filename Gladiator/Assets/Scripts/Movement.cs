using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float smoothValue = 0.2f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float rotationSpeed = 10f;

    private float inputX, inputY, maxValue;
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
    }

    void FixedUpdate()
    {
        ApplyGravity(); // sorgt dafür, dass wir nicht in der Luft schweben
    }

    public void HandleMovement()
    {
        if (playerState.Strafe)
        {
            StrafeMovement();
        }
        else
        {
            NormalMovement();
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
        Vector3 finalMove = move * movementSpeed;
        finalMove.y = velocity.y;

        controller.Move(finalMove * Time.deltaTime);

        anim.SetFloat("InputX", inputX,smoothValue, Time.deltaTime);
        anim.SetFloat("InputY", inputY,smoothValue, Time.deltaTime);
    }

    private void NormalMovement()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        Vector3 rawDir = new Vector3(inputX, 0, inputY);
        Vector3 moveDir = cam.transform.TransformDirection(rawDir);
        moveDir.y = 0f;
        moveDir = Vector3.ClampMagnitude(moveDir, 1f).normalized;

        // Bewegungsgeschwindigkeit
        float sprintMultiplier = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;
        Vector3 finalMove = moveDir * movementSpeed * sprintMultiplier;
        finalMove.y = velocity.y;

        controller.Move(finalMove * Time.deltaTime);

        // Berechne weichen Übergang für Animation
        float moveAmount = rawDir.magnitude * sprintMultiplier;
        moveAmount = Mathf.Min(moveAmount, 2f); // Max-Wert für Animation

        anim.SetFloat("Locomotion", moveAmount, smoothValue, Time.deltaTime);

        if (moveDir != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            velocity.y = -1f; // "kleiner Druck", um grounded zu bleiben
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }
    }

}

// direction = new Vector3(inputX, 0, inputY);
// anim.SetFloat("Locomotion", Vector3.ClampMagnitude(direction, maxValue).magnitude, smoothValue, Time.deltaTime);

// Vector3 rot = cam.transform.TransformDirection(direction);
// rot.y = 0;
// transform.forward = Vector3.Slerp(transform.forward, rot, Time.deltaTime * movementSpeed);'