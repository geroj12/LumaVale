using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODHurt : MonoBehaviour
{
    [SerializeField]
    private EventReference hurtEvent; // NEU: korrektes EventReference-Feld
    private EventInstance instance;

    internal void PlayHurt()
    {
        instance = RuntimeManager.CreateInstance(hurtEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        instance.start();
        instance.release();
    }


}
