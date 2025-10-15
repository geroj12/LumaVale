using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODSwordHitsBody : MonoBehaviour
{
    [SerializeField]
    private EventReference swordHitsBodyEvent; // NEU: korrektes EventReference-Feld
    private EventInstance instance;

    internal void PlaySwordHitsBodySFX()
    {
        instance = RuntimeManager.CreateInstance(swordHitsBodyEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        instance.start();
        instance.release();
    }
}
