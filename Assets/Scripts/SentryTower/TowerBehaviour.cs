using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    public LayerMask EnemiesLayer;
    public EnemyMovement Target;
    public Transform TowerPivot;
    public float Damage;
    public float FireRate;
    public float Range;
    private float Delay;
    private float fireTimer;

    void Start()
    {
        Delay = 1f / FireRate;
    }

    void Update()
    {
        // First, check if we have a target
        if (Target == null)
        {
            // If not, find one using the targetting class
            Target = TowerTargetting.GetTarget(this, TowerTargetting.TargetType.First);
        }
        else
        {
            // If we have a target, check if it's still in range and alive
            if (Vector3.Distance(transform.position, Target.transform.position) > Range || Target.Health <= 0)
            {
                Target = null; // Target is gone, look for a new one
                return; // Stop here and wait for the next frame
            }

            // Aim at the target
            if (TowerPivot != null)
            {
                Vector3 direction = Target.transform.position - TowerPivot.position;
                direction.y = 0; // Prevent looking up/down if you want a flat aim
                TowerPivot.rotation = Quaternion.LookRotation(direction);
            }

            // Attack logic
            fireTimer += Time.deltaTime;
            if (fireTimer >= Delay)
            {
                // This is where you would do something like fire a projectile
                // For now, let's just deal damage directly
                // You would need a TakeDamage method on the enemy script
                fireTimer = 0f;
            }
        }
    }
}