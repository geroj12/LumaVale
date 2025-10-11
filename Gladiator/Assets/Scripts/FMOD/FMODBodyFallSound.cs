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


    internal void PlayBodyFallOnGround(float impactForce)
    {
        instance = RuntimeManager.CreateInstance(bodyFallsOnGroundEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

        // Dynamische Parameter
        float volume = Mathf.Clamp01(impactForce / 10f); // skaliert zwischen 0 und 1
        instance.setVolume(volume);

        float pitch = 1f + Random.Range(-0.1f, 0.1f); // leichte Variation für Realismus
        instance.setPitch(pitch);

        instance.start();
        instance.release();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasPlayed) return;
        if (Time.time - spawnTime < 0.2f) return; // Warte kurz nach Spawn


        if (collision.gameObject.CompareTag("Ground")) // oder ein anderer passender Tag
        {
            float impactForce = collision.relativeVelocity.magnitude;
            Debug.Log(collision.relativeVelocity.magnitude);
            // Optional: nur abspielen, wenn es stark genug ist
            if (impactForce > 1f)
            {
                PlayBodyFallOnGround(impactForce);
                hasPlayed = true;
            }
        }
    }
}
