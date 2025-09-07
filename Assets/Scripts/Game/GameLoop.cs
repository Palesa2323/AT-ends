using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    // Set this in the Inspector
    public Transform NodeParent;

    // These are used by TowerTargetting
    public static Vector3[] NodePositions;
    public static float[] NodeDistance;
    public static List<TowerBehaviour> TowersInGame;

    private MeshGenerator meshGenerator;
    public bool LoopShouldEnd;

    void Start()
    {
        // Initialize Towers list
        TowersInGame = new List<TowerBehaviour>();

        // Initialize enemies
        EntitySummoner.Init();

        if (NodeParent != null)
        {
            NodePositions = new Vector3[NodeParent.childCount];
            for (int i = 0; i < NodePositions.Length; i++)
            {
                NodePositions[i] = NodeParent.GetChild(i).position;
            }

            NodeDistance = new float[NodePositions.Length - 1];
            for (int i = 0; i < NodeDistance.Length; i++)
            {
                NodeDistance[i] = Vector3.Distance(NodePositions[i], NodePositions[i + 1]);
            }
        }
        else
        {
            Debug.LogError("NodeParent not set in the Inspector. Cannot calculate node positions.");
        }

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
