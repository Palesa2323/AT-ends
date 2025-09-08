using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float MaxHealth;
    public float Health;
    public float Speed;
    public int ID;
    public int NodeIndex;
    public int resourcesToAward = 10;
    public float damageToCore = 10f;

    private List<Vector3> waypoints;
    private int currentWaypointIndex = 0;
    private Rigidbody rb;
    private CoreTower coreTower;
    private EnemyHealthBar healthBar;

    public void Init(List<Vector3> assignedPath, CoreTower tower)
    {
        Health = MaxHealth;
        waypoints = assignedPath;
        rb = GetComponent<Rigidbody>();

        // assign the reference to core tower
        coreTower = tower;

        healthBar = GetComponent<EnemyHealthBar>();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(MaxHealth);
        }

        if (waypoints != null && waypoints.Count > 0)
        {
            transform.position = waypoints[0];
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
        else
        {
            // The enemy has reached the end of the path
            if (coreTower != null)
            {
                coreTower.TakeDamage(damageToCore);
            }
            EntitySummoner.RemoveEnemy(this);
        }
    }
}