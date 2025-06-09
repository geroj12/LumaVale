using UnityEngine;

public class ProgressStrafeUI : MonoBehaviour
{
    private Transform mainCamera;
    void Start()
    {
        mainCamera = Camera.main.transform;

    }

    void Update()
    {
        Vector3 lookDir = transform.position - mainCamera.position;
        lookDir.y = 0f; // Optional: kein Neigen
        transform.rotation = Quaternion.LookRotation(lookDir);
    }
}
