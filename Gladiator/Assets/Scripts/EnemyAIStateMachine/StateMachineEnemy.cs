
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineEnemy : MonoBehaviour
{
    [Header("References")]
    public EnemyVision vision;
    [SerializeField] private EnemyCombat combat;
    public EnemyCombat Combat => combat;

    public Animator animator;
    public CharacterController controller;

    [Header("States")]
    public EnemyState initialState;
    public EnemyState chaseState;
    public EnemyState attackState;
    public EnemyState investigateState;
    public EnemyState returnState;
    public EnemyState blockState;
    public EnemyState dodgeState;
    public EnemyState combatIdleState;
    public EnemyState combatRetreatState;

    [HideInInspector] public Transform target;
    [HideInInspector] public Vector3 startPosition;

    private EnemyState currentState;
    public EnemyState CurrentState => currentState;

    private bool isRunning;
    public bool isTurning;
    public bool IsBrainDisabled { get; private set; }

    private float turnStartTime;
    private const float maxTurnDuration = 2f;

    [HideInInspector] public bool playerRecentlySeen;

    private float playerLastSeenTime = -Mathf.Infinity;

    private Dictionary<string, float> transitionLastTriggerTimes =
        new Dictionary<string, float>();

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        if (vision != null)
            target = vision.target;

        startPosition = transform.position;

        if (initialState != null)
            SetState(initialState);
    }

    private void Update()
    {
        if (IsBrainDisabled || currentState == null || combat.IsStunned)
            return;

        if (isTurning && Time.time - turnStartTime > maxTurnDuration)
            isTurning = false;

        currentState.Tick();
    }

    // Transition Cooldowns

    public bool IsTransitionCooldownActive(string key, float cooldown)
    {
        if (string.IsNullOrEmpty(key))
            return false;

        if (!transitionLastTriggerTimes.ContainsKey(key))
            return false;

        float lastTime = transitionLastTriggerTimes[key];

        return Time.time - lastTime < cooldown;
    }

    public void RegisterTransitionTrigger(string key)
    {
        if (string.IsNullOrEmpty(key))
            return;

        transitionLastTriggerTimes[key] = Time.time;
    }

    public string BuildTransitionKey(EnemyState fromState, EnemyState toState)
    {
        string fromName = fromState != null ? fromState.name : "null";
        string toName = toState != null ? toState.name : "null";

        return gameObject.GetInstanceID() + "_" + fromName + "_to_" + toName;
    }

    // State Management

    public void TransitionTo(EnemyState newState)
    {
        if (newState == null || isTurning)
            return;

        if (currentState != null && currentState.name == newState.name)
            return;

        if (currentState != null)
            currentState.Exit();

        currentState = Instantiate(newState);

        currentState.Initialize(this);
        currentState.Enter();
    }

    private void SetState(EnemyState newState)
    {
        currentState = Instantiate(newState);

        currentState.Initialize(this);
        currentState.Enter();
    }

    // Player Detection

    public bool HasRecentlySeenPlayer(float duration)
    {
        return Time.time - playerLastSeenTime <= duration;
    }

    public void NotifyPlayerSeen()
    {
        playerRecentlySeen = true;
        playerLastSeenTime = Time.time;
    }

    // Movement

    public void MoveTo(Vector3 destination, float speed)
    {
        Vector3 direction = destination - transform.position;
        direction.y = 0;

        direction = direction.normalized;

        if (direction.magnitude < 0.1f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            10f * Time.deltaTime
        );

        controller.Move(direction * speed * Time.deltaTime);
    }

    public void RetreatMove(Vector3 retreatTarget, float speed, Transform player)
    {
        Vector3 moveDirection = retreatTarget - transform.position;
        moveDirection.y = 0;

        moveDirection = moveDirection.normalized;

        controller.Move(moveDirection * speed * Time.deltaTime);

        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
        }
    }

    public void StopMovement()
    {
        controller.Move(Vector3.zero);
    }

    // Turning

    public bool TryPlayTurnAnimation(Transform target)
    {
        if (isTurning)
            return false;

        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.01f)
            return false;

        direction.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float angle = Quaternion.Angle(transform.rotation, targetRotation);

        if (angle < 40f)
            return false;

        string animationName;

        float directionAngle = Vector3.SignedAngle(
            transform.forward,
            direction,
            Vector3.up
        );

        bool playerIsOnRight = directionAngle > 0f;

        if (angle < 67.5f)
            animationName = playerIsOnRight ? "Turn_Right_45" : "Turn_Left_45";
        else if (angle < 112.5f)
            animationName = playerIsOnRight ? "Turn_Right_90" : "Turn_Left_90";
        else if (angle < 157.5f)
            animationName = playerIsOnRight ? "Turn_Right_135" : "Turn_Left_135";
        else
            animationName = playerIsOnRight ? "Turn_Right_180" : "Turn_Left_180";

        animator.SetTrigger(animationName);

        isTurning = true;
        turnStartTime = Time.time;

        return true;
    }

    public void TurnFinished()
    {
        isTurning = false;
    }

    // Animations

    public void SetRunning(bool running)
    {
        if (isRunning == running)
            return;

        isRunning = running;
        animator.SetBool("IsRunning", running);
    }

    internal void PlayIdleAnimation()
    {
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsWalking", false);
    }

    // Disable State Machine

    public void TemporarilyDisableFSM(float duration)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(DisableFSMBrain(duration));
    }

    private IEnumerator DisableFSMBrain(float duration)
    {
        IsBrainDisabled = true;

        yield return new WaitForSeconds(duration);

        IsBrainDisabled = false;
    }
}
