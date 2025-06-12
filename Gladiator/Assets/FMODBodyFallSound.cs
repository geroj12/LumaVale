using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODBodyFallSound : MonoBehaviour
{
    [SerializeField]
    private EventReference bodyFallsOnGroundEvent; // NEU: korrektes EventReference-Feld
    private EventInstance instance;
    private bool hasPlayed = false;
    private float spawnTime;
   
    void Start()
    {
        spawnTime = Time.time;
    }


    internal void PlayBodyFallOnGround()
    {
        instance = RuntimeManager.CreateInstance(bodyFallsOnGroundEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

        instance.start();
        instance.release();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasPlayed) return;
        if (Time.time - spawnTime < 0.2f) return; // Warte kurz nach Spawn


        if (collision.gameObject.CompareTag("Ground")) // oder ein anderer passender Tag
        {
            PlayBodyFallOnGround();
            hasPlayed = true;
        }
    }
}
