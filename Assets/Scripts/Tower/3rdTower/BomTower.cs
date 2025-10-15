using UnityEngine;
using System.Collections;

public class BombTower : TowerBehaviour
{
    // Inherits Damage, FireRate, Range, Target, fireTimer, etc., from TowerBehaviour

    [Header("Bomb Settings")]
    public GameObject ProjectilePrefab; // Must be linked in Inspector
    public GameObject ExplosionPrefab;  // Must be linked in Inspector
    public float ExplosionRadius = 3f;

    // Use a custom update to handle the projectile launch
    void Update()
    {
        // TARGETING LOGIC (Same as base class, but includes Healer skip)
        if (Target == null || Target.Health <= 0 || !Target.gameObject.activeSelf || Vector3.Distance(transform.position, Target.transform.position) > Range)
        {
            Target = TowerTargetting.GetTarget(this, TowerTargetting.TargetType.First);

            // This Healer check is redundant if done in TowerTargetting.cs, but harmless here.
            if (Target != null && Target.enemyType == EnemyMovement.EnemyType.Healer)
                Target = null;

            if (Target == null)
            {
                if (lineRenderer != null) lineRenderer.enabled = false;
                return;
            }
        }

        // ROTATION LOGIC
        if (TowerPivot != null && Target != null)
        {
            Vector3 direction = Target.transform.position - TowerPivot.position;
            direction.y = 0;
            // Use Quaternion.Slerp for smooth rotation (better than direct assignment)
            TowerPivot.rotation = Quaternion.Slerp(TowerPivot.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);
        }

        // FIRING LOGIC
        fireTimer += Time.deltaTime;
        if (fireTimer >= delay && Target != null)
        {
            // NEW: Fire the dedicated AoE projectile
            FireAoEProjectile(Target.transform.position);
            fireTimer = 0f;
        }
    }

    private void FireAoEProjectile(Vector3 targetPosition)
    {
        if (ProjectilePrefab == null) return;

        // 1. Instantiate the projectile at the tower pivot
        GameObject projectileGO = Instantiate(ProjectilePrefab, TowerPivot.position, Quaternion.identity);

        // 2. Get the Bomb script component
        BombProjectile bomb = projectileGO.GetComponent<BombProjectile>();

        if (bomb != null)
        {
            // 3. Initialize the bomb with the tower's stats
            // NOTE: We pass EnemiesLayer so the projectile knows what to hit
            bomb.Init(targetPosition, Damage, ExplosionRadius, ExplosionPrefab, EnemiesLayer);
        }
        else
        {
            Debug.LogError("ProjectilePrefab is missing the BombProjectile script!");
            Destroy(projectileGO);
        }
    }
}