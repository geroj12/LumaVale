using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODEnemyTelegraphRoar : MonoBehaviour
{
   [SerializeField]
    private EventReference enemyTelegraphRoarEvent; 
    private EventInstance instance;

    internal void PlayEnemyTelegraphRoarSFX()
    {
        instance = RuntimeManager.CreateInstance(enemyTelegraphRoarEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        instance.start();
        instance.release();
    }
}
