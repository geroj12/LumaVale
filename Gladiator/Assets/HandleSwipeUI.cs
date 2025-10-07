using Assets.SimpleSpinner;
using UnityEngine;
using UnityEngine.UI;

public class HandleSwipeUI : MonoBehaviour
{
    [SerializeField] private State playerState;
    [SerializeField] private Combat combat;

    [SerializeField] private GameObject leftSwipeUI;
    [SerializeField] private GameObject rightSwipeUI;
    [SerializeField] private GameObject blockUI;
    [SerializeField] private GameObject holdAttackUI;


    private RotateUI spinner;

    [SerializeField] private HoldAttack_ChangeColor changeColorOnAttackHolding;
    [SerializeField] private ParticleSystem overloadAttackVFX;
    private bool vfxPlayed = false;

    void Start()
    {
        if (holdAttackUI != null)
            spinner = holdAttackUI.GetComponent<RotateUI>();
    }
    void Update()
    {

        blockUI.SetActive(playerState.blocking);


        leftSwipeUI.SetActive(playerState.mouseOnLeftSide);
        rightSwipeUI.SetActive(playerState.mouseOnRightSide);


        // Hold-Attack-UI
        if (playerState.holdingAttack)
        {
            if (combat.isOvercharged)
            {
                holdAttackUI.SetActive(false);

                if (!vfxPlayed && overloadAttackVFX != null)
                {
                    overloadAttackVFX.Play();
                    vfxPlayed = true;
                }

                return; 
            }

            holdAttackUI.SetActive(true);

            float t = Mathf.Clamp01(combat.holdAttackTimer / combat.maxHoldTime);

            if (spinner != null)
            {
                float speed = Mathf.Lerp(0.5f, 1f, t);
                spinner.SetRotationSpeed(speed);
            }

            // Farbe anpassen
            if (changeColorOnAttackHolding != null)
                changeColorOnAttackHolding.UpdateColor(t);
        }
        else
        {
            holdAttackUI.SetActive(false);

            if (spinner != null)
                spinner.SetRotationSpeed(0.5f);

            if (overloadAttackVFX != null && overloadAttackVFX.isPlaying)
                overloadAttackVFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            vfxPlayed = false;
        }


    }
}
