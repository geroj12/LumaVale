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

    [HideInInspector] public Transform target;
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Vector3 startPosition;

    private EnemyState currentState;
    private int patrolIndex = 0;

    public Animator animator;

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
    public Transform GetNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return null;
        Transform point = patrolPoints[patrolIndex];
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        return point;
    }
}