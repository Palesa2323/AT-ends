using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Assumes ITakeDamage interface is defined elsewhere
public abstract class TowerBehaviour : MonoBehaviour
{
    // CORE PUBLIC STATS (Set in Inspector)
    public float Range = 10f;
    public LayerMask EnemiesLayer;
    public EnemyMovement Target;
    public Transform TowerPivot;
    public float Damage = 10f;
    public float FireRate = 1f;

    [Header("Defender Health")]
    public float MaxHealth = 50f;
    [HideInInspector] public float Health;

    // PRIVATE FIELDS
    public float delay;
    public float fireTimer;
    private LineRenderer lineRenderer;

    protected virtual void Start()
    {
        Health = MaxHealth; // Initialize health
        delay = 1f / FireRate;
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

        // Initialize TowerPivot if null (can be set manually in Inspector)
        if (TowerPivot == null && transform.childCount > 0)
        {
            TowerPivot = transform.GetChild(0);
        }
    }

    protected virtual void Update()
    {
        // 1. TARGETING LOGIC (SHARED)
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

        // 2. ROTATION LOGIC (SHARED)
        if (TowerPivot != null && Target != null)
        {
            Vector3 direction = Target.transform.position - TowerPivot.position;
            direction.y = 0;
            TowerPivot.rotation = Quaternion.LookRotation(direction);
        }

        // 3. ATTACK TIMER LOGIC (SHARED)
        fireTimer += Time.deltaTime;
        if (fireTimer >= delay)
        {
            fireTimer = 0f;
            ExecuteAttack(); // Calls the unique attack for the inherited script
        }
    }

    // NEW: Damage-taking logic
    public void TakeDamage(float damage)
    {
        Health -= damage;
        // Optional: Update health bar here
        if (Health <= 0)
        {
            Destroy(gameObject); // Defender is destroyed
        }
    }

    // NEW: Abstract method that MUST be implemented by child classes
    protected abstract void ExecuteAttack();

    // NEW: Laser visual effect helper (still protected for children to use)
    protected IEnumerator FireLaser()
    {
        if (lineRenderer != null && Target != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, TowerPivot.position);
            lineRenderer.SetPosition(1, Target.transform.position);
            yield return new WaitForSeconds(0.05f); // Short display time
            lineRenderer.enabled = false;
        }
    }
}