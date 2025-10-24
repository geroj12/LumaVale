using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODSwordHit : MonoBehaviour
{
    [SerializeField]
    private EventReference swordHitSFX; // NEU: korrektes EventReference-Feld
    private EventInstance instance;

    internal void PlayOnSwordHitSFX()
    {
        instance = RuntimeManager.CreateInstance(swordHitSFX);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        instance.start();
        instance.release();
    }

}
