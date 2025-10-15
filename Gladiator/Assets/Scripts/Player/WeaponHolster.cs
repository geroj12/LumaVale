using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponHolster : MonoBehaviour
{
    public delegate void WeaponChangedDelegate(PlayerWeapon newDamage);
    public event WeaponChangedDelegate OnWeaponChanged;
    public enum WeaponState { Idle, Equipping, Equipped, Unequipping }
    [SerializeField] private FinisherController finisherController;
    public WeaponState currentWeaponState = WeaponState.Idle;
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject twoHandedWeapon;
    public Combat combatScript;
    State state;
    [SerializeField] private Animator anim;

    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private Transform holsterTransform;

    [SerializeField] private Transform holsterTransformTwoHandedSword;

    public Vector3 handPositionOffsetSword;
    public Vector3 handRotationOffset;

    public Vector3 handPositionOffsetTwoHandedSword;
    public Vector3 handRotationOffsetTwoHandedSword;

    public int currentWeaponType = 0; // 0 = unbewaffnet, 1 = Schwert, 2 = Zweihänder usw.

    void Start()
    {
        state = GetComponent<State>();
    }
    void Update()
    {
        HandleEquipInput();
    }
    void LateUpdate()
    {

    }
    void HandleEquipInput()
    {
        if (currentWeaponState != WeaponState.Idle && currentWeaponState != WeaponState.Equipped) return;

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

    // Wird durch Animation Event aufgerufen
    public void AttachCurrentWeaponToHand()
    {
        if (currentWeaponType == 1)
        {
            sword.transform.SetParent(rightHandTransform);
            sword.transform.localPosition = handPositionOffsetSword;
            sword.transform.localRotation = Quaternion.Euler(handRotationOffset);
        }
        else if (currentWeaponType == 2)
        {
            twoHandedWeapon.transform.SetParent(rightHandTransform);
            twoHandedWeapon.transform.localPosition = handPositionOffsetTwoHandedSword;
            twoHandedWeapon.transform.localRotation = Quaternion.Euler(handRotationOffsetTwoHandedSword);
        }
    }

    // Wird durch Animation Event aufgerufen
    public void AttachCurrentWeaponToHolster()
    {
        if (currentWeaponType == 1)
        {
            sword.transform.SetParent(holsterTransform);
            sword.transform.localPosition = Vector3.zero;
            sword.transform.localRotation = Quaternion.identity;
        }
        else if (currentWeaponType == 2)
        {
            twoHandedWeapon.transform.SetParent(holsterTransformTwoHandedSword);
            twoHandedWeapon.transform.localPosition = Vector3.zero;
            twoHandedWeapon.transform.localRotation = Quaternion.identity;
        }
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

        // 🟡 WeaponType VOR Animation setzen!
        currentWeaponType = weaponType;
        anim.SetInteger("WeaponType", weaponType);

        anim.SetBool("Equip", true);
        yield return new WaitForSeconds(1f); // Wartezeit abhängig von Animation
        anim.SetBool("Equip", false);

        state.equipped = true;
        currentWeaponState = WeaponState.Equipped;
        NotifyWeaponChanged();
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
        NotifyWeaponChanged();
    }

    private void NotifyWeaponChanged()
    {
        PlayerWeapon newDamage = GetCurrentWeaponDamage();
        combatScript.UpdateWeaponDamage(newDamage);
        OnWeaponChanged?.Invoke(newDamage);
    }
    public bool IsBusy()
    {
        return currentWeaponState == WeaponState.Equipping || currentWeaponState == WeaponState.Unequipping;
    }
    public PlayerWeapon GetCurrentWeaponDamage()
    {
        GameObject activeWeapon = null;

        switch (currentWeaponType)
        {
            case 1:
                activeWeapon = sword;
                break;
            case 2:
                activeWeapon = twoHandedWeapon;
                break;
        }

        if (activeWeapon != null)
            return activeWeapon.GetComponentInChildren<PlayerWeapon>();

        return null;
    }
}
