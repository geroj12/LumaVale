using UnityEngine;

public class AudioHookEnemy : MonoBehaviour
{
    [SerializeField] private FMODFootsteps fmodFootstepsAudio;
    [SerializeField] private FMODSwordWoosh swordWoosh;
    [SerializeField] private FMODHurt fMODHurt;
    [SerializeField] private FMODEnemyDying fMODEnemyDying;
    [SerializeField] private FMODFinisher fMODFinisher;
    [SerializeField] private FMODShield fMODShield;
    [SerializeField] private FMODSwordHitsBody swordHitsBody;
    [SerializeField] private EnemyCombat enemyCombat;

    public void AudioOnFootStep()
    {
        fmodFootstepsAudio.PlayFootstepWalkAudio();
    }
    public void PlaySwordWooshSound()
    {
        if (!enemyCombat.IsAttacking) return;

        swordWoosh.PlaySwordWoosh();

    }

    public void PlayHurtSound()
    {
        fMODHurt.PlayHurt();
    }

    public void PlayDyingSound()
    {
        fMODEnemyDying.PlayDyingSFX();
    }

    public void PlayerFinisherSound()
    {
        fMODFinisher.PlayerFinisherSFX();
    }

    public void PlayShieldImpactSound()
    {
        fMODShield.PlayShieldImpact();
    }

    public void PlaySwordHitsBodySound()
    {
        swordHitsBody.PlaySwordHitsBodySFX();
    }
}
