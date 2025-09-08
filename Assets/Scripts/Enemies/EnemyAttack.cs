using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public float attackDamage = 10f;
    public float attackRate = 1f;
    private float nextAttackTime = 0f;

    public Transform AttackOrigin;
    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        }
        lineRenderer.enabled = false;
    }

    public void AttackTarget(ITakeDamage target, Transform targetTransform)
    {
        if (Time.time >= nextAttackTime)
        {
            target.TakeDamage(attackDamage);
            nextAttackTime = Time.time + 1f / attackRate;
            StartCoroutine(FireLaser(targetTransform));
        }
    }

    IEnumerator FireLaser(Transform targetTransform)
    {
        lineRenderer.enabled = true;
        Vector3 startPoint = (AttackOrigin != null) ? AttackOrigin.position : transform.position;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, targetTransform.position);

        yield return new WaitForSeconds(0.1f);
        lineRenderer.enabled = false;
    }
}