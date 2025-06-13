using UnityEngine;
using UnityEngine.UI;

public class EnergyUI : MonoBehaviour
{
    [SerializeField] private Slider energySlider;
    [SerializeField] private State playerState;
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(-1.2f, 1.5f, 0f);
    [SerializeField] private Image fillImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Color normalColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color pulseColor = Color.white;

    private float pulseTimer;
    private bool isPulsing = false;
    private float lastEnergy;
    private float fadeTimer;
    private float fadeDelay = 2.5f;   // Sekunden ohne Veränderung bis Fade-Out
    private float fadeSpeed = 2f;
    private Transform mainCamera;
    private float shakeDuration = 0.2f;
    private float shakeTimer = 0f;
    private float shakeStrength = 0.05f;

    [System.Obsolete]
    private void Start()
    {
        if (playerState == null)
            playerState = FindObjectOfType<State>();
        if (player == null)
            player = playerState.transform;
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        if (fillImage == null)
            fillImage = energySlider.fillRect.GetComponent<Image>();

        offset = transform.localPosition;

        mainCamera = Camera.main.transform;

        energySlider.maxValue = playerState.maxEnergy;
        fillImage.color = normalColor;
        lastEnergy = playerState.currentEnergy;

        // Start transparent
        canvasGroup.alpha = 0f;
    }

    private void Update()
    {
        // Position links vom Spieler
        transform.position = player.position + player.right * offset.x + Vector3.up * offset.y + player.forward * offset.z;

        // Look at camera
        Vector3 lookDir = transform.position - mainCamera.position;
        lookDir.y = 0f; // Optional: kein Neigen
        transform.rotation = Quaternion.LookRotation(lookDir);

        // Smooth slider
        float currentEnergy = playerState.currentEnergy;
        energySlider.value = Mathf.Lerp(energySlider.value, currentEnergy, Time.deltaTime * 10f);

        // Pulse wenn wenig Energie
        float percent = currentEnergy / playerState.maxEnergy;
        if (percent < 0.2f)
        {
            if (!isPulsing)
            {
                isPulsing = true;
                pulseTimer = 0f;
            }

            pulseTimer += Time.deltaTime * 5f;
            float pulse = Mathf.Abs(Mathf.Sin(pulseTimer));
            fillImage.color = Color.Lerp(normalColor, pulseColor, pulse);
        }
        else
        {
            isPulsing = false;
            fillImage.color = normalColor;
        }

        // Fading
        if (Mathf.Abs(currentEnergy - lastEnergy) > 0.01f)
        {
            fadeTimer = 0f;
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, Time.deltaTime * fadeSpeed);
        }
        else
        {
            fadeTimer += Time.deltaTime;
            if (fadeTimer > fadeDelay)
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, Time.deltaTime * fadeSpeed);
            }
        }
        // Shake Trigger
        if (currentEnergy < lastEnergy - 0.5f) // Nur bei merklichem Verbrauch
        {
            shakeTimer = shakeDuration;
        }

        // Shake Update
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            Vector3 shakeOffset = Random.insideUnitSphere * shakeStrength;
            transform.localPosition = offset + shakeOffset;
        }
        else
        {
            transform.localPosition = offset;
        }
        lastEnergy = currentEnergy;
    }



}
