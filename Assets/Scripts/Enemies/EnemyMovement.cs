using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float MaxHealth;
    public float Health;
    public float Speed;
    public int ID;
    public int resourcesToAward = 10;
    public float damageToCore = 10f;

    public float AttackRange = 2f;
    public LayerMask TowerLayer; 

    private List<Vector3> waypoints;
    private int currentWaypointIndex = 0;
    private Rigidbody rb;
    private CoreTower coreTower;
    private EnemyHealthBar healthBar;
    
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

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(MaxHealth);
        }

        if (waypoints != null && waypoints.Count > 0)
        {
            transform.position = waypoints[0];
            currentWaypointIndex = 0;
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
            rb.linearVelocity = Vector3.zero;
            Vector3 lookDirection = currentTargetTransform.position - transform.position;
            if (lookDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 5f * Time.fixedDeltaTime);
            }
            
            if (enemyAttack != null)
            {
                enemyAttack.AttackTarget(currentTargetDamageable, currentTargetTransform);
            }
        }
        else
        {
            MoveAlongPath();
        }
    }

    private void CheckForTargets()
    {
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
        
        if (currentWaypointIndex >= waypoints.Count)
        {
            if (coreTower != null)
            {
                // Here's the fix: explicit casting with 'as' or direct casting
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