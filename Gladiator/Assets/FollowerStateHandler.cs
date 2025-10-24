using UnityEngine;

public class FollowerStateHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FollowerAI ai;
    [SerializeField] private FollowerController controller;
    [SerializeField] private OrbsHandler energyManager;
    [SerializeField] private Transform player;

    private bool playerDead = false;

    private void Start()
    {
        ai.enabled = true;
        controller.enabled = false;
    }

    private void Update()
    {
        // Prüfe, ob Player tot ist (hier Platzhalter - du ersetzt das später mit deiner PlayerState-Referenz)
        if (playerDead && !controller.enabled)
        {
            EnterIndependentMode();
        }
        else if (!playerDead && controller.enabled)
        {
            ReturnToFollowerMode();
        }
    }

    public void SetPlayerDead(bool dead)
    {
        playerDead = dead;
    }

    private void EnterIndependentMode()
    {
        ai.enabled = false;
        controller.enabled = true;

        Debug.Log("Follower is now independent!");
    }

    private void ReturnToFollowerMode()
    {
        ai.enabled = true;
        controller.enabled = false;

        Debug.Log("Follower returned to AI mode.");
    }
}
