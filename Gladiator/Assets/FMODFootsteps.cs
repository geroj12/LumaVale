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
    // Optional: Mindestgeschwindigkeit ab der Sound abgespielt wird

    void Start()
    {
        RuntimeManager.LoadBank("Master", true);
    }

    public void PlayFootstepWalkAudio()
    {
        float animSpeed = animator.GetFloat("Locomotion");
        if (animSpeed < 0.1f || !detectGround.isGrounded)
            return;

        Debug.Log("PlayFootstepWalkAudio triggered"); // Zum Debuggen
        var instance = RuntimeManager.CreateInstance(terrainFootstepEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        instance.setParameterByNameWithLabel("Terrain", detectGround.currentTerrainLabel);
        instance.start();
        instance.release(); // wichtig, sonst Memory-Leak
    }

    private void OnDestroy()
    {
        if (instance.isValid())
        {
            instance.release(); // Fallback
        }
    }
}
