using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODEquipUnequip : MonoBehaviour
{
    [SerializeField]
    private EventReference equipUnequipEvent; // NEU: korrektes EventReference-Feld
    [SerializeField] WeaponHolster weaponHolster;
    private EventInstance instance;



    void Start()
    {
        weaponHolster = GetComponent<WeaponHolster>();
        instance = RuntimeManager.CreateInstance(equipUnequipEvent);
    }

    void Update()
    {
        if (weaponHolster.currentWeaponState == WeaponHolster.WeaponState.Equipping)
        {
            PlayEquip();
        }
        else if (weaponHolster.currentWeaponState == WeaponHolster.WeaponState.Unequipping)
        {
            PlayUnequip();
        }
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
