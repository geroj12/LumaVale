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

        if (Input.GetKeyDown(KeyCode.Q) && currentWeaponState == WeaponState.Idle)
        {
            StartCoroutine(EquipRoutine());
        }
        else if (Input.GetKeyDown(KeyCode.Q) && currentWeaponState == WeaponState.Equipped)
        {
            StartCoroutine(UnequipRoutine());
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
    IEnumerator EquipRoutine()
    {
        currentWeaponState = WeaponState.Equipping;
        anim.SetBool("Equip", true);

        yield return new WaitForSeconds(1f); // warte auf Equip-Animation
        anim.SetBool("Equip", false);

        state.equipped = true;
        currentWeaponState = WeaponState.Equipped;
    }

    IEnumerator UnequipRoutine()
    {
        currentWeaponState = WeaponState.Unequipping;
        anim.SetBool("Unequip", true);

        yield return new WaitForSeconds(1f); // warte auf Unequip-Animation
        anim.SetBool("Unequip", false);

        state.equipped = false;
        currentWeaponState = WeaponState.Idle;
    }
}
