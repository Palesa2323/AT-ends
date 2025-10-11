using UnityEngine;
using System.Collections;

public class ArtilleryTower : TowerBehaviour
{
    [Header("Artillery Settings")]
    public float AoeRadius = 3f;

    protected override void ExecuteAttack()
    {
        if (Target != null)
        {
            // The point where the AoE damage occurs
            Vector3 impactPoint = Target.transform.position;

            // Find all enemies within the AoE radius around the impact point
            // Uses the base TowerBehaviour's EnemiesLayer
            Collider[] hits = Physics.OverlapSphere(impactPoint, AoeRadius, EnemiesLayer);

            foreach (Collider hit in hits)
            {
                // We use EnemyMovement because all enemies inherit from it
                EnemyMovement enemy = hit.GetComponent<EnemyMovement>();
                if (enemy != null)
                {
                    // Apply full damage to all enemies in the area
                    enemy.TakeDamage(Damage);
                }
            }

            // Trigger visual effect (You may need a different coroutine for a splash effect)
            StartCoroutine(FireLaser()); // Using base laser for simplicity to impact point
        }
    }
}
