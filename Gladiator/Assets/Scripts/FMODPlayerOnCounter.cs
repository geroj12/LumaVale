using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODPlayerOnCounter : MonoBehaviour
{
   [SerializeField]
    private EventReference onPlayerCounterEvent; 
    private EventInstance instance;

    internal void OnPlayerCounterSFX()
    {
        instance = RuntimeManager.CreateInstance(onPlayerCounterEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        instance.start();
        instance.release();
    }

}
