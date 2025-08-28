using UnityEngine;

public class Nexus : MonoBehaviour
{
    public float health = 100f;
    public float attackRange = 10f;
    public float attackRate = 1f;
    public float damage = 5f;
    public GameObject projectilePrefab;

    private float nextAttackTime;

    void Update()
    {
        // Check for enemies in range
        if (Time.time >= nextAttackTime)
        {
            Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));
            if (enemiesInRange.Length > 0)
            {
                // Simple targeting: find the first enemy in the list
                Transform target = enemiesInRange[0].transform;
                ShootAt(target);
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void ShootAt(Transform target)
    {
        // Instantiate a projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // Make the projectile move towards the target.
        // You'll need a separate simple script on the projectile prefab itself to handle its movement and collision.
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            // The Nexus is destroyed, game over
            Debug.Log("Game Over! The Nexus has been destroyed.");
            // You can add a call to your GameManager here
        }
    }
}