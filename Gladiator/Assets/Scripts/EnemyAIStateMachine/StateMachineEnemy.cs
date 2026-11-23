
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineEnemy : MonoBehaviour
{
    [Header("References")]
    public EnemyVision vision;
    [SerializeField] private EnemyCombat combat;

    public EnemyCombat Combat
    {
        get { return combat; }
    }

    public Animator animator;
    public CharacterController controller;

    [Header("States")]
    public EnemyState initialState;
    public EnemyState chaseState;
    public EnemyState returnState;
    public EnemyState combatIdleState;

    [HideInInspector] public Transform target;
    [HideInInspector] public Vector3 startPosition;

    private EnemyState currentState;

    public EnemyState CurrentState
    {
        get { return currentState; }
    }

    private bool isRunning;
    public bool isTurning = false;

    public bool IsBrainDisabled
    {
        get;
        private set;
    }

    private float turnStartTime;
    private float maxTurnDuration = 2f;

    [HideInInspector] public bool playerRecentlySeen;

    private float playerLastSeenTime = -Mathf.Infinity;

    private Dictionary<string, float> transitionLastTriggerTimes =
        new Dictionary<string, float>();

    // Setup

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        if (vision != null)
        {
            target = vision.target;
        }

        startPosition = transform.position;

        if (initialState != null)
        {
            SetState(initialState);
        }
    }

    private void Update()
    {
        if (IsBrainDisabled)
            return;

        if (currentState == null)
            return;

        if (combat != null && combat.IsStunned)
            return;

        // Drehung absichern
        if (isTurning)
        {
            if (Time.time - turnStartTime > maxTurnDuration)
            {
                isTurning = false;
            }
        }

        currentState.Tick();
    }

    public void TransitionTo(EnemyState newState)
    {
        if (newState == null)
            return;

        if (isTurning)
            return;

        if (currentState != null)
        {
            if (currentState.name == newState.name)
                return;

            currentState.Exit();
        }

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
        string fromName = "null";
        string toName = "null";

        if (fromState != null)
        {
            fromName = fromState.name;
        }

        if (toState != null)
        {
            toName = toState.name;
        }

        string key = gameObject.GetInstanceID() + "_" + fromName + "_to_" + toName;

        return key;
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

        direction.Normalize();

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

    public void StopMovement()
    {
        if (controller != null)
        {
            controller.Move(Vector3.zero);
        }
    }

    public bool TryPlayTurnAnimation(Transform target)
    {
        if (isTurning || target == null)
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

        float directionAngle = Vector3.SignedAngle(
            transform.forward,
            direction,
            Vector3.up
        );

        bool playerIsOnRight = directionAngle > 0f;

        string animationName;

        if (angle < 67.5f)
        {
            if (playerIsOnRight)
                animationName = "Turn_Right_45";
            else
                animationName = "Turn_Left_45";
        }
        else if (angle < 112.5f)
        {
            if (playerIsOnRight)
                animationName = "Turn_Right_90";
            else
                animationName = "Turn_Left_90";
        }
        else if (angle < 157.5f)
        {
            if (playerIsOnRight)
                animationName = "Turn_Right_135";
            else
                animationName = "Turn_Left_135";
        }
        else
        {
            if (playerIsOnRight)
                animationName = "Turn_Right_180";
            else
                animationName = "Turn_Left_180";
        }

        animator.SetTrigger(animationName);

        isTurning = true;
        turnStartTime = Time.time;

        return true;
    }

    public void SetRunning(bool running)
    {
        if (isRunning == running)
            return;

        isRunning = running;

        animator.SetBool("IsRunning", running);
    }

    public void TemporarilyDisableFSM(float duration)
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(DisableFSMBrain(duration));
        }
    }

    private IEnumerator DisableFSMBrain(float duration)
    {
        IsBrainDisabled = true;

        yield return new WaitForSeconds(duration);

        IsBrainDisabled = false;
    }
    public void ResetCombatTriggers()
    {
        animator.ResetTrigger("Attack_Right");
        animator.ResetTrigger("Attack_Left");
        animator.ResetTrigger("Attack_Overhead");

        animator.ResetTrigger("BlockRight");
        animator.ResetTrigger("BlockLeft");

        animator.ResetTrigger("DodgeLeft");
        animator.ResetTrigger("DodgeRight");
        animator.ResetTrigger("DodgeBack");
    }

}
