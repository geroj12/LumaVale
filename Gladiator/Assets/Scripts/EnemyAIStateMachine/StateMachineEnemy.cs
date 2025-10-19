using System.Collections;
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
    private bool isRunning;
    public bool isTurning = false;
    public bool IsBrainDisabled { get; private set; }

    private float turnStartTime;
    private const float maxTurnDuration = 2f;

    [HideInInspector] public bool playerRecentlySeen = false;
    private float playerLastSeenTime = -Mathf.Infinity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        if (vision != null)
            target = vision.target;

        startPosition = transform.position;

        if (initialState != null)
        {
            SetState(initialState);
        }
        else
        {
            Debug.LogError("[StateMachineEnemy] Kein Initial State gesetzt!");
        }
    }

    private void Update()
    {
        if (IsBrainDisabled || currentState == null)
            return;

        // failsafe für Drehungen
        if (isTurning && Time.time - turnStartTime > maxTurnDuration)
            isTurning = false;

        currentState.Tick();
    }

    public void TransitionTo(EnemyState newState)
    {
        if (newState == null)
        {
            Debug.LogWarning("[StateMachineEnemy] Target state is null!");
            return;
        }

        if (currentState != null && currentState.name == newState.name)
            return; // keine Selbsttransition

        if (isTurning)
            return; // blockiere Wechsel während Drehung

        Debug.Log($"[FSM] Transition: {name} -> {newState.name}");

        currentState?.Exit();

        // Wichtig: ScriptableObject instanziieren, sonst shared data!
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
    public bool HasRecentlySeenPlayer(float duration)
    {
        return Time.time - playerLastSeenTime <= duration;
    }
    public void NotifyPlayerSeen()
    {
        playerRecentlySeen = true;
        playerLastSeenTime = Time.time;
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
            StartCoroutine(DisableFSMBrain(duration));
    }
    private IEnumerator DisableFSMBrain(float duration)
    {
        IsBrainDisabled = true;
        yield return new WaitForSeconds(duration);
        IsBrainDisabled = false;
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
        if (isTurning) return false;


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
        turnStartTime = Time.time;
        return true;
    }
    public void TurnFinished() => isTurning = false;

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

    internal void PlayIdleAnimation()
    {
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsWalking", false);

    }
}