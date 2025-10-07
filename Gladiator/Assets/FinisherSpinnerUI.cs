using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleSpinner
{
    [RequireComponent(typeof(Image))]
    public class FinisherSpinnerUI : MonoBehaviour
    {
        [Header("Scaling (Pulse Effect)")]
        public bool Pulse = true;

        [Range(0f, 10f), Tooltip("Wie oft pro Sekunde das UI pulsiert.")]
        public float PulseSpeed = 1f;

        [Range(0f, 1f), Tooltip("Wie stark die Skalierung variiert (0.1 = ±10%)")]
        public float PulseAmplitude = 0.2f;

        public AnimationCurve PulseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Vector3 _originalScale;
        private float _time;


        void Start()
        {
            _originalScale = transform.localScale;
        }

        void Update()
        {
            if (!Pulse) return;

            _time += Time.deltaTime * PulseSpeed;

            // Kurvenwert berechnen (0..1..0)
            float curveValue = PulseCurve.Evaluate(_time % 1f);

            // Sinuswelle oder Curve für sanftes Rein-/Rauszoomen
            float scaleOffset = Mathf.Lerp(-PulseAmplitude, PulseAmplitude, curveValue);

            transform.localScale = _originalScale * (1f + scaleOffset);
        }
        public void SetPulseSpeed(float speed)
        {
            PulseSpeed = speed;
        }

        public void ResetPulse()
        {
            transform.localScale = _originalScale;
            _time = 0f;
        }
    }
}