using UnityEngine;

public class CombatVisualFollower : MonoBehaviour
{
    public Transform anchorTransform;
    public Vector3 targetLocalOffset = new Vector3(0.5f, 1.5f, 0f);
    public float followSpeed = 10f;
    public float swipeOffsetDistance = 1.2f;
    public float swipeReturnDelay = 0.3f;

    private bool facingRight = true;
    private Vector3 currentTargetPosition;
    private Vector3 baseOffset;
    private bool isInSwipeMotion = false;

    void Start()
    {
        baseOffset = targetLocalOffset;
    }

    void Update()
    {
        if (anchorTransform == null) return;

        Vector3 offset = facingRight ? baseOffset : new Vector3(-baseOffset.x, baseOffset.y, baseOffset.z);
        currentTargetPosition = anchorTransform.position + anchorTransform.rotation * offset;

        transform.position = Vector3.Lerp(transform.position, currentTargetPosition, Time.deltaTime * followSpeed);

        // Rotation (optional neigen nach vorne)
        Vector3 toAnchor = (anchorTransform.position - transform.position).normalized;
        if (toAnchor != Vector3.zero)
        {
            Quaternion targetRot = anchorTransform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 8f);
        }
    }

    public void SetFacingDirection(bool right)
    {
        facingRight = right;
    }

    public void ReactToSwipe(Vector2 swipeDirection)
    {
        if (isInSwipeMotion) return;

        isInSwipeMotion = true;

        // Richtung im Welt-Raum berechnen
        Vector3 worldSwipe = new Vector3(swipeDirection.x, 0f, swipeDirection.y).normalized;
        Vector3 newOffset = baseOffset + worldSwipe * swipeOffsetDistance;

        StopAllCoroutines();
        StartCoroutine(SwipeMotionCoroutine(newOffset));
    }

    private System.Collections.IEnumerator SwipeMotionCoroutine(Vector3 swipeTargetOffset)
    {
        Vector3 originalOffset = baseOffset;
        baseOffset = swipeTargetOffset;

        yield return new WaitForSeconds(swipeReturnDelay);

        baseOffset = originalOffset;
        isInSwipeMotion = false;
    }
}
