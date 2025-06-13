using UnityEngine;

public class BloodSpawner : MonoBehaviour
{
    public GameObject bloodPrefab;
    public LayerMask decalAllowedLayers; // Set this in Inspector (exclude Player)

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, decalAllowedLayers))
            {
                // Check if target is in Player layer
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    Debug.Log("Hit Player — skip blood decal");
                    return;
                }

                // Rotation based on surface normal
                Vector3 direction = hit.normal;
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + 180;
                Quaternion rotation = Quaternion.Euler(0, angle + 90, 0);

                // Spawn blood
                var instance = Instantiate(bloodPrefab, hit.point, rotation);

                // Optional: Set ground height if needed
                var bloodSettings = instance.GetComponent<BFX_BloodSettings>();
                if (bloodSettings != null)
                {
                    bloodSettings.GroundHeight = hit.point.y;
                }
            }
        }
    }
}
