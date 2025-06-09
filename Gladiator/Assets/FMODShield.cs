using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODShield : MonoBehaviour
{
    [SerializeField]
    private EventReference shieldImpactEvent; // NEU: korrektes EventReference-Feld
    private EventInstance instance;
    internal void PlayShieldImpact()
    {
        instance = RuntimeManager.CreateInstance(shieldImpactEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

        instance.start();
        instance.release();
    }


}
