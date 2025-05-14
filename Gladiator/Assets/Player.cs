using UnityEngine;

public class Player : MonoBehaviour
{
    private Movement movement;



    void Start()
    {
        movement = GetComponent<Movement>();

    }

    void Update()
    {

        movement.HandleMovement();

    }

}
