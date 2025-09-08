using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameLoop : MonoBehaviour
{
    public Transform NodeParent;
    
    private TMPro.TextMeshProUGUI resourceText;
    public GameObject gameOverPanel;
    public GameObject winPanel; // Add a new public variable for the win screen
    
    public static int Resources = 100;
    
    public CoreTower coreTower;
    
    public static Vector3[] NodePositions;
    public static float[] NodeDistance;
    public static List<TowerBehaviour> TowersInGame;

    private MeshGenerator meshGenerator;
    public int maxEnemiesToSpawn = 50; // The total number of enemies to spawn
    private int enemiesSpawned = 0; // Tracks the number of enemies spawned

    void Start()
    {
        resourceText = FindFirstObjectByType<TMPro.TextMeshProUGUI>();
        if (resourceText == null)
        {
            Debug.LogError("No TextMeshProUGUI component found for resources.");
        }

        TowersInGame = new List<TowerBehaviour>();
        EntitySummoner.Init();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        if (NodeParent != null)
        {
            NodePositions = new Vector3[NodeParent.childCount];
            for (int i = 0; i < NodePositions.Length; i++)
            {
                NodePositions[i] = NodeParent.GetChild(i).position;
            }

            if (coreTower != null && NodePositions.Length > 0)
            {
                coreTower.transform.position = NodePositions[NodePositions.Length - 1];
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

        UpdateResourceUI();
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
        UpdateResourceUI();
    }
    
    public void DeductCost(int amount)
    {
        Resources -= amount;
        UpdateResourceUI();
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void YouWin()
    {
        Time.timeScale = 0;
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
    }

    IEnumerator WaveManager()
    {
        while (enemiesSpawned < maxEnemiesToSpawn)
        {
            if (meshGenerator.enemyPaths.Count > 0)
            {
                int randomIndex = Random.Range(0, meshGenerator.enemyPaths.Count);
                List<Vector3> selectedPath = meshGenerator.enemyPaths[randomIndex].waypoints;

                EnemyMovement newEnemy = EntitySummoner.SummonEnemy(0);

                if (newEnemy != null)
                {
                    newEnemy.Init(selectedPath, coreTower);
                    enemiesSpawned++; // Increment the counter
                }
            }
            yield return new WaitForSeconds(1f);
        }
        
        // After all enemies have been spawned, check if the core tower is still alive
        if (coreTower.CurrentHealth > 0)
        {
            YouWin();
        }
    }
}