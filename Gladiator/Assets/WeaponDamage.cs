using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public float damage = 20f;
    public Player player;

    private bool canDealDamage = false;

    public void EnableDamage()
    {
        canDealDamage = true;
    }

    public void DisableDamage()
    {
        canDealDamage = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage) return;
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, player.transform.position);
                canDealDamage = false; // Nur 1x pro Schlag
            }
        }
        else if (other.CompareTag("EnemyShield"))
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(0f, player.transform.position, true);
                player.InterruptAttack(); // Bricht Angriffsanimation ab
            }
        }
    }
}
