using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODShieldFallsOnGround : MonoBehaviour
{
    [SerializeField]
    private EventReference shieldFallsOnGroundEvent; // NEU: korrektes EventReference-Feld
    private EventInstance instance;
    private bool hasPlayed = false;
    private float spawnTime;
   
    void Start()
    {
        spawnTime = Time.time;
    }
    internal void PlayShieldFallGround()
    {
        instance = RuntimeManager.CreateInstance(shieldFallsOnGroundEvent);
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
            PlayShieldFallGround();
            hasPlayed = true;
        }
    }
  
}
