using FMOD.Studio;
using FMODUnity;
using UnityEngine;



[RequireComponent(typeof(Collider))]
public class CampfireSound : MonoBehaviour
{
    [SerializeField] private EventReference campfireEvent;

    [SerializeField] private Transform player; // Referenz direkt zuweisbar im Inspector
    [SerializeField] private float fadeSpeed = 2f;

    private EventInstance campfireInstance;
    private float currentProximity = 0f;
    private bool isPlayerInside = false;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;

        if (player == null)
        {
            Debug.LogWarning("⚠️ [CampfireSound] Player reference not assigned in Inspector.");
        }

        campfireInstance = RuntimeManager.CreateInstance(campfireEvent);
        campfireInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        campfireInstance.start();
    }

    private void Update()
    {
        float target = isPlayerInside ? 1f : 0f;

        // Nur aktualisieren, wenn sich was verändert
        if (!Mathf.Approximately(currentProximity, target))
        {
            currentProximity = Mathf.MoveTowards(currentProximity, target, fadeSpeed * Time.deltaTime);
            campfireInstance.setParameterByName("Proximity", currentProximity);
        }

        // Optional: aktualisiere 3D-Position nur wenn das Campfire sich bewegt
        campfireInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player != null && other.transform == player)
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (player != null && other.transform == player)
        {
            isPlayerInside = false;
        }
    }

    private void OnDestroy()
    {
        if (campfireInstance.isValid())
        {
            campfireInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            campfireInstance.release();
        }
    }
}
