using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Brute : MonoBehaviour
{
    [Header("Brute Stats")]
    public float health = 30f;       // High Health
    public float speed = 1.5f;       // Slow Speed
    public float damage = 5f;        // High Damage
    public float attackRange = 1.2f; // Slightly extended for big enemies
    public float attackCooldown = 2f;

    private NavMeshAgent agent;
    private CoreTower coreTower;
    private float nextAttackTime;
    private GameLoop gameLoop;

    void Awake()
    {
        gameLoop = FindFirstObjectByType<GameLoop>();
        if (gameLoop == null)
        {
            Debug.LogError("GameLoop not found! Cannot reward player or access resource methods.");
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("Brute needs a NavMeshAgent component.");
            return;
        }

        agent.speed = speed;
        coreTower = FindFirstObjectByType<CoreTower>();

        StartCoroutine(SetInitialDestination());
    }

    IEnumerator SetInitialDestination()
    {
        yield return null;

        if (coreTower != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(coreTower.transform.position);
        }
    }

    void Update()
    {
        if (coreTower == null) return;

        float distanceToCore = Vector3.Distance(transform.position, coreTower.transform.position);

        if (distanceToCore <= attackRange)
        {
            agent.isStopped = true;
            AttackCore();
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(coreTower.transform.position);
        }
    }

    void AttackCore()
    {
        if (Time.time >= nextAttackTime)
        {
            coreTower.TakeDamage(damage);
            nextAttackTime = Time.time + attackCooldown;
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
        if (gameLoop != null)
        {
            gameLoop.AddResources(10); // Brutes drop more resources since they’re tanky
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
