using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float damage = 20f;
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

        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.TakeDamage(damage, false);
            Debug.Log("Player hit");
        }
        else if (other.CompareTag("PlayerShield"))
        {
            Player player = other.GetComponentInChildren<Player>();
            player.TakeDamage(damage, true);
            Debug.Log("Player shield hit");

        }

    }
}
