using UnityEngine;

public class BombProjectile : MonoBehaviour
{
    private Vector3 targetPosition;
    private float damage;
    private float radius;
    private GameObject explosionPrefab;
    private LayerMask enemiesLayer; // LayerMask passed from the tower
    private float speed = 15f;

    // Updated Init signature
    public void Init(Vector3 targetPos, float dmg, float rad, GameObject explosionFX, LayerMask layer)
    {
        targetPosition = targetPos;
        damage = dmg;
        radius = rad;
        explosionPrefab = explosionFX;
        enemiesLayer = layer;

        // Ensure bomb hits the floor level where enemies move
        targetPosition.y = 0.5f;
    }

    void Update()
    {
        // Check if the target is already hit
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Explode();
            return;
        }

        // Move the projectile towards the target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    private void Explode()
    {
        // 1. Find all colliders within the radius on the specified layer
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, enemiesLayer);

        // 2. Deal damage to ALL valid enemies
        foreach (var hitCollider in hitColliders)
        {
            EnemyMovement enemy = hitCollider.GetComponent<EnemyMovement>();

            // Apply damage only if it is a valid, non-Healer enemy
            if (enemy != null && enemy.enemyType != EnemyMovement.EnemyType.Healer)
            {
                enemy.TakeDamage(damage);
            }
        }

        // 3. Spawn effect and destroy self
        if (explosionPrefab != null)
        {
            // Instantiate the visual/audio effect
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}