using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int crystalShards = 0;
    public int sentryTowerCost = 5;

    public Text shardsText;
    public Text nexusHealthText;
    public Text buildCostText;

    public Nexus nexus;
    public GameObject sentryTowerPrefab;

    private bool isBuilding = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI();
        if (buildCostText != null)
        {
            buildCostText.text = "Cost: " + sentryTowerCost.ToString();
        }
    }

    void Update()
    {
        UpdateUI();
        HandleBuildingPlacement();
    }

    void UpdateUI()
    {
        if (nexusHealthText != null)
        {
            nexusHealthText.text = "Nexus Health: " + nexus.health.ToString("F0"); // Display health as a whole number
        }
        if (shardsText != null)
        {
            shardsText.text = "Shards: " + crystalShards.ToString();
        }
    }

    public void AddShards(int amount)
    {
        crystalShards += amount;
    }

    public void StartBuildingMode()
    {
        if (crystalShards >= sentryTowerCost)
        {
            isBuilding = true;
            Debug.Log("Building mode activated. Click a valid spot to place a tower.");
        }
        else
        {
            Debug.Log("Not enough shards to build a tower!");
        }
    }

    void HandleBuildingPlacement()
    {
        if (isBuilding && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the raycast hit a valid terrain area (e.g., check its tag)
                if (hit.collider.gameObject.CompareTag("Terrain"))
                {
                    Instantiate(sentryTowerPrefab, hit.point, Quaternion.identity);
                    crystalShards -= sentryTowerCost;
                    isBuilding = false;
                }
            }
        }
    }
}