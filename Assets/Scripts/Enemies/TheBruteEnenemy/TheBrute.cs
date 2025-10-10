using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Brute : MonoBehaviour
{
    [Header("Brute Stats")]
    public float health = 30f;      // High Health
    public float speed = 1.5f;      // Slow Speed
    public float damage = 5f;       // High Damage
    public float attackRange = 1f;
    public float attackCooldown = 2f;

    private NavMeshAgent agent;
    private CoreTower nexusTarget;
    private float nextAttackTime;
    private GameLoop gameLoop; // Reference to the GameLoop

    void Awake()
    {
        // Find the single GameLoop instance to handle resource updates
        gameLoop = FindFirstObjectByType<GameLoop>();
        if (gameLoop == null)
        {
            Debug.LogError("GameLoop not found! Cannot reward player or access resource methods.");
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        // Find the CoreTower
        nexusTarget = FindFirstObjectByType<CoreTower>();

        // Coroutine ensures NavMeshAgent is active before setting destination
        StartCoroutine(SetInitialDestination());
    }

    IEnumerator SetInitialDestination()
    {
        yield return null; // Wait one frame for the agent to initialize

        if (nexusTarget != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(nexusTarget.transform.position);
        }
    }

    void Update()
    {
        if (nexusTarget == null) return;

        // The Brute always targets the CoreTower, ignoring all Sentry Towers.
        float distanceToNexus = Vector3.Distance(transform.position, nexusTarget.transform.position);

        if (distanceToNexus <= attackRange)
        {
            // Stop movement and attack the CoreTower
            agent.isStopped = true;
            AttackNexus();
        }
        else
        {
            // Resume movement towards the CoreTower
            agent.isStopped = false;
            agent.SetDestination(nexusTarget.transform.position);
        }
    }

    void AttackNexus()
    {
        if (Time.time >= nextAttackTime)
        {
            nexusTarget.TakeDamage(damage);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // Public method required by SentryTower, Barracks, and SnipeTower
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
        // Reward the player with resources (shards) upon defeat by calling the GameLoop method
        gameLoop?.AddResources(5);

        // Removed code that communicated with the WaveManager system.

        Destroy(gameObject);
    }
}