using UnityEngine;

public class FinisherUI : MonoBehaviour
{
    public GameObject iconObject;
    [SerializeField]private EnemyHealth enemyHealth;
    private Transform camTransform;
    void Start()
    {
        if (Camera.main != null)
            camTransform = Camera.main.transform;
    }
    void Update()
    {
        if (enemyHealth.IsDead)
        {
            iconObject.SetActive(false);
            return;
        }
        float hpPercent = enemyHealth.HealthPercent; // z. B. 5 = 5%

        if (hpPercent <= enemyHealth.fatalFinisherThreshold)
        {
            iconObject.SetActive(true);
        }
        else if (hpPercent >= enemyHealth.fatalFinisherThreshold)
        {
            iconObject.SetActive(false);

        }


        if (camTransform != null)
            iconObject.transform.LookAt(camTransform);

    }

   
}

