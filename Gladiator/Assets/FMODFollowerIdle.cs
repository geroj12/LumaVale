using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODFollowerIdle : MonoBehaviour
{
    [SerializeField]
    private EventReference followerIdleEvent; // NEU: korrektes EventReference-Feld
    private EventInstance instance;

    internal void PlayFollowerIdleSFX()
    {
        instance = RuntimeManager.CreateInstance(followerIdleEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        instance.start();
        instance.release();
    }
}
