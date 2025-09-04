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
        if (IsInitialized)
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

   /*ublic static EnemyMovement SummonEnemy(int enemyID, Vector3 position, Quaternion rotation)
    {
        EnemyMovement SummonedEnemy = null;

        if (EnemyPrefabs.ContainsKey(enemyID))
        {

        }
        else
        {
            Debug.LogError($"Enemy ID {enemyID} not found in EnemyPrefabs.");
        }
    }*/
}
