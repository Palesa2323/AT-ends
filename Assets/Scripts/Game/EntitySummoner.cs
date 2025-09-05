using System.Collections.Generic;
using UnityEngine;

public class EntitySummoner : MonoBehaviour
{
    public static List<EnemyMovement> EnemiesInGame = new List<EnemyMovement>();
    public static Dictionary<int, GameObject> EnemyPrefabs = new Dictionary<int, GameObject>();
    public static Dictionary<int, Queue<EnemyMovement>> EnemyObjectPools = new Dictionary<int, Queue<EnemyMovement>>();

    private static bool IsInitialized = false;

    // An instance reference to call coroutines from outside
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

    public static void Init()
    {
        if (!IsInitialized)
        {
            // The path for your ScriptableObjects.
            EnemySummonData[] Enemies = Resources.LoadAll<EnemySummonData>("Enemies");

            if (Enemies.Length == 0)
            {
                Debug.LogError("No EnemySummonData found in Resources/Enemies. Please create a folder and place your ScriptableObjects there.");
            }

            foreach (EnemySummonData enemy in Enemies)
            {
                if (!EnemyPrefabs.ContainsKey(enemy.EnemyID))
                {
                    EnemyPrefabs.Add(enemy.EnemyID, enemy.EnemyPrefab);
                    EnemyObjectPools.Add(enemy.EnemyID, new Queue<EnemyMovement>());
                }
            }
            IsInitialized = true;
            // NEW: Pre-populate the pools
            foreach (var enemyData in Enemies)
            {
                PrepopulatePool(enemyData.EnemyID, 100); // Change 10 to a desired starting number
            }
        }
        else
        {
            Debug.LogWarning("EntitySummoner is already initialized.");
        }
    }

    // NEW: Method to create and fill the pools
    private static void PrepopulatePool(int enemyID, int count)
    {
        if (EnemyPrefabs.ContainsKey(enemyID))
        {
            for (int i = 0; i < count; i++)
            {
                GameObject newEnemy = Instantiate(EnemyPrefabs[enemyID]);
                EnemyMovement enemyComponent = newEnemy.GetComponent<EnemyMovement>();
                if (enemyComponent != null)
                {
                    enemyComponent.ID = enemyID; // Set the ID for the pooled enemy
                    enemyComponent.gameObject.SetActive(false);
                    EnemyObjectPools[enemyID].Enqueue(enemyComponent);
                }
                else
                {
                    Destroy(newEnemy);
                }
            }
        }
    }

    public static EnemyMovement SummonEnemy(int EnemyID)
    {
        if (EnemyPrefabs.ContainsKey(EnemyID))
        {
            EnemyMovement summonedEnemy = null;
            Queue<EnemyMovement> referencedQueue = EnemyObjectPools[EnemyID];

            if (referencedQueue.Count > 0)
            {
                summonedEnemy = referencedQueue.Dequeue();
                summonedEnemy.gameObject.SetActive(true);
            }
            else
            {
                GameObject newEnemy = Instantiate(EnemyPrefabs[EnemyID]);
                summonedEnemy = newEnemy.GetComponent<EnemyMovement>();
                if (summonedEnemy == null)
                {
                    Debug.LogError("The instantiated prefab does not have an EnemyMovement component.");
                    return null;
                }
            }

            // Set the ID, Initialize stats, add to list, and return
            summonedEnemy.ID = EnemyID;
            EnemiesInGame.Add(summonedEnemy);
            return summonedEnemy;
        }
        else
        {
            Debug.LogError($"Enemy ID {EnemyID} not found in EnemyPrefabs.");
            return null; // Return null on failure
        }
    }

    public static void RemoveEnemy(EnemyMovement EnemyToRemove)
    {
        if (EnemyToRemove != null && EnemyObjectPools.ContainsKey(EnemyToRemove.ID))
        {
            EnemyObjectPools[EnemyToRemove.ID].Enqueue(EnemyToRemove);
            EnemyToRemove.gameObject.SetActive(false);
            EnemiesInGame.Remove(EnemyToRemove); // This is crucial for tracking
        }
    }
}