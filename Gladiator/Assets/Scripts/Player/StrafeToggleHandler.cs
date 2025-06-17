using UnityEngine;
using UnityEngine.UI;

public class StrafeToggleHandler : MonoBehaviour
{
    public Image strafeHoldImage;
    [SerializeField] private float holdDuration = 1.5f;

    private float holdTimer = 0f;
    private bool toggled = false;
    private State state;
    private Animator anim;

    void Start()
    {
        state = GetComponent<State>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.V))
        {
            holdTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(holdTimer / holdDuration);

            if (strafeHoldImage) strafeHoldImage.fillAmount = progress;

            if (!toggled && holdTimer >= holdDuration)
            {
                state.Strafe = !state.Strafe;
                anim.SetBool("Strafe", state.Strafe);
                toggled = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.V))
        {
            holdTimer = 0f;
            toggled = false;
            if (strafeHoldImage) strafeHoldImage.fillAmount = 0f;
        }
    }
}
