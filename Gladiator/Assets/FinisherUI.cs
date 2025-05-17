using UnityEngine;

public class FinisherUI : MonoBehaviour
{
    public GameObject iconObject;
    private Enemy enemy;
    public CanvasGroup canvasGroup;
    public float blinkSpeed = 2f;

    void Start()
    {
        enemy = GetComponent<Enemy>();
    }
    void Update()
    {
        float hpPercent = enemy.GetHealthPercent(); // z. B. 5 = 5%

        if (hpPercent <= enemy.fatalFinisherThreshold)
        {
            iconObject.SetActive(true);
            BlinkIcon();
        }
        else if(hpPercent >= enemy.fatalFinisherThreshold) 
        {
            iconObject.SetActive(false);

        }

        if (Camera.main != null)
        {
            iconObject.transform.LookAt(Camera.main.transform);
        }

    }

    private void BlinkIcon()
    {
        float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
        canvasGroup.alpha = alpha;
    }
}

