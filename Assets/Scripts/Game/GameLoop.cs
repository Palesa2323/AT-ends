using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
public class GameLoop : MonoBehaviour
{
    public Transform NodeParent;

    // UI References
    public TMPro.TextMeshPro resourceText;
    public GameObject gameOverPanel;

    public static int Resources = 100;

    public CoreTower coreTower;

    public static Vector3[] NodePositions;
    public static float[] NodeDistance;
    public static List<TowerBehaviour> TowersInGame;

    private MeshGenerator meshGenerator;

    void Start()
    {
        TowersInGame = new List<TowerBehaviour>();
        EntitySummoner.Init();

        // Ensure the game over panel is hidden at the start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

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

        UpdateResourceUI(); // Update UI at the start
        StartCoroutine(WaveManager());
    }

    public void UpdateResourceUI()
    {
        if (resourceText != null)
        {
            resourceText.text = "Resources: " + Resources;
        }
    }

    public void AddResources(int amount)
    {
        Resources += amount;
        UpdateResourceUI(); // Update UI when resources change
    }

    public void DeductCost(int amount)
    {
        Resources -= amount;
        UpdateResourceUI(); // Update UI when resources change
    }

    public void GameOver()
    {
        Time.timeScale = 0; // Pause the game
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    IEnumerator WaveManager()
    {
        while (true)
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