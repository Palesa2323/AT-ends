using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    // These are now public fields
    public float Range;
    public LayerMask EnemiesLayer;

    // Other public variables
    public EnemyMovement Target;
    public Transform TowerPivot;
    public float Damage;
    public float FireRate;

    private float delay;
    private float fireTimer;
    private LineRenderer lineRenderer;

    void Start()
    {
        delay = 1f / FireRate;
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    void Update()
    {
        if (Target == null || Target.Health <= 0 || !Target.gameObject.activeSelf || Vector3.Distance(transform.position, Target.transform.position) > Range)
        {
            // Now correctly calling the GetTarget overload for TowerBehaviour
            Target = TowerTargetting.GetTarget(this, TowerTargetting.TargetType.First);
            if (Target == null)
            {
                if (lineRenderer != null) lineRenderer.enabled = false;
                return;
            }
        }

        if (TowerPivot != null && Target != null)
        {
            Vector3 direction = Target.transform.position - TowerPivot.position;
            direction.y = 0;
            TowerPivot.rotation = Quaternion.LookRotation(direction);
        }

        fireTimer += Time.deltaTime;
        if (fireTimer >= delay && Target != null)
        {
            Target.TakeDamage(Damage);
            fireTimer = 0f;

            if (lineRenderer != null)
            {
                StartCoroutine(FireLaser());
            }
        }
    }

    IEnumerator FireLaser()
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, TowerPivot.position);
        lineRenderer.SetPosition(1, Target.transform.position);
        yield return new WaitForSeconds(0.1f);
        lineRenderer.enabled = false;
    }
}