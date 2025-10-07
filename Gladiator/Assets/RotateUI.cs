using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleSpinner
{
    [RequireComponent(typeof(Image))]
    public class RotateUI : MonoBehaviour
    {
        [Header("Rotation")]
        public bool Rotation = true;
        [Range(-10, 10), Tooltip("Value in Hz (revolutions per second).")]
        public float RotationSpeed = 1;
        public AnimationCurve RotationAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        private float _angleAccumulator = 0f; 

        public void Update()
        {
            float deltaTime = Time.deltaTime;

            if (Rotation)
            {

                _angleAccumulator += RotationSpeed * 360f * deltaTime;
                float curveMultiplier = RotationAnimationCurve.Evaluate(_angleAccumulator / 360f % 1f);
                transform.localEulerAngles = new Vector3(0, 0, -_angleAccumulator * curveMultiplier);
            }
        }

        public void SetRotationSpeed(float speed)
        {
            RotationSpeed = speed;
        }
    }
}