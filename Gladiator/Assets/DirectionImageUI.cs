using UnityEngine;

public class DirectionImageUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    private Transform mainCamera;

    void Start()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        mainCamera = Camera.main.transform;

    }
    void Update()
    {
        Vector3 lookDir = transform.position - mainCamera.position;
        lookDir.y = 0f; // Optional: kein Neigen
        transform.rotation = Quaternion.LookRotation(lookDir);
    }
}
