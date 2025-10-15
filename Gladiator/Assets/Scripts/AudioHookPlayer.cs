using UnityEngine;

public class AudioHookPlayer : MonoBehaviour
{
    [SerializeField] private FMODFootsteps fmodFootstepsAudio;
    [SerializeField] private FMODEquipUnequip fmodEquipUnequip;
    [SerializeField] private FMODSwordWoosh swordWoosh;
    [SerializeField] private FMODHurt fMODHurt;
    [SerializeField] private FMODSwordHit fMODSwordHit;
    [SerializeField] private FMODSwordHitsBody swordHitsBody;


    public void AudioOnFootStep()
    {
        fmodFootstepsAudio.PlayFootstepWalkAudio();
    }

    public void PlayEquipSound()
    {
        fmodEquipUnequip.PlayEquip();
    }

    public void PlayUnequipSound()
    {
        fmodEquipUnequip.PlayUnequip();
    }

    public void PlaySwordWooshSound()
    {
        swordWoosh.PlaySwordWoosh();
    }

    public void PlayHurtSound()
    {
        fMODHurt.PlayHurt();
    }
    public void PlayOnSwordHitImpactSound()
    {
        fMODSwordHit.PlayOnSwordHitSFX();
    }

    public void PlaySwordHitsBodySound()
    {
        swordHitsBody.PlaySwordHitsBodySFX();
    }
}
