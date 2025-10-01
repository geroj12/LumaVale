using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float damage = 20f;
    public Enemy enemy;
    [SerializeField] private TrailRenderer weaponTrail;
    private bool canDealDamage = false;


    public void EnableDamage()
    {
        canDealDamage = true;


        if (weaponTrail != null)
        {
            weaponTrail.Clear();          // Trail-Cache löschen (verhindert Artefakte)
            weaponTrail.emitting = true; // ⬅️ Trail aktivieren
        }

    }

    public void DisableDamage()
    {
        canDealDamage = false;
        if (weaponTrail != null)
            weaponTrail.emitting = false; // ⬅️ Trail deaktivieren
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage) return;

        if (other.CompareTag("PlayerHitbox"))
        {
            Player player = other.GetComponentInParent<Player>();
            player.TakeDamage(damage, enemy.transform.position, false);
            Debug.Log("Player hit");
        }
        else if (other.CompareTag("PlayerShield"))
        {
            Player player = other.GetComponentInParent<Player>();
            player.TakeDamage(damage, enemy.transform.position, true);
            enemy.InterruptAttack(); // Bricht Angriffsanimation ab

            Debug.Log("Player shield hit");

        }

    }
}
