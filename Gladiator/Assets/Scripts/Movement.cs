using System;
using Unity.Cinemachine;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float smoothValue = 0.2f;
    private float inputX, inputY, maxValue;
    private Vector3 direction;
    private Camera cam;
    private Animator anim;
    private State playerState;

    void Start()
    {
        cam = Camera.main;
        anim = GetComponent<Animator>();
        playerState = GetComponent<State>();
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

        anim.SetFloat("InputX", inputX);
        anim.SetFloat("InputY", inputY);
    }

    private void NormalMovement()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !playerState.equipped)
        {
            maxValue = 2f;
            inputX = Input.GetAxis("Horizontal") * 2f;
            inputY = Input.GetAxis("Vertical") * 2f;
        }
        else
        {
            maxValue = 1f;
            inputX = Input.GetAxis("Horizontal");
            inputY = Input.GetAxis("Vertical");
        }

        direction = new Vector3(inputX, 0, inputY);
        anim.SetFloat("Locomotion", Vector3.ClampMagnitude(direction, maxValue).magnitude, smoothValue, Time.deltaTime);

        Vector3 rot = cam.transform.TransformDirection(direction);
        rot.y = 0;
        transform.forward = Vector3.Slerp(transform.forward, rot, Time.deltaTime * movementSpeed);
    }

    

}
