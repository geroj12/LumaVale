using UnityEngine;
using static FinisherData;

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

        if (enemy.finisherAnchor != null)
        {
            transform.position = enemy.finisherAnchor.position;

            switch (finisher.rotationMode)
            {
                case FinisherRotationMode.FaceTarget:
                    RotateTowards(enemy.transform, transform);       // Spieler
                    RotateTowards(transform, enemy.transform);       // Gegner
                    break;

                case FinisherRotationMode.CustomRotation:
                    transform.rotation = Quaternion.Euler(finisher.playerRotationEuler);
                    enemy.transform.rotation = Quaternion.Euler(finisher.enemyRotationEuler);
                    break;

                case FinisherRotationMode.Ignore:
                    // Kein Eingriff – z. B. RootMotion oder bereits durch Animation geregelt
                    break;


            }
            // Animation triggern
            playerAnimator.SetTrigger(finisher.animation.name);
            enemy.StartFinisher(finisher.animation.name);
        }
    }
    private void RotateTowards(Transform target, Transform self)
    {
        Vector3 dir = (target.position - self.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
            self.rotation = Quaternion.LookRotation(dir);
    }
    public void ResetFinisherState()
    {
        playerState.canMove = true;
        cameraObject.SetActive(true);
        weaponCollider.isTrigger = true;
        player.enabled = true;
    }
}
