using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    // Make this a public variable to set in the Inspector
    public Transform NodeParent;

    // The rest of your variables
    private MeshGenerator meshGenerator;
    public bool LoopShouldEnd = false;

    void Start()
    {
        // TowersInGame list is not used in this script, so you can remove it if not needed.
        // TowersInGame = new List<TowerBehaviour>(); 

        EntitySummoner.Init();

        // The following lines had errors and are not currently used in the WaveManager.
        // They are commented out to prevent errors until you need them.
        /*
        Vector3[] NodePositions = new Vector3[NodeParent.childCount];
        for(int i = 0; i < NodePositions.Length; i++)
        {
            NodePositions[i] = NodeParent.GetChild(i).position;
        }

        float[] NodeDistance = new float[NodePositions.Length - 1];
        for (int i = 0; i < NodeDistance.Length; i++)
        {
            NodeDistance[i] = Vector3.Distance(NodePositions[i], NodePositions[i + 1]);
        }
        */

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
                int randomIndex = Random.Range(0, meshGenerator.enemyPaths.Count);
                List<Vector3> selectedPath = meshGenerator.enemyPaths[randomIndex].waypoints;

                EnemyMovement newEnemy = EntitySummoner.SummonEnemy(0);

                if (newEnemy != null)
                {
                    newEnemy.Init(selectedPath);
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
