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
    private LineRenderer lineRenderer;

    void Start()
    {
        Delay = 1f / FireRate;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    void Update()
    {
        fireTimer += Time.deltaTime;

        if (Target == null || Target.Health <= 0 || !Target.gameObject.activeSelf)
        {
            Target = TowerTargetting.GetTarget(this, TowerTargetting.TargetType.First);
            if (Target == null)
            {
                lineRenderer.enabled = false;
                return;
            }
        }

        if (Vector3.Distance(transform.position, Target.transform.position) > Range)
        {
            Target = null;
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, TowerPivot.position);
        lineRenderer.SetPosition(1, Target.transform.position);
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
                Target.TakeDamage(Damage);
            fireTimer = 0f;
            }
    }
}