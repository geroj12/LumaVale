using UnityEngine;

public class FinisherUI : MonoBehaviour
{
    public GameObject iconObject;
    private Enemy enemy;
    private Transform camTransform;
    void Start()
    {
        enemy = GetComponent<Enemy>();
        if (Camera.main != null)
            camTransform = Camera.main.transform;
    }
    void Update()
    {
        if (enemy.isDead)
        {
            iconObject.SetActive(false);
            return;
        }
        float hpPercent = enemy.GetHealthPercent(); // z. B. 5 = 5%

        if (hpPercent <= enemy.fatalFinisherThreshold)
        {
            iconObject.SetActive(true);
        }
        else if (hpPercent >= enemy.fatalFinisherThreshold)
        {
            iconObject.SetActive(false);

        }


        if (camTransform != null)
            iconObject.transform.LookAt(camTransform);

    }

   
}

