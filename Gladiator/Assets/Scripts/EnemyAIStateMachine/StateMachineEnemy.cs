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

    public EnemyState aggressiveIdleState;

    [HideInInspector] public Transform target;
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Vector3 startPosition;

    private EnemyState currentState;

    private int patrolIndex = 0;

    public Animator animator;
    private bool isRunning;
    public bool isTurning = false;


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
    public void TemporarilyDisableFSM(float duration)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(DisableFSMCoroutine(duration));
    }

    private IEnumerator DisableFSMCoroutine(float duration)
    {
        enabled = false;
        yield return new WaitForSeconds(duration);
        enabled = true;
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
    public bool TryPlayTurnAnimation(Transform target)
    {
        if (isTurning || target == null)
            return false; // Bereits im Turn oder kein Ziel

        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0;

        if (directionToTarget.sqrMagnitude < 0.01f)
            return false; // Kein sinnvoller Richtungsvektor

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget.normalized);
        float angle = Quaternion.Angle(transform.rotation, targetRotation);

        if (angle < 40f)
            return false; // Kein Turn notwendig bei geringem Winkel

        string triggerToPlay = null;

        if (Vector3.SignedAngle(transform.forward, directionToTarget.normalized, Vector3.up) > 0f)
        {
            // Player ist rechts
            if (angle < 67.5f) triggerToPlay = "Turn_Right_45";
            else if (angle < 112.5f) triggerToPlay = "Turn_Right_90";
            else if (angle < 157.5f) triggerToPlay = "Turn_Right_135";
            else triggerToPlay = "Turn_Right_180";
        }
        else
        {
            // Player ist links
            if (angle < 67.5f) triggerToPlay = "Turn_Left_45";
            else if (angle < 112.5f) triggerToPlay = "Turn_Left_90";
            else if (angle < 157.5f) triggerToPlay = "Turn_Left_135";
            else triggerToPlay = "Turn_Left_180";
        }

        if (string.IsNullOrEmpty(triggerToPlay))
            return false;

        animator.SetTrigger(triggerToPlay);
        isTurning = true;

        return true;
    }
    public void TurnFinished()
    {
        isTurning = false;
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