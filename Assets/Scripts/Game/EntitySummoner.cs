using System.Collections.Generic;
using UnityEngine;

public class EntitySummoner : MonoBehaviour
{
    // Keeps track of active enemies
    public static List<EnemyMovement> EnemiesInGame = new List<EnemyMovement>();

    // Stores enemy prefab references by ID
    public static Dictionary<int, GameObject> EnemyPrefabs = new Dictionary<int, GameObject>();

    // Object pools for reusing inactive enemies
    public static Dictionary<int, Queue<EnemyMovement>> EnemyObjectPools = new Dictionary<int, Queue<EnemyMovement>>();

    private static bool IsInitialized = false;
    private static EntitySummoner instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Initializes all enemy prefabs and pools
    public static void Init()
    {
        if (IsInitialized)
        {
            Debug.LogWarning("EntitySummoner is already initialized.");
            return;
        }

        // Load all ScriptableObjects from Resources/Enemies
        EnemySummonData[] Enemies = Resources.LoadAll<EnemySummonData>("Enemies");

        if (Enemies.Length == 0)
        {
            Debug.LogError("No EnemySummonData found in Resources/Enemies. Please create them and assign prefabs.");
            return;
        }

        // Register prefabs and create pools
        foreach (EnemySummonData enemy in Enemies)
        {
            if (enemy == null || enemy.EnemyPrefab == null)
            {
                Debug.LogError($"EnemySummonData with ID {enemy?.EnemyID} has a missing prefab reference!");
                continue;
            }

            if (!EnemyPrefabs.ContainsKey(enemy.EnemyID))
            {
                EnemyPrefabs.Add(enemy.EnemyID, enemy.EnemyPrefab);
                EnemyObjectPools.Add(enemy.EnemyID, new Queue<EnemyMovement>());
            }
        }

        // Pre-populate each pool with a few instances
        foreach (var enemyData in Enemies)
        {
            PrepopulatePool(enemyData.EnemyID, enemyData.PoolSize);
        }

        IsInitialized = true;
        Debug.Log($"EntitySummoner initialized {Enemies.Length} enemy types successfully.");
    }

    // Preload a few instances of each enemy type
    private static void PrepopulatePool(int enemyID, int count)
    {
        if (!EnemyPrefabs.ContainsKey(enemyID)) return;

        for (int i = 0; i < count; i++)
        {
            GameObject newEnemy = Instantiate(EnemyPrefabs[enemyID]);
            EnemyMovement enemyComponent = newEnemy.GetComponent<EnemyMovement>();

            if (enemyComponent != null)
            {
                enemyComponent.ID = enemyID;
                newEnemy.SetActive(false);
                EnemyObjectPools[enemyID].Enqueue(enemyComponent);
            }
            else
            {
                Debug.LogError($"Prefab with ID {enemyID} is missing an EnemyMovement component. Check prefab setup.");
                Destroy(newEnemy);
            }
        }
    }

    // Spawns (or reuses) an enemy based on ID
    public static EnemyMovement SummonEnemy(int EnemyID)
    {
        if (!EnemyPrefabs.ContainsKey(EnemyID))
        {
            Debug.LogError($"Enemy ID {EnemyID} not found in EnemyPrefabs. Check your EnemySummonData assets.");
            return null;
        }

        EnemyMovement summonedEnemy = null;
        Queue<EnemyMovement> referencedQueue = EnemyObjectPools[EnemyID];

        // Try to reuse from pool
        if (referencedQueue.Count > 0)
        {
            summonedEnemy = referencedQueue.Dequeue();
            summonedEnemy.gameObject.SetActive(true);
        }
        else
        {
            // Otherwise instantiate a new one
            GameObject newEnemy = Instantiate(EnemyPrefabs[EnemyID]);
            summonedEnemy = newEnemy.GetComponent<EnemyMovement>();

            if (summonedEnemy == null)
            {
                Debug.LogError($"Enemy prefab for ID {EnemyID} is missing EnemyMovement script.");
                Destroy(newEnemy);
                return null;
            }
        }

        summonedEnemy.ID = EnemyID;
        EnemiesInGame.Add(summonedEnemy);
        return summonedEnemy;
    }

    // Deactivates an enemy and returns it to the pool
    public static void RemoveEnemy(EnemyMovement enemy)
    {
        if (enemy == null) return;

        int id = enemy.ID;

        if (EnemiesInGame.Contains(enemy))
            EnemiesInGame.Remove(enemy);

        if (EnemyObjectPools.ContainsKey(id))
        {
            enemy.gameObject.SetActive(false);
            EnemyObjectPools[id].Enqueue(enemy);
        }
        else
        {
            Destroy(enemy.gameObject);
        }
    }
}
