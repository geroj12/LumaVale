using UnityEngine;

public class FinisherUI : MonoBehaviour
{
    public GameObject iconObject;
    private Enemy enemyHealth;
    public CanvasGroup canvasGroup;
    public float blinkSpeed = 2f;

    void Start()
    {
        enemyHealth = GetComponent<Enemy>();
    }
    void Update()
    {
        float hpPercent = enemyHealth.GetHealthPercent(); // z. B. 5 = 5%

        if (hpPercent <= enemyHealth.fatalFinisherThreshold)
        {
            iconObject.SetActive(true);
            BlinkIcon();
        }

        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0); // Falls rückwärts
        }

    }

    private void BlinkIcon()
    {
        float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
        canvasGroup.alpha = alpha;
    }
}

