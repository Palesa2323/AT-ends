using UnityEngine;

public class TowerUpgrade : MonoBehaviour
{
    [Header("Upgrade Settings")]
    public GameObject upgradeLevel2Prefab;
    public GameObject upgradeLevel3Prefab;

    public int upgrade1Cost = 75;
    public int upgrade2Cost = 150;

    private int currentLevel = 1;
    private GameLoop gameLoop;

    void Start()
    {
        gameLoop = FindFirstObjectByType<GameLoop>();
    }

    public void TryUpgrade()
    {
        if (currentLevel == 1 && upgradeLevel2Prefab != null)
        {
            AttemptUpgrade(upgrade1Cost, upgradeLevel2Prefab);
        }
        else if (currentLevel == 2 && upgradeLevel3Prefab != null)
        {
            AttemptUpgrade(upgrade2Cost, upgradeLevel3Prefab);
        }
        else
        {
            Debug.Log("Tower already maxed out or missing upgrade prefab!");
        }
    }

    private void AttemptUpgrade(int cost, GameObject nextPrefab)
    {
        if (GameLoop.Resources < cost)
        {
            Debug.Log("Not enough resources to upgrade!");
            return;
        }

        gameLoop.DeductCost(cost);
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        // Destroy old tower
        Destroy(gameObject);

        // Spawn upgraded tower
        GameObject newTower = Instantiate(nextPrefab, pos, rot);
        Debug.Log($"Tower upgraded to {newTower.name}!");
    }
}
