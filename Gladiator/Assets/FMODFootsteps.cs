using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.AI;

public class FMODFootsteps : MonoBehaviour
{
    private EventInstance instance;

    [SerializeField]
    private EventReference terrainFootstepEvent; // NEU: korrektes EventReference-Feld

    [SerializeField]
    private GroundCheck detectGround;
    [SerializeField] private Animator animator;


    [SerializeField] private Transform playerTransform;
    private Vector3 lastPosition;
    // Optional: Mindestgeschwindigkeit ab der Sound abgespielt wird
    [SerializeField]
    private float minSpeed = 0.1f;

    [SerializeField]
    private float stepInterval = 0.5f;

    private float stepTimer = 0f;
    private float lastStepTime = 0f;
    [SerializeField]
    private float minStepInterval = 0.3f; // für Schutz gegen Doppel-Trigger
    [SerializeField] private State state;
    void Start()
    {
        RuntimeManager.LoadBank("Master", true);
        lastPosition = playerTransform.position;

    }
    void Update()
    {
        if (state.Strafe)
        {
            float inputX = animator.GetFloat("InputX");
            float inputY = animator.GetFloat("InputY");
            float inputMagnitude = new Vector2(inputX, inputY).magnitude;

            // Bewegungsgeschwindigkeit (Distanz seit letztem Frame)
            float moveSpeed = Vector3.Distance(playerTransform.position, lastPosition) / Time.deltaTime;

            lastPosition = playerTransform.position;

            // Check, ob Input und Bewegungsgeschwindigkeit beide über Threshold sind
            if (inputMagnitude > minSpeed && moveSpeed > 0.1f && detectGround.isGrounded)
            {
                stepTimer -= Time.deltaTime;
                if (stepTimer <= 0f && (Time.time - lastStepTime) > minStepInterval)
                {
                    PlayFootstepWalkAudio();
                    stepTimer = stepInterval;
                    lastStepTime = Time.time;
                }
            }
            else
            {
                stepTimer = 0f;
            }
        }
    }
    public void PlayFootstepWalkAudio()
    {
        if (!detectGround.isGrounded)
            return;

        if (!state.Strafe)
        {
            float speed = animator.GetFloat("InputMagnitude");
            if (speed < minSpeed)
                return;
        }

        Debug.Log("PlayFootstepWalkAudio triggered");

        instance = RuntimeManager.CreateInstance(terrainFootstepEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        instance.setParameterByNameWithLabel("Terrain", detectGround.currentTerrainLabel);
        instance.start();
        instance.release();
    }

    private void OnDestroy()
    {
        if (instance.isValid())
        {
            instance.release();
        }
    }
}

