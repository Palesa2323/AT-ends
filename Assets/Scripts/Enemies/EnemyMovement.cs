using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour, ITakeDamage
{
    // New Enum to distinguish enemy types in code
    public enum EnemyType { Normal, Runner, Healer }

    // Core Enemy Stats
    public EnemyType enemyType = EnemyType.Normal; // Set this in the inspector
    public float MaxHealth;
    public float Health;
    public float Speed;
    public int ID;
    public int resourcesToAward = 10;
    public float damageToCore = 10f;

    // Attack Settings
    public float AttackRange = 2f;
    public LayerMask TowerLayer;

    [Header("Healer Settings")]
    public float HealRadius = 5f;
    public float HealAmount = 10f;

    // Private Component and Path References
    private List<Vector3> waypoints;
    private int currentWaypointIndex = 0;
    private Rigidbody rb;
    private CoreTower coreTower;
    public EnemyHealthBar healthBar;
    private EnemyAttack enemyAttack;
    private ITakeDamage currentTargetDamageable;
    private Transform currentTargetTransform;

    public void Init(List<Vector3> assignedPath, CoreTower tower)
    {
        Health = MaxHealth;
        waypoints = assignedPath;
        rb = GetComponent<Rigidbody>();

        coreTower = tower;
        healthBar = GetComponent<EnemyHealthBar>();
        enemyAttack = GetComponent<EnemyAttack>();
        currentWaypointIndex = 0; // Reset for pooling

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(MaxHealth);
        }

        if (waypoints != null && waypoints.Count > 0)
        {
            transform.position = waypoints[0];
        }
        else
        {
            Debug.LogError("Assigned path is empty or null!");
        }
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (healthBar != null)
        {
            healthBar.SetCurrentHealth(Health);
        }

        if (Health <= 0)
        {
            GameLoop gameLoop = FindFirstObjectByType<GameLoop>();
            if (gameLoop != null)
            {
                gameLoop.AddResources(resourcesToAward);
            }
            EntitySummoner.RemoveEnemy(this);
        }
    }

    void FixedUpdate()
    {
        if (waypoints == null) return;

        CheckForTargets();

        if (currentTargetDamageable != null && currentTargetTransform != null)
        {
            // Stop and engage target
            rb.linearVelocity = Vector3.zero;

            Vector3 lookDirection = currentTargetTransform.position - transform.position;
            if (lookDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 5f * Time.fixedDeltaTime);
            }

            // NEW LOGIC: Healer vs. Attacker
            if (enemyType == EnemyType.Healer)
            {
                HealAllies();
            }
            else // Normal and Runner attack targets
            {
                if (enemyAttack != null)
                {
                    enemyAttack.AttackTarget(currentTargetDamageable, currentTargetTransform);
                }
            }
        }
        else
        {
            // Continue moving along the path
            MoveAlongPath();
        }
    }

    // NEW: Healer's unique behavior
    private void HealAllies()
    {
        // Use LayerMask.GetMask("Enemy") assuming you created the "Enemy" layer
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, HealRadius, LayerMask.GetMask("Enemy"));

        // Use the EnemyAttack timer to control the healing rate
        if (enemyAttack != null && Time.time >= enemyAttack.nextAttackTime)
        {
            // Advance the attack timer immediately to match the Heal Rate
            enemyAttack.nextAttackTime = Time.time + 1f / enemyAttack.attackRate;

            foreach (Collider collider in enemiesInRange)
            {
                EnemyMovement ally = collider.GetComponent<EnemyMovement>();
                if (ally != null && ally != this) // Must be an ally, not self
                {
                    // Heal and clamp health to MaxHealth
                    ally.Health = Mathf.Min(ally.Health + HealAmount, ally.MaxHealth);

                    // Update Health Bar if it exists
                    if (ally.healthBar != null)
                    {
                        ally.healthBar.SetCurrentHealth(ally.Health);
                    }

                    // Optional: You could trigger a visual effect on the ally here
                }
            }
        }
    }


    private void CheckForTargets()
    {
        // 1. Check for nearby defender towers within attack range
        Collider[] targetsInRange = Physics.OverlapSphere(transform.position, AttackRange, TowerLayer);

        if (targetsInRange.Length > 0)
        {
            ITakeDamage foundDamageable = targetsInRange[0].GetComponent<ITakeDamage>();
            if (foundDamageable != null)
            {
                currentTargetDamageable = foundDamageable;
                currentTargetTransform = targetsInRange[0].transform;
                return;
            }
        }

        // 2. Check if we've reached the end of the path (Target: Core Tower)
        if (currentWaypointIndex >= waypoints.Count)
        {
            if (coreTower != null)
            {
                currentTargetDamageable = coreTower as ITakeDamage;
                currentTargetTransform = coreTower.transform;
            }
            else
            {
                EntitySummoner.RemoveEnemy(this);
            }
        }
        else
        {
            // No targets found, reset current target
            currentTargetDamageable = null;
            currentTargetTransform = null;
        }
    }

    private void MoveAlongPath()
    {
        if (currentWaypointIndex < waypoints.Count)
        {
            Vector3 targetPosition = waypoints[currentWaypointIndex];
            Vector3 direction = (targetPosition - transform.position).normalized;

            rb.MovePosition(transform.position + direction * Speed * Time.fixedDeltaTime);

            if (direction != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 5f * Time.fixedDeltaTime);
            }

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
    }
}
