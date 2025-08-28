using UnityEngine;

public class Goblin : MonoBehaviour
{
    public float health = 20f;
    public float speed = 2f;
    public float damage = 5f;

    [HideInInspector]
    public Transform[] pathWaypoints; // Set this from your Spawner script
    private int currentWaypointIndex = 0;

    void Update()
    {
        if (currentWaypointIndex < pathWaypoints.Length)
        {
            MoveToWaypoint();
        }
        else
        {
            // Reached the end of the path
            AttackNexus();
        }
    }

    void MoveToWaypoint()
    {
        Vector3 targetPosition = pathWaypoints[currentWaypointIndex].position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if we've reached the waypoint
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentWaypointIndex++;
        }
    }

    void AttackNexus()
    {
        // This is where you would handle the attack logic for the Nexus.
        // For now, we'll just destroy the goblin to simulate it reaching the end.
        // In a more complete game, you'd get a reference to the Nexus and call Nexus.TakeDamage().
        Debug.Log("A goblin has reached the Nexus!");
        Destroy(gameObject);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            // Die and drop a "Crystal Shard"
            Debug.Log("Goblin defeated! Dropping a shard.");
            // You can call a GameManager function to add shards
            Destroy(gameObject);
        }
    }
}