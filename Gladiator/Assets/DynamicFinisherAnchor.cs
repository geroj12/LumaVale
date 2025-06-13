using UnityEngine;

public class DynamicFinisherAnchor : MonoBehaviour
{
    [Header("Settings")]
    public Transform anchorPoint; // Leeres Child z. B. "FinisherAnchor"
    public float idealDistance = 1.8f;
    public float minDistance = 0.6f;

    /// <summary>
    /// Ruft diese Methode nur auf, wenn Spieler versucht, einen Finisher auf diesen Gegner zu starten.
    /// </summary>
    /// <param name="player">Transform des Spielers</param>
    public void UpdateAnchorToPlayer(Transform player)
    {
        if (anchorPoint == null || player == null) return;

        Vector3 toPlayer = (player.position - transform.position).normalized;
        toPlayer.y = 0;

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance < minDistance) return; // Zu nah – kein Anker-Update nötig

        Vector3 desiredPos = transform.position + toPlayer * idealDistance;
        anchorPoint.position = desiredPos;
        anchorPoint.rotation = Quaternion.LookRotation(-toPlayer); // Anchor schaut zum Gegner
    }
}
