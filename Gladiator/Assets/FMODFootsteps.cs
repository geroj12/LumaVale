using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.AI;

public class FMODFootsteps : MonoBehaviour
{
    private EventInstance instance;

    [SerializeField] private EventReference terrainFootstepEvent;
    [SerializeField] private GroundCheck detectGround;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private State state;

    private Vector3 lastPosition;
    private float stepTimer = 0f;
    private float lastStepTime = 0f;

    [Header("Step Settings")]
    [SerializeField] private float minSpeed = 0.1f;
    [SerializeField] private float baseStepInterval = 0.5f;
    [SerializeField] private float minStepInterval = 0.3f;
    
    void Start()
    {
        RuntimeManager.LoadBank("Master", true);
        lastPosition = playerTransform.position;

    }
    void Update()
    {
        float inputMagnitude = animator.GetFloat("InputMagnitude");

        float moveSpeed = Vector3.Distance(playerTransform.position, lastPosition) / Time.deltaTime;
        lastPosition = playerTransform.position;

        // Nur wenn Bewegung und grounded
        if (inputMagnitude > minSpeed && moveSpeed > 0.05f && detectGround.isGrounded)
        {
            float dynamicStepInterval = baseStepInterval / Mathf.Max(1f, inputMagnitude); // z.B. schneller = häufiger Schritte
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f && (Time.time - lastStepTime) > minStepInterval)
            {
                PlayFootstepWalkAudio();
                stepTimer = dynamicStepInterval;
                lastStepTime = Time.time;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }
    public void PlayFootstepWalkAudio()
    {
        if (!detectGround.isGrounded) return;


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

