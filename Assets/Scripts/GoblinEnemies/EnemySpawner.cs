using UnityEngine;
using UnityEngine.AI; // Make sure this using statement is still included

public class EnemySpawner : MonoBehaviour
{
    public GameObject goblinPrefab;
    public float spawnRate = 2f;
    private float nextSpawnTime;

    void Start()
    {
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
        // We now simply instantiate the goblin at the spawner's position.
        // The goblin's NavMeshAgent will automatically find its own way
        // to the Nexus or a Sentry Tower.
        GameObject newGoblin = Instantiate(goblinPrefab, transform.position, Quaternion.identity);
    }
}
