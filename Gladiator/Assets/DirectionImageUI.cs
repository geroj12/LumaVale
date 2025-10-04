using UnityEngine;

public class DirectionImageUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    private Camera mainCamera;

    void Start()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        mainCamera = Camera.main;

    }
    void LateUpdate()
    {
        if (mainCamera == null) return;

        // Schritt 1: Immer zur Kamera ausrichten (Billboard-Effekt)
        transform.forward = mainCamera.transform.forward;

        // Schritt 2: Beispielrotation Richtung Maus
        if (Input.GetMouseButton(0))
        {


            // Richtung von UI-Element -> Maus (im ScreenSpace)
            Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
            Vector3 dir = Input.mousePosition - screenPos;

            // Winkel bestimmen
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // Rotation zur Kamera + Z-Winkel für die Pfeilrichtung
            transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward) * Quaternion.Euler(0, 0, angle);
        }

    }
}
