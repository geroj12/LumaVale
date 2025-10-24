using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODFollowerFlyingIdle : MonoBehaviour
{
    [SerializeField]
    private EventReference followerFlyingIdleEvent; // NEU: korrektes EventReference-Feld
    private EventInstance instance;

    internal void PlayFollowerFlyingIdleSFX()
    {
        instance = RuntimeManager.CreateInstance(followerFlyingIdleEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        instance.start();
        instance.release();
    }
}
