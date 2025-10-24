using UnityEngine;

public class FollowerAnimator : MonoBehaviour
{
    private Animator animator;
    private static readonly int IsMoving = Animator.StringToHash("isFlying");

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMoving(bool moving)
    {
        animator.SetBool(IsMoving, moving);
    }
}
