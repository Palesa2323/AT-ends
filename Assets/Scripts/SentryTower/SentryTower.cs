using UnityEngine;

public class SentryTower : MonoBehaviour
{
    public float health = 50f;
    public float attackRange = 8f;
    public float attackRate = 1f;
    public float damage = 10f;
    public GameObject projectilePrefab;
    public Transform pivot; // A child object that will rotate to aim

    private float nextAttackTime;
    private Transform currentTarget;

    void Update()
    {
        if (currentTarget == null)
        {
            FindTarget();
        }
        else
        {
            // If the target is out of range or destroyed, find a new one
            if (Vector3.Distance(transform.position, currentTarget.position) > attackRange || currentTarget == null)
            {
                currentTarget = null;
                return;
            }

            // Look at the target
            pivot.LookAt(currentTarget);

            // Attack if cooldown is ready
            if (Time.time >= nextAttackTime)
            {
                ShootAt(currentTarget);
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void FindTarget()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));
        if (enemiesInRange.Length > 0)
        {
            currentTarget = enemiesInRange[0].transform;
        }
    }

    void ShootAt(Transform target)
    {
        Instantiate(projectilePrefab, transform.position, pivot.rotation);
        // The projectile script will handle the rest
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}