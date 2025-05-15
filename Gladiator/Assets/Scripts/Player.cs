using UnityEngine;

public class Player : MonoBehaviour
{
    private Movement movement;
    [SerializeField] private Animator animator;


    void Start()
    {
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        movement.HandleMovement();
        if (Input.GetKeyDown(KeyCode.F))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1f))
            {   
                var enemy = hit.collider.GetComponent<Enemy>();
                Debug.Log(hit.collider);
                if (enemy != null && enemy.IsFinisherReady)
                {
                    enemy.StartFinisher();
                    animator.SetTrigger("FinisherLongsword01"); // Spieleranimation
                }
            }
        }
    }

}
