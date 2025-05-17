using UnityEngine;

public class FinisherController : MonoBehaviour
{
    public Animator playerAnimator;
    public string[] fatalFinishers = { "Fatal1", "Fatal2" };
    public string[] nonFatalFinishers = { "NonFatal1", "NonFatal2" };

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1.3f))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                Debug.Log(hit.collider);
                TryStartFinisher(enemy);

            }
        }
    }
    public void TryStartFinisher(Enemy enemy)
    {

        string chosenFinisher = ChooseFinisher(enemy);
        playerAnimator.SetTrigger(chosenFinisher);

        // Finisher starten
        enemy.StartFinisher(chosenFinisher);
        // Richte den Spieler zum Gegner aus
        Vector3 toEnemy = (enemy.transform.position - transform.position).normalized;
        toEnemy.y = 0;
        transform.forward = toEnemy;

    }

    private string ChooseFinisher(Enemy enemy)
    {
        float hpPercent = enemy.GetHealthPercent(); // z. B. 5 = 5%

        if (hpPercent <= enemy.fatalFinisherThreshold)
        {
            return fatalFinishers[Random.Range(0, fatalFinishers.Length)];
        }
        else
        {
            return nonFatalFinishers[Random.Range(0, nonFatalFinishers.Length)];
        }
    }
}
