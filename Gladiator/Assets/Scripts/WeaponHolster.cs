using System.Collections;
using UnityEngine;

public class WeaponHolster : MonoBehaviour
{
    public enum WeaponState { Idle, Equipping, Equipped, Unequipping }
    public WeaponState currentWeaponState = WeaponState.Idle;
    [SerializeField] private GameObject sword;
    State state;
    private Animator anim;

    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private Transform holsterTransform;


    public Vector3 handPositionOffset;
    public Vector3 handRotationOffset;

    int currentWeaponType = 0; // 0 = unbewaffnet, 1 = Schwert, 2 = Zweihänder usw.

    void Start()
    {
        anim = GetComponent<Animator>();
        state = GetComponent<State>();

    }
    void LateUpdate()
    {
        HandleEquipInput();
    }
    void HandleEquipInput()
    {
        if (currentWeaponState != WeaponState.Idle && currentWeaponState != WeaponState.Equipped)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentWeaponType == 1)
                StartCoroutine(UnequipRoutine());
            else
                StartCoroutine(SwitchWeapon(1));
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentWeaponType == 2)
                StartCoroutine(UnequipRoutine());
            else
                StartCoroutine(SwitchWeapon(2));
        }
    }

    //Über Animation Events aufgerufen
    public void AttachSwordToHand()
    {
        sword.transform.SetParent(rightHandTransform);
        sword.transform.localPosition = handPositionOffset;
        sword.transform.localRotation = Quaternion.Euler(handRotationOffset);
    }
    //Über Animation Events aufgerufen
    public void AttachSwordToHolster()
    {
        sword.transform.SetParent(holsterTransform);
        sword.transform.localPosition = Vector3.zero;
        sword.transform.localRotation = Quaternion.identity;
    }

    IEnumerator SwitchWeapon(int newWeaponType)
    {
        if (state.equipped)
        {
            yield return StartCoroutine(UnequipRoutine());
            yield return new WaitForSeconds(0.1f); // kleine Pufferzeit (optional)
        }

        yield return StartCoroutine(EquipRoutine(newWeaponType));
    }
    IEnumerator EquipRoutine(int weaponType)
    {
        if (currentWeaponState == WeaponState.Equipping)
            yield break;

        currentWeaponState = WeaponState.Equipping;

        anim.SetBool("Equip", true);
        yield return new WaitForSeconds(1f); // warte auf Equip-Animation
        anim.SetBool("Equip", false);


        currentWeaponType = weaponType;
        anim.SetInteger("WeaponType", weaponType);
        // Sofort Animation aktualisieren, falls im Strafe-Zustand
        
        state.equipped = true;
        currentWeaponState = WeaponState.Equipped;
    }

    IEnumerator UnequipRoutine()
    {
        if (currentWeaponState == WeaponState.Unequipping || !state.equipped)
            yield break;

        currentWeaponState = WeaponState.Unequipping;

        anim.SetBool("Unequip", true);
        yield return new WaitForSeconds(1f); // warte auf Unequip-Animation
        anim.SetBool("Unequip", false);

        anim.SetInteger("WeaponType", 0); // auf unbewaffnet setzen

        state.equipped = false;
        currentWeaponType = 0;
        currentWeaponState = WeaponState.Idle;
    }
}
