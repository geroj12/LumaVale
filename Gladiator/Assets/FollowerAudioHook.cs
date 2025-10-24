using UnityEngine;

public class FollowerAudioHook : MonoBehaviour
{
    [SerializeField] private FMODFollowerFlyingIdle followerFlyingIdle;
    [SerializeField] private FMODFollowerIdle followerIdle;

    public void PlayAudioFollowerFlyingIdle()
    {
        followerFlyingIdle.PlayFollowerFlyingIdleSFX();
    }
    public void PlayAudioFollowerIdle()
    {
        followerIdle.PlayFollowerIdleSFX();
    }
}
