using UnityEngine;

public class AudioHook : MonoBehaviour
{
    [SerializeField] private FMODFootsteps fmodFootstepsAudio;

    public void AudioOnFootStep()
    {
        fmodFootstepsAudio.PlayFootstepWalkAudio();
    }
}
