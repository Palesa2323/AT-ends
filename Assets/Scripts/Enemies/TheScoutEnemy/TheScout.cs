using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;

public class Scout : MonoBehaviour
{
    [Header("Scout Stats")]
    public float health = 5f;       // Low health
    public float speed = 5f;        // High speed
    public float damage = 2f;       // Moderate damage
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float detectionRange = 7f; // Range to spot towers

    private NavMeshAgent agent;
    private CoreTower coreTower;
    private TowerBehaviour currentTarget;
    private float nextAttackTime;
    private GameLoop gameLoop;

    void Awake()
    {
        gameLoop = FindFirstObjectByType<GameLoop>();
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("Scout needs a NavMeshAgent component.");
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

        FindNewTarget();

        if (currentTarget != null)
        {
            float distanceToTower = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (distanceToTower <= attackRange)
            {
                agent.isStopped = true;
                AttackTower(currentTarget);
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(currentTarget.transform.position);
            }
        }
        else
        {
            // No tower in range, attack CoreTower
            agent.isStopped = false;
            agent.SetDestination(coreTower.transform.position);

            float distanceToCore = Vector3.Distance(transform.position, coreTower.transform.position);
            if (distanceToCore <= attackRange)
            {
                agent.isStopped = true;
                AttackCoreTower();
            }
        }
    }

    void FindNewTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange, LayerMask.GetMask("Tower"));

        if (currentTarget == null ||
            !currentTarget.gameObject.activeInHierarchy ||
            Vector3.Distance(transform.position, currentTarget.transform.position) > detectionRange)
        {
            currentTarget = null;

            if (hitColliders.Length > 0)
            {
                currentTarget = hitColliders
                    .Select(col => col.GetComponent<TowerBehaviour>())
                    .Where(t => t != null)
                    .OrderBy(t => Vector3.Distance(transform.position, t.transform.position))
                    .FirstOrDefault();
            }
        }
    }

    void AttackTower(TowerBehaviour tower)
    {
        if (Time.time >= nextAttackTime)
        {
            tower.Target?.TakeDamage(damage); // Damage whatever the tower is targeting
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void AttackCoreTower()
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
            gameLoop.AddResources(5);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
