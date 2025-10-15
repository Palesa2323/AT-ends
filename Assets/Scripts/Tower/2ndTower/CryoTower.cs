using UnityEngine;
using System.Collections;

public class CryoTower : TowerBehaviour
{
    [Header("Cryo Settings")]
    public float SlowFactor = 0.5f;
    public float SlowDuration = 2f;

    void Update()
    {
        if (Target == null || Target.Health <= 0 || !Target.gameObject.activeSelf || Vector3.Distance(transform.position, Target.transform.position) > Range)
        {
            Target = TowerTargetting.GetTarget(this, TowerTargetting.TargetType.First);
            if (Target != null && Target.enemyType == EnemyMovement.EnemyType.Healer)
                Target = null;

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
            StartCoroutine(ApplyCryoEffect(Target));
            fireTimer = 0f;

            if (lineRenderer != null)
                StartCoroutine(FireLaser(Color.cyan));
        }
    }

    IEnumerator ApplyCryoEffect(EnemyMovement enemy)
    {
        if (enemy == null || enemy.enemyType == EnemyMovement.EnemyType.Healer) yield break;

        float originalSpeed = enemy.Speed;
        enemy.Speed *= SlowFactor;
        enemy.TakeDamage(Damage);

        yield return new WaitForSeconds(SlowDuration);

        if (enemy != null && enemy.gameObject.activeSelf)
            enemy.Speed = originalSpeed;
    }

    IEnumerator FireLaser(Color laserColor)
    {
        lineRenderer.enabled = true;
        lineRenderer.startColor = laserColor;
        lineRenderer.endColor = laserColor;
        lineRenderer.SetPosition(0, TowerPivot.position);
        lineRenderer.SetPosition(1, Target.transform.position);
        yield return new WaitForSeconds(0.1f);
        lineRenderer.enabled = false;
    }
}

