using System.Collections.Generic;
using UnityEngine;

public class EntitySummoner : MonoBehaviour
{
    public static List<EnemyMovement> EnemiesInGame;
    public static Dictionary<int, GameObject> EnemyPrefabs;
    public static Dictionary<int, Queue<EnemyMovement>> EnemyObjectPools;

    private static bool IsInitialized = false;

    public static void Init()
    {
        if (!IsInitialized) // FIX: was wrong way around
        {
            EnemyPrefabs = new Dictionary<int, GameObject>();
            EnemyObjectPools = new Dictionary<int, Queue<EnemyMovement>>();
            EnemiesInGame = new List<EnemyMovement>();

            EnemySummonData[] Enemies = Resources.LoadAll<EnemySummonData>("Enemies");
            Debug.Log(Enemies[0].name);

            foreach (EnemySummonData enemy in Enemies)
            {
                EnemyPrefabs.Add(enemy.EnemyID, enemy.EnemyPrefab);
                EnemyObjectPools.Add(enemy.EnemyID, new Queue<EnemyMovement>());
            }

            IsInitialized = true;
        }
        else
        {
            Debug.LogWarning("EntitySummoner is already initialized.");
        }
    }

    public static EnemyMovement SummonEnemy(int EnemyID)
    {
        EnemyMovement SummonedEnemy = null;

        if (EnemyPrefabs.ContainsKey(EnemyID))
        {
            Queue<EnemyMovement> ReferencedQueue = EnemyObjectPools[EnemyID];

            if (ReferencedQueue.Count > 0)
            {
                // Dequeue Enemy and Initialize
                SummonedEnemy = ReferencedQueue.Dequeue();
                SummonedEnemy.Init();
            }
            else
            {
                // Instantiate new Enemy and Initialize
                GameObject NewEnemy = Object.Instantiate(EnemyPrefabs[EnemyID], Vector3.zero, Quaternion.identity);
                SummonedEnemy = NewEnemy.GetComponent<EnemyMovement>();
                SummonedEnemy.Init();
            }
        }
        else
        {
            Debug.LogError($"Enemy ID {EnemyID} not found in EnemyPrefabs.");
        }

        return SummonedEnemy; // FIX: you forgot to return
    }
}
