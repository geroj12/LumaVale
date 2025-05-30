using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float smoothValue = 0.2f;
    private float inputX, inputY, maxValue;
    private Vector3 direction;
    private Camera cam;
    [SerializeField]private Animator anim;
    private State playerState;
    public NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        cam = Camera.main;
        playerState = GetComponent<State>();
        playerState.canMove = true;

        // Agent sollte keine eigene Rotation machen
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = movementSpeed;

    }


    public void HandleMovement()
    {
        if (playerState.attackThrust || playerState.attackUp) return;
        if (!playerState.canMove) return;
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

        anim.SetFloat("InputX", inputX, smoothValue, Time.deltaTime);
        anim.SetFloat("InputY", inputY, smoothValue, Time.deltaTime);

        Vector3 inputDir = new Vector3(inputX, 0, inputY);
        Vector3 worldDir = cam.transform.TransformDirection(inputDir);
        worldDir.y = 0;
        worldDir.Normalize();
        agent.Move(worldDir * movementSpeed * Time.deltaTime);
    }



    private void NormalMovement()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            maxValue = 2f;
            agent.speed = movementSpeed * 2f;

            inputX = Input.GetAxis("Horizontal") * 2f;
            inputY = Input.GetAxis("Vertical") * 2f;
        }
        else
        {
            maxValue = 1f;
            agent.speed = movementSpeed;

            inputX = Input.GetAxis("Horizontal");
            inputY = Input.GetAxis("Vertical");
        }

        direction = new Vector3(inputX, 0, inputY);
        anim.SetFloat("Locomotion", Vector3.ClampMagnitude(direction, maxValue).magnitude, smoothValue, Time.deltaTime);

        Vector3 move = cam.transform.TransformDirection(direction);
        move.y = 0;
        move.Normalize();

        // Neue Bewegung über NavMeshAgent
        agent.Move(move * agent.speed * Time.deltaTime);

        // Schnelle und präzise Rotation zur Bewegungsrichtung
        if (move.magnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 540f * Time.deltaTime); // 720 Grad/s
        }
    }



}
