using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODAlert : MonoBehaviour
{
    [SerializeField]
    private EventReference alertEvent; // NEU: korrektes EventReference-Feld
    private EventInstance instance;

  
    internal void PlayAlertSound()
    {
        instance = RuntimeManager.CreateInstance(alertEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        instance.start();
        instance.release();

    }
}
