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
        instance = RuntimeManager.CreateInstance(musicEventReference); // EventReference ziehst du im Inspector rein
        instance.start();


        instance.setParameterByNameWithLabel("MusicState", "Main");
    }

     public void SetMusicState(string label)
    {
        if (instance.isValid())
        {
            instance.setParameterByNameWithLabel("MusicState", label);
        }
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
