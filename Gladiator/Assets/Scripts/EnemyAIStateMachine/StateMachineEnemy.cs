using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class StateMachineEnemy : MonoBehaviour
{
    public EnemyVision vision;
    public Transform[] patrolPoints;

    public EnemyState initialState;
    public EnemyState patrolState;
    public EnemyState chaseState;
    public EnemyState attackState;
    public EnemyState investigateState;
    public EnemyState returnState;
    public EnemyState blockState;
    public EnemyState dodgeState;
    public EnemyState combatIdleState;
    public EnemyState combatRetreatState;

    [HideInInspector] public Transform target;
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Vector3 startPosition;

    private EnemyState currentState;
    private int patrolIndex = 0;

    public Animator animator;
    private bool isRunning;

    
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        target = vision.target;
        startPosition = transform.position;

        TransitionTo(initialState);
    }

    private void Update()
    {
        currentState?.Tick(this);
    }

    public void TransitionTo(EnemyState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }

    public void MoveTo(Vector3 destination, float speed)
    {
        Vector3 direction = (destination - transform.position).normalized;
        direction.y = 0;

        if (direction.magnitude > 0.1f)
        {
            Quaternion rot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
            controller.Move(direction * speed * Time.deltaTime);
        }
    }
    public void RetreatMove(Vector3 retreatTarget, float speed, Transform player)
    {
        // Bewegung: Vom Ziel weg
        Vector3 moveDir = (retreatTarget - transform.position).normalized;
        moveDir.y = 0;
        controller.Move(moveDir * speed * Time.deltaTime);

        // Rotation: Zum Spieler schauen
        Vector3 lookDir = (player.position - transform.position).normalized;
        lookDir.y = 0;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }
    }
    public void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        if (direction.magnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 10f * Time.deltaTime);
        }
    }
    public void SetRunning(bool running)
    {
        if (isRunning == running) return;
        isRunning = running;
        animator.SetBool("IsRunning", running);
    }
    public void StopMovement()
    {
        controller.Move(Vector3.zero); // verhindert Restbewegung
    }


    public Transform GetNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return null;
        Transform point = patrolPoints[patrolIndex];
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        return point;
    }
}