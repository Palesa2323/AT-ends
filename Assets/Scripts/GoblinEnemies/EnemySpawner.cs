using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject goblinPrefab;
    public EnemyPath enemyPath; // Reference to our new EnemyPath script
    public float spawnRate = 2f;
    private float nextSpawnTime;

    void Start()
    {
        // Make sure the enemyPath reference is set in the Inspector!
        if (enemyPath == null)
        {
            Debug.LogError("EnemyPath reference not set on the EnemySpawner!");
            return;
        }

        nextSpawnTime = Time.time + 1f; // Initial delay before first spawn
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnGoblin();
            nextSpawnTime = Time.time + 1f / spawnRate;
        }
    }

    void SpawnGoblin()
    {
        // Instantiate the goblin at the first waypoint's position
        GameObject newGoblin = Instantiate(goblinPrefab, enemyPath.waypoints[0].position, Quaternion.identity);

        // Get the Goblin component and pass the entire path array to it
        Goblin goblinScript = newGoblin.GetComponent<Goblin>();
        if (goblinScript != null)
        {
            goblinScript.pathWaypoints = enemyPath.GetPath();
        }
    }
}
