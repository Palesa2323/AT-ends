using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CoreTower : MonoBehaviour, ITakeDamage
{
    [Header("Core Health Settings")]
    public float MaxHealth = 100f;
    public float CurrentHealth;
    public Slider healthSlider;
    public Image healthFill;

    [Header("Attack Settings")]
    public float Range = 15f;
    public float Damage = 10f;
    public float FireRate = 1f;
    public LayerMask EnemyLayer;
    public Transform FirePoint; // optional, start of beam

    [Header("Visual Settings")]
    public LineRenderer lineRendererPrefab;  // assign in Inspector (a prefab with color gradient)
    public int MaxBeams = 10;                // how many simultaneous beams allowed
    private Queue<LineRenderer> laserPool = new Queue<LineRenderer>();

    private float fireTimer = 0f;
    private GameLoop gameLoop;

    void Start()
    {
        CurrentHealth = MaxHealth;
        gameLoop = FindFirstObjectByType<GameLoop>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth;
            healthSlider.value = CurrentHealth;
        }

        if (healthFill != null)
            healthFill.color = Color.green;

        // Pre-create a pool of reusable beams
        if (lineRendererPrefab != null)
        {
            for (int i = 0; i < MaxBeams; i++)
            {
                LineRenderer beam = Instantiate(lineRendererPrefab, transform);
                beam.enabled = false;
                laserPool.Enqueue(beam);
            }
        }
    }

    void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= 1f / FireRate)
        {
            AttackEnemiesInRange();
            fireTimer = 0f;
        }
    }

    private void AttackEnemiesInRange()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, Range, EnemyLayer);

        foreach (var hit in hitEnemies)
        {
            EnemyMovement enemy = hit.GetComponent<EnemyMovement>();
            if (enemy == null || enemy.Health <= 0)
                continue;

            // Deal damage
            enemy.TakeDamage(Damage);

            // Fire beam visual
            if (lineRendererPrefab != null)
                StartCoroutine(FireBeamAt(enemy.transform));
        }
    }

    private IEnumerator FireBeamAt(Transform target)
    {
        if (target == null)
            yield break;

        if (laserPool.Count == 0)
            yield break;

        LineRenderer beam = laserPool.Dequeue();
        beam.enabled = true;

        Vector3 start = FirePoint != null ? FirePoint.position : transform.position;
        beam.SetPosition(0, start);
        beam.SetPosition(1, target.position);

        yield return new WaitForSeconds(0.1f);

        beam.enabled = false;
        laserPool.Enqueue(beam);
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(0f, CurrentHealth);

        if (healthSlider != null)
            healthSlider.value = CurrentHealth;

        if (healthFill != null)
            healthFill.color = Color.Lerp(Color.red, Color.green, CurrentHealth / MaxHealth);

        if (CurrentHealth <= 0)
        {
            Debug.Log("Core Tower Destroyed! Game Over!");
            if (gameLoop != null)
                gameLoop.GameOver();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyMovement enemy = other.GetComponent<EnemyMovement>();
        if (enemy != null && enemy.gameObject.activeInHierarchy)
        {
            TakeDamage(enemy.damageToCore);
            EntitySummoner.RemoveEnemy(enemy);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
