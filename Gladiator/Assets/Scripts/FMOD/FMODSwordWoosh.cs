using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODSwordWoosh : MonoBehaviour
{
    [SerializeField]
    private EventReference swordWooshEvent; // NEU: korrektes EventReference-Feld
    private EventInstance instance;

    [SerializeField] private float moanVolume;

    internal void PlaySwordWoosh()
    {
        instance = RuntimeManager.CreateInstance(swordWooshEvent);
        instance.setParameterByName("MoanVolume", moanVolume);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

        instance.start();
        instance.release();
    }
}
