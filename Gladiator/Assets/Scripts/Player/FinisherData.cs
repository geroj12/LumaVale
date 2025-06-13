using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Finisher Data")]
public class FinisherData : ScriptableObject
{
    public AnimationClip animation;
    public bool isFatal;
    public WeaponType weaponType;
}
