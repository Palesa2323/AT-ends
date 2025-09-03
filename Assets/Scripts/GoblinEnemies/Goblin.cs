using UnityEngine;
using UnityEngine.AI;
using System.Collections; 


[RequireComponent(typeof(NavMeshAgent))]
public class Goblin : MonoBehaviour
{
    [Header("Enemy Properties")]
    public float health = 10f;
    public float speed = 1f;
    public float damage = 1f;
    public int crystalShardsValue = 1;
    public float attackRange = 1f;
    public float attackCooldown = 1f;

    private NavMeshAgent agent;
    private Transform target;
    private SentryTower currentTowerTarget;
    private Nexus nexusTarget;
    private float nextAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        // Find the Nexus at the start of the game using the new, more efficient method
        nexusTarget = FindFirstObjectByType<Nexus>();

        // Start the coroutine to set the initial destination after one frame
        StartCoroutine(SetInitialDestination());
    }

    // Coroutine to wait one frame before setting the destination
    IEnumerator SetInitialDestination()
    {
        // Wait for one frame
        yield return null;

        // Set the initial destination to the Nexus
        if (nexusTarget != null)
        {
            agent.SetDestination(nexusTarget.transform.position);
        }
    }

    void Update()
    {
        // First, check for a nearby Sentry Tower to attack
        FindClosestTower();

        if (currentTowerTarget != null)
        {
            // Move towards the tower
            agent.SetDestination(currentTowerTarget.transform.position);

            // Check if we are in attack range of the tower
            if (Vector3.Distance(transform.position, currentTowerTarget.transform.position) <= attackRange)
            {
                AttackTower();
            }
        }
        else
        {
            // If no tower is found, move towards the Nexus
            if (target == null && nexusTarget != null)
            {
                agent.SetDestination(nexusTarget.transform.position);
            }

            // Check if we have reached the Nexus
            if (nexusTarget != null && Vector3.Distance(transform.position, nexusTarget.transform.position) <= attackRange)
            {
                AttackNexus();
            }
        }
    }

    // Finds the closest Sentry Tower within a certain range
    void FindClosestTower()
    {
        // Use the new, more efficient FindObjectsByType
        SentryTower[] allTowers = FindObjectsByType<SentryTower>(FindObjectsSortMode.None);
        SentryTower closestTower = null;
        float shortestDistance = Mathf.Infinity;

        foreach (SentryTower tower in allTowers)
        {
            float distanceToTower = Vector3.Distance(transform.position, tower.transform.position);
            if (distanceToTower < shortestDistance)
            {
                shortestDistance = distanceToTower;
                closestTower = tower;
            }
        }

        // If the closest tower is within a reasonable "aggro" range, set it as the target
        if (closestTower != null && shortestDistance <= 20f)
        {
            currentTowerTarget = closestTower;
        }
        else
        {
            currentTowerTarget = null;
        }
    }

    void AttackTower()
    {
        if (Time.time >= nextAttackTime)
        {
            if (currentTowerTarget != null)
            {
                currentTowerTarget.TakeDamage(damage);
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void AttackNexus()
    {
        if (Time.time >= nextAttackTime)
        {
            if (nexusTarget != null)
            {
                nexusTarget.TakeDamage(damage);
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddShards(crystalShardsValue);
        }
        Destroy(gameObject);
    }
}
