using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODMusic : MonoBehaviour
{
    private EventInstance instance;
    [SerializeField]
    private EventReference musicEventReference; // NEU: korrektes EventReference-Feld
    void Start()
    {
        instance = RuntimeManager.CreateInstance(musicEventReference);
        instance.start();
        instance.release(); // autom. Freigabe nach Stop
    }


    public float GetCombatLevel()
    {
        float value;
        instance.getParameterByName("Combat", out value);
        return value;
    }
    public void SetCombatLevel(float value)
    {
        instance.setParameterByName("Combat", Mathf.Clamp01(value));
    }
    private void OnDestroy()
    {
        if (instance.isValid())
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            instance.release();
        }
    }
}
