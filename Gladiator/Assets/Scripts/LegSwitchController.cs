using UnityEngine;

public class LegSwitchController : MonoBehaviour
{
    private Animator anim;
    private float legSwitch;
    private Transform rightFoot;
    private Transform leftFoot;
    private Movement movement;

    void Start()
    {
        anim = GetComponent<Animator>();
        rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        movement = GetComponent<Movement>();
    }

    void Update()
    {
        if (rightFoot == null || leftFoot == null) return;

        bool rightUp = rightFoot.position.y > leftFoot.position.y;
        anim.SetBool("IsRightLegUp", rightUp);

        if (movement.inputMagnitude > 0.1f)
        {
            float target = rightUp ? 1f : 0f;
            float legDistance = Mathf.Abs(rightFoot.position.x - leftFoot.position.x);
            if (legDistance > 0.1f)
            {
                legSwitch = Mathf.MoveTowards(legSwitch, target, Time.deltaTime * 5f);
            }
        }

        anim.SetFloat("LegSwitch", legSwitch);
    }
}
