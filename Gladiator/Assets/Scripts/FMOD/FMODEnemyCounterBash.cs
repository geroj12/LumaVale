using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODEnemyCounterBash : MonoBehaviour
{
    [SerializeField]
    private EventReference counterBashEvent; 
    private EventInstance instance;

    internal void PlayCounterBashSFX()
    {
        instance = RuntimeManager.CreateInstance(counterBashEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        instance.start();
        instance.release();
    }

}
