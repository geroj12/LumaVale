using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODEquipUnequip : MonoBehaviour
{
    [SerializeField]
    private EventReference equipUnequipEvent; // NEU: korrektes EventReference-Feld
    WeaponHolster weaponHolster;
    private EventInstance instance;



    void Start()
    {
        weaponHolster = GetComponent<WeaponHolster>();
        instance = RuntimeManager.CreateInstance(equipUnequipEvent);
    }



    internal void PlayEquip()
    {
        instance.setParameterByName("Equiping", 1f);
        instance.start();

    }

    internal void PlayUnequip()
    {
        instance.setParameterByName("Unequiping", 1f);
        instance.start();

    }

    private void OnDestroy()
    {
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
    }
}
