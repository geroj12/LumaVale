using System.Collections;
using UnityEngine;

public class WeaponHolster : MonoBehaviour
{
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
        EquipANDUnequip();
    }
    void EquipANDUnequip()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !state.equipped)
        {
            anim.SetBool("Equip", true);
            state.equipped = true;
            StartCoroutine(EquipTimer());
        }
        else if (Input.GetKeyDown(KeyCode.Q) && state.equipped)
        {
            anim.SetBool("Unequip", true);
            StartCoroutine(UnequipTimer());
        }
    }

    public void AttachSwordToHand()
    {
        sword.transform.SetParent(rightHandTransform);
        sword.transform.localPosition = handPositionOffset;
        sword.transform.localRotation = Quaternion.Euler(handRotationOffset);
    }

    public void AttachSwordToHolster()
    {
        sword.transform.SetParent(holsterTransform);
        sword.transform.localPosition = Vector3.zero;
        sword.transform.localRotation = Quaternion.identity;
    }
    IEnumerator EquipTimer()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("Equip", false);
        state.equipped = true;
    }

    IEnumerator UnequipTimer()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("Unequip", false);
        state.equipped = false;
    }
}
