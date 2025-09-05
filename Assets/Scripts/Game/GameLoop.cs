using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    private MeshGenerator meshGenerator;
    public bool LoopShouldEnd = false;

    void Start()
    {
        EntitySummoner.Init();
        meshGenerator = FindFirstObjectByType<MeshGenerator>();

        if (meshGenerator == null)
        {
            Debug.LogError("MeshGenerator not found in the scene.");
            return;
        }

        StartCoroutine(WaveManager());
    }

    IEnumerator WaveManager()
    {
        while (!LoopShouldEnd)
        {
            if (meshGenerator.enemyPaths.Count > 0)
            {
                // 1. Select a random path from the list
                int randomIndex = Random.Range(0, meshGenerator.enemyPaths.Count);
                List<Vector3> selectedPath = meshGenerator.enemyPaths[randomIndex].waypoints;

                // 2. Summon the enemy using the EntitySummoner
                EnemyMovement newEnemy = EntitySummoner.SummonEnemy(0);

                // 3. Pass the selected path to the enemy's Init method
                if (newEnemy != null)
                {
                    newEnemy.Init(selectedPath);
                }
            }

            yield return new WaitForSeconds(1f); // Wait for 1 second before summoning the next enemy
        }
    }
}

// This method is obsolete and commented out
/*
IEnumerator GameLoopM()
{
    while (!LoopShouldEnd)
    {
        if (enemyIDsToSummon.Count > 0)
        {
            EntitySummoner.SummonEnemy(enemyIDsToSummon.Dequeue());
        }
        yield return null; 
    }
}
*/
