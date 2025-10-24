using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CounterUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyCounterWindow counterWindow;
    [SerializeField] private Image telegraphIcon;

    [Header("Visual Settings")]
    [SerializeField] private Color activeColor = Color.red;
    [SerializeField] private float fadeInTime = 0.2f;
    [SerializeField] private float fadeOutTime = 0.3f;

    private Coroutine fadeRoutine;
    private bool isVisible = false;

    private Transform camTransform;

    private void Start()
    {
        if (Camera.main != null)
            camTransform = Camera.main.transform;

        if (counterWindow == null)
            counterWindow = GetComponentInParent<EnemyCounterWindow>();

        if (counterWindow != null)
        {
            counterWindow.OnCounterWindowOpened += ShowTelegraph;
            counterWindow.OnCounterWindowClosed += HideTelegraph;
            counterWindow.OnCounterTriggered += HideTelegraph;
        }

        HideImmediate();
    }

    private void LateUpdate()
    {
        // UI immer zur Kamera ausrichten (falls über dem Gegner)
        if (camTransform != null)
            transform.LookAt(transform.position + camTransform.forward);
    }

    private void ShowTelegraph()
    {
        if (isVisible) return;
        isVisible = true;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeIcon(0f, 1f, fadeInTime, activeColor));
    }

    private void HideTelegraph()
    {
        if (!isVisible) return;
        isVisible = false;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeIcon(1f, 0f, fadeOutTime, activeColor, deactivateAtEnd: true));
    }

    private IEnumerator FadeIcon(float startAlpha, float endAlpha, float duration, Color baseColor, bool deactivateAtEnd = false)
    {
        telegraphIcon.gameObject.SetActive(true);
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t / duration);
            telegraphIcon.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }

        telegraphIcon.color = new Color(baseColor.r, baseColor.g, baseColor.b, endAlpha);

        if (deactivateAtEnd)
            telegraphIcon.gameObject.SetActive(false);
    }

    private void HideImmediate()
    {
        if (telegraphIcon != null)
            telegraphIcon.gameObject.SetActive(false);

        isVisible = false;
    }
}
