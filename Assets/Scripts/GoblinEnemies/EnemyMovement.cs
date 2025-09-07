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

    public void Init(List<Vector3> assignedPath)
    {
        Health = MaxHealth;
        // Assign the path to this enemy
        waypoints = assignedPath;

        rb = GetComponent<Rigidbody>();

        // Ensure the enemy is at the start of its path
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
        if (Health <= 0)
        {
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
            GameLoop.Lives--; // Reduce player lives
            // The enemy has reached the end of the path
            EntitySummoner.RemoveEnemy(this);
        }
    }
}