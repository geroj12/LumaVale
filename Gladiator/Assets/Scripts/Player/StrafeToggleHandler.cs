using UnityEngine;

public class StrafeToggleHandler : MonoBehaviour
{
    
    private State state;
    private Animator anim;

    void Start()
    {
        state = GetComponent<State>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (state.equipped)
        {
            anim.SetBool("Strafe", state.Strafe = true);
        }
        else if (!state.equipped)
        {
            anim.SetBool("Strafe", state.Strafe = false);
        }
       
    }
}
