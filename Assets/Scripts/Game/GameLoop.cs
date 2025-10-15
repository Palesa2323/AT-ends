using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameLoop : MonoBehaviour
{
    [Header("References")]
    public Transform NodeParent;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public CoreTower coreTower;
    public WaveSpawner waveSpawner;

    [Header("Resources")]
    public static int Resources = 100;
    public TextMeshProUGUI resourceText;

    [Header("Towers")]
    public static Vector3[] NodePositions;
    public static float[] NodeDistance;
    public static List<TowerBehaviour> TowersInGame;

    private MeshGenerator meshGenerator;

    void Start()
    {
        resourceText = FindFirstObjectByType<TextMeshProUGUI>();
        if (resourceText == null)
        {
            Debug.LogError("No TextMeshProUGUI found for resources!");
        }
       
        TowersInGame = new List<TowerBehaviour>();
        EntitySummoner.Init();

        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);

        SetupNodeData();
        meshGenerator = FindFirstObjectByType<MeshGenerator>();

        UpdateResourceUI();
        Debug.Log("Shard text reference: " + (resourceText != null));

        // Find and initialize the WaveSpawner
        waveSpawner = FindFirstObjectByType<WaveSpawner>();
       // if (waveSpawner != null)
        //{
        //    waveSpawner.Init(this, meshGenerator, coreTower);
      //  }
      //  else
       // {
      //      Debug.LogError("No WaveSpawner found in the scene!");
      //  }
    }

    private void SetupNodeData()
    {
        if (NodeParent == null)
        {
            Debug.LogError("NodeParent not assigned!");
            return;
        }

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

    // ------------------- Resource Handling -------------------
    public void AddResources(int amount)
    {
        Resources += amount;
        UpdateResourceUI();
        Debug.Log($"Added {amount} resources. Total: {Resources}");
    }

    public void DeductCost(int amount)
    {
        Resources -= amount;
        if (Resources < 0) Resources = 0;
        UpdateResourceUI();
        Debug.Log($"Deducted {amount} resources. Total: {Resources}");
    }

    public void UpdateResourceUI()
    {
        if (resourceText == null)
        {
            Debug.LogError("Resource Text not assigned!");
            return;
        }

        resourceText.text = "💰 Resources: " + Resources;
        Debug.Log("Updated UI text → " + resourceText.text);

    }

    // ------------------- Game State -------------------
    public void GameOver()
    {
        Time.timeScale = 0;
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }

    public void YouWin()
    {
        Time.timeScale = 0;
        if (winPanel) winPanel.SetActive(true);
    }
}
