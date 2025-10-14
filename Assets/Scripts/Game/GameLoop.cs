using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameLoop : MonoBehaviour
{
    public Transform NodeParent;

    public TextMeshProUGUI resourceText;
    public GameObject gameOverPanel;
    public GameObject winPanel;

    public static int Resources = 100;

    public CoreTower coreTower;

    public static Vector3[] NodePositions;
    public static float[] NodeDistance;
    public static List<TowerBehaviour> TowersInGame;

    private MeshGenerator meshGenerator;

    // Track total enemies across all waves
    private int totalEnemiesSpawned;
    private bool wavesFinished;


    [Header("Wave Management")]
    public int CurrentWave = 0;
    public float BaseDR_Multiplier = 1.2f;

    [Header("Lane Management")]
    public Transform[] SpawnLocations; // Assign your three spawn points here
    private float[] laneThreatRatings; // LTR for Blue, Red, Green lanes
    private float damageTakenLastInterval = 0f;

    void Start()
    {
        resourceText = FindFirstObjectByType<TextMeshProUGUI>();
        if (resourceText == null)
        {
            Debug.LogError("No TextMeshProUGUI component found for resources.");
        }

        TowersInGame = new List<TowerBehaviour>();
        EntitySummoner.Init(); // initializes all enemies via ScriptableObjects

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

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
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void YouWin()
    {
        Time.timeScale = 0;
        if (winPanel != null) winPanel.SetActive(true);
    }

    // --- MULTI-TYPE WAVE SYSTEM ---
    IEnumerator WaveManager()
    {
        // --- Wave 1 ---
        yield return StartCoroutine(SpawnWave(new (int id, int count, float interval)[]
        {
            (0, 8, 0.6f), // 8 Normal enemies
            (1, 5, 0.5f)  // 5 Runners
        }));

        yield return new WaitForSeconds(5f); // small pause between waves

        // --- Wave 2 ---
        yield return StartCoroutine(SpawnWave(new (int id, int count, float interval)[]
        {
            (1, 8, 0.45f), // 8 Runners
            (2, 3, 1.0f)   // 3 Healers
        }));

        yield return new WaitForSeconds(5f);

        // --- Wave 3 (mixed finale) ---
        yield return StartCoroutine(SpawnWave(new (int id, int count, float interval)[]
        {
            (0, 6, 0.6f),
            (1, 6, 0.5f),
            (2, 4, 1.0f)
        }));

        wavesFinished = true;

        // When waves are done, check if you survived
        yield return new WaitForSeconds(5f);
        if (coreTower.CurrentHealth > 0)
        {
            YouWin();
        }
    }

    IEnumerator SpawnWave((int id, int count, float interval)[] entries)
    {
        foreach (var entry in entries)
        {
            for (int i = 0; i < entry.count; i++)
            {
                if (meshGenerator.enemyPaths.Count > 0)
                {
                    int randomIndex = Random.Range(0, meshGenerator.enemyPaths.Count);
                    List<Vector3> selectedPath = meshGenerator.enemyPaths[randomIndex].waypoints;

                    EnemyMovement newEnemy = EntitySummoner.SummonEnemy(entry.id);
                    if (newEnemy != null)
                    {
                        newEnemy.Init(selectedPath, coreTower);
                        totalEnemiesSpawned++;
                    }
                }

                yield return new WaitForSeconds(entry.interval);
            }
        }
    }


}
