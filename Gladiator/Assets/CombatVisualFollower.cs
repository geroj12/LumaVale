using UnityEngine;
using System.Collections;
using FMODUnity;
using FMOD.Studio;

public class CombatVisualFollower : MonoBehaviour
{
    public Transform anchorTransform;
    public Vector3 targetLocalOffset = new Vector3(0.5f, 1.5f, 0f);
    public float followSpeed = 10f;
    public float swipeOffsetDistance = 1.2f;
    public float swipeReturnDelay = 0.3f;

    public float weaponRange = 2f;
    public LayerMask enemyLayer;

    private bool facingRight = true;
    private Vector3 currentTargetPosition;
    private Vector3 baseOffset;
    private bool isInSwipeMotion = false;

    private Animator animator;

    [Header("VFX Prefabs")]
    public GameObject chargeEffectPrefab;
    public GameObject rangeGlowEffectPrefab;

    private GameObject currentChargeEffect;
    private GameObject currentRangeGlow;

    [Header("Sounds")]
    [SerializeField] private EventReference chargeSound;
    [SerializeField] private EventReference hypedSound;

    private EventInstance instance;
    private EventInstance swipeSoundInstance;

    void Start()
    {
        baseOffset = targetLocalOffset;
        animator = GetComponentInChildren<Animator>();

    }

    void Update()
    {
        if (anchorTransform == null) return;

        // Folgebewegung
        Vector3 offset = facingRight ? baseOffset : new Vector3(-baseOffset.x, baseOffset.y, baseOffset.z);
        currentTargetPosition = anchorTransform.position + anchorTransform.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, currentTargetPosition, Time.deltaTime * followSpeed);

        // Rotation
        Vector3 toAnchor = (anchorTransform.position - transform.position).normalized;
        if (toAnchor != Vector3.zero)
        {
            Quaternion targetRot = anchorTransform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 8f);
        }

        CheckEnemyInRange();
    }

    public void SetFacingDirection(bool right)
    {
        facingRight = right;
    }

    public void ReactToSwipe(Vector2 swipeDirection)
    {
        if (isInSwipeMotion) return;

        isInSwipeMotion = true;

        if (animator != null)
            animator.SetBool("isFlying", true);

        Vector3 worldSwipe = new Vector3(swipeDirection.x, 0f, swipeDirection.y).normalized;
        Vector3 newOffset = baseOffset + worldSwipe * swipeOffsetDistance;

        // Swipe Sound abspielen
        if (!hypedSound.IsNull)
        {
            swipeSoundInstance = RuntimeManager.CreateInstance(hypedSound);
            swipeSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            swipeSoundInstance.start();
            swipeSoundInstance.release(); // wichtig, damit FMOD ihn automatisch bereinigt nach dem Abspielen
        }

        StopAllCoroutines();
        StartCoroutine(SwipeMotionCoroutine(newOffset));
    }

    private IEnumerator SwipeMotionCoroutine(Vector3 swipeTargetOffset)
    {
        Vector3 originalOffset = baseOffset;
        baseOffset = swipeTargetOffset;

        yield return new WaitForSeconds(swipeReturnDelay);

        baseOffset = originalOffset;
        isInSwipeMotion = false;

        if (animator != null)
            animator.SetBool("isFlying", false);
    }

    public void BeginCharge()
    {
        if (chargeEffectPrefab != null && currentChargeEffect == null)
            currentChargeEffect = Instantiate(chargeEffectPrefab, transform);


        if (chargeSound.IsNull) return;

        instance = RuntimeManager.CreateInstance(chargeSound);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

        // Länge ermitteln & zufällige Startzeit
        instance.getDescription(out var desc);
        desc.getLength(out int lengthMs);
        int randomMs = UnityEngine.Random.Range(300, lengthMs - 200);
        instance.setTimelinePosition(randomMs);

        instance.start();
    }

    public void EndCharge()
    {
        if (currentChargeEffect != null)
        {
            Destroy(currentChargeEffect);
            currentChargeEffect = null;
        }
        if (instance.isValid())
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); // oder IMMEDIATE
            instance.release();
            instance.clearHandle();
        }
    }

    private void CheckEnemyInRange()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, weaponRange, enemyLayer);
        bool inRange = enemies.Length > 0;

        if (inRange)
        {
            if (rangeGlowEffectPrefab != null && currentRangeGlow == null)
                currentRangeGlow = Instantiate(rangeGlowEffectPrefab, transform);
        }
        else
        {
            if (currentRangeGlow != null)
            {
                Destroy(currentRangeGlow);
                currentRangeGlow = null;
            }
        }
    }


}
