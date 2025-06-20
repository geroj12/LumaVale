using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODFinisher : MonoBehaviour
{
    [SerializeField]
    private EventReference finisherEvent; // NEU: korrektes EventReference-Feld
    private EventInstance instance;


    internal void PlayFinisher()
    {
        instance = RuntimeManager.CreateInstance(finisherEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

        instance.start();
        instance.release();
    }
}
