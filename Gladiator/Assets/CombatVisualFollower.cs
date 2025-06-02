using UnityEngine;

public class CombatVisualFollower : MonoBehaviour
{
    public Transform anchorTransform;
    public float followSpeed = 8f;
    public Vector3 idleLocalOffset = new Vector3(1f, 1.5f, 0f); // Lokaler Offset relativ zur Blickrichtung
    public float swipeOffsetDistance = 2f;
    public float swipeMoveDuration = 0.3f;

    private Vector3 currentLocalOffset;
    private float swipeTimer = 0f;
    private bool inSwipeMove = false;

    private float floatAmplitude = 0.2f;
    private float floatFrequency = 2f;

    void Start()
    {
        currentLocalOffset = idleLocalOffset;
    }

    void Update()
    {
        if (anchorTransform == null) return;

        // Wackel-Offset für lebendiges Schweben
        float floatOffsetY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        Vector3 floatOffset = new Vector3(0f, floatOffsetY, 0f);

        // Swipe-Timer auslaufen lassen
        if (inSwipeMove)
        {
            swipeTimer -= Time.deltaTime;
            if (swipeTimer <= 0f)
            {
                inSwipeMove = false;
                currentLocalOffset = idleLocalOffset;
            }
        }

        // Offset in Weltkoordinaten transformieren (Blickrichtungs-abhängig)
        Vector3 worldOffset = anchorTransform.TransformDirection(currentLocalOffset);
        Vector3 targetPosition = anchorTransform.position + worldOffset + floatOffset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
    }

    public void OnSwipeDirection(bool swipeFromRight)
    {
        float dir = swipeFromRight ? -1f : 1f;
        currentLocalOffset = idleLocalOffset + new Vector3(dir * swipeOffsetDistance, 0f, 0f);
        swipeTimer = swipeMoveDuration;
        inSwipeMove = true;
    }

    public void SetFacingDirection(bool facingRight)
    {
        idleLocalOffset = new Vector3(facingRight ? 1f : -1f, 1.5f, 0f);
        if (!inSwipeMove)
        {
            currentLocalOffset = idleLocalOffset;
        }
    }
}
