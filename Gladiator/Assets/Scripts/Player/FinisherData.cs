using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Finisher Data")]
public class FinisherData : ScriptableObject
{
    public enum FinisherRotationMode
    {
        FaceTarget,     // Dynamisch berechnet
        CustomRotation, // Feste Rotation aus SO
        Ignore          // Keine Rotation (z. B. RootMotion-only)
    }
    public AnimationClip animation;
    public bool isFatal;
    public WeaponType weaponType;

    public FinisherRotationMode rotationMode = FinisherRotationMode.FaceTarget;

    [Tooltip("Nur verwendet bei CustomRotation")]
    public Vector3 playerRotationEuler;
    public Vector3 enemyRotationEuler;
}
