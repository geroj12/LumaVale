using UnityEngine;

public class FinisherExecutor : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private BoxCollider weaponCollider;
    [SerializeField] private Player player;
    [SerializeField] private State playerState;

    public void ExecuteFinisher(Enemy enemy, FinisherData finisher)
    {
        // Bewegung & Kontrolle abschalten
        playerState.canMove = false;
        cameraObject.SetActive(false);
        weaponCollider.isTrigger = true;
        player.enabled = false;

        // Position korrigieren
        if (enemy.finisherAnchor != null)
        {
            transform.position = enemy.finisherAnchor.position;
            transform.rotation = enemy.finisherAnchor.rotation;
        }

        // Animation triggern
        playerAnimator.SetTrigger(finisher.animation.name);
        enemy.StartFinisher(finisher.animation.name);
    }

    public void ResetFinisherState()
    {
        playerState.canMove = true;
        cameraObject.SetActive(true);
        weaponCollider.isTrigger = true;
        player.enabled = true;
    }
}
