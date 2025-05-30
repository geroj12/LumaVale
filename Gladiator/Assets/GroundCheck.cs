using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool isGrounded;

    [SerializeField] private LayerMask groundLayer;

    public enum CurrentTerrain { Grass, Mud, Dirt, Gravel }

    public CurrentTerrain currentTerrain = CurrentTerrain.Grass;

    // Öffentliche Property für FMOD (z. B. "Grass")
    public string currentTerrainLabel => currentTerrain.ToString();

    private void Update()
    {
        CheckGrounded();
        DetectTerrain();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(transform.position, 1f, groundLayer);
    }

    private void DetectTerrain()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f))
        {
            int layer = hit.transform.gameObject.layer;

            if (layer == LayerMask.NameToLayer("Grass"))
                currentTerrain = CurrentTerrain.Grass;
            else if (layer == LayerMask.NameToLayer("Mud"))
                currentTerrain = CurrentTerrain.Mud;
            else if (layer == LayerMask.NameToLayer("Dirt"))
                currentTerrain = CurrentTerrain.Dirt;
            else if (layer == LayerMask.NameToLayer("Gravel"))
                currentTerrain = CurrentTerrain.Gravel;
        }
    }
}
