using UnityEngine;
using System.Collections;

public class BombTower : TowerBehaviour
{
    [Header("Bomb Settings")]
    public float ExplosionRadius = 3f;

    void Update()
    {
        if (Target == null || Target.Health <= 0 || !Target.gameObject.activeSelf || Vector3.Distance(transform.position, Target.transform.position) > Range)
        {
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
            StartCoroutine(FireBomb(Target.transform.position));
            fireTimer = 0f;
        }
    }

    IEnumerator FireBomb(Vector3 impactPoint)
    {
        // optional explosion visual placeholder
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.yellow;
            lineRenderer.SetPosition(0, TowerPivot.position);
            lineRenderer.SetPosition(1, impactPoint);
            yield return new WaitForSeconds(0.1f);
            lineRenderer.enabled = false;
        }

        yield return new WaitForSeconds(0.2f); // simulate projectile travel time

        Collider[] hitEnemies = Physics.OverlapSphere(impactPoint, ExplosionRadius, EnemiesLayer);
        foreach (Collider hit in hitEnemies)
        {
            EnemyMovement e = hit.GetComponent<EnemyMovement>();
            if (e != null)
                e.TakeDamage(Damage);
        }
    }
}
