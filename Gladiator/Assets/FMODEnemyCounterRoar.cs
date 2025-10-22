using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODEnemyCounterRoar : MonoBehaviour
{
   [SerializeField]
    private EventReference enemyCounterRoarEvent; 
    private EventInstance instance;

    internal void PlayEnemyCounterRoarSFX()
    {
        instance = RuntimeManager.CreateInstance(enemyCounterRoarEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        instance.start();
        instance.release();
    }
}
