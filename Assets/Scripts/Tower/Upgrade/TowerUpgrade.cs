using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;

    [Header("Upgrade Data")]
    public TowerUpgradeData upgradeData;  // SO holding all tower prefabs for upgrades

    [Header("Upgrade Costs")]
    public int normalUpgradeCost = 100;
    public int bombUpgradeCost = 150;
    public int cryptoUpgradeCost = 200;
    public int playerResources = 500; // starting resources

    private Dictionary<TowerType, int> towerLevels = new Dictionary<TowerType, int>();
    private List<TowerBehaviour> allTowers = new List<TowerBehaviour>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // start all tower types at level 1
        foreach (TowerType type in System.Enum.GetValues(typeof(TowerType)))
            towerLevels[type] = 1;
    }

    public void RegisterTower(TowerBehaviour tower)
    {
        if (!allTowers.Contains(tower))
            allTowers.Add(tower);
    }

    public void UnregisterTower(TowerBehaviour tower)
    {
        allTowers.Remove(tower);
    }

    public int GetUpgradeLevel(TowerType type)
    {
        return towerLevels.ContainsKey(type) ? towerLevels[type] : 1;
    }

    public void TryUpgradeTowerType(TowerType type)
    {
        int currentLevel = GetUpgradeLevel(type);
        if (currentLevel >= 3)
        {
            Debug.Log($"{type} towers are maxed out!");
            return;
        }

        // Determine cost
        int cost = type switch
        {
            TowerType.Normal => normalUpgradeCost,
            TowerType.Bomb => bombUpgradeCost,
            TowerType.Crypto => cryptoUpgradeCost,
            _ => 0
        };

        if (!SpendResources(cost))
            return;

        // Increment global upgrade level
        towerLevels[type] = currentLevel + 1;
        Debug.Log($"{type} towers upgraded to Level {towerLevels[type]}!");

        // Upgrade all existing towers of this type
        UpgradeExistingTowers(type);
    }

    public bool SpendResources(int amount)
    {
        if (playerResources >= amount)
        {
            playerResources -= amount;
            Debug.Log($"Spent {amount} resources. Remaining: {playerResources}");
            return true;
        }
        else
        {
            Debug.Log("Not enough resources!");
            return false;
        }
    }

    public void AddResources(int amount)
    {
        playerResources += amount;
        Debug.Log($"Added {amount} resources. Total: {playerResources}");
    }

    public void UpgradeExistingTowers(TowerType type)
    {
        TowerData data = upgradeData.GetTowerData(type, GetUpgradeLevel(type));
        if (data == null)
        {
            Debug.Log($"{type} tower is already max level or missing data!");
            return;
        }

        GameObject nextPrefab = data.towerPrefab;

        foreach (var tower in allTowers.ToArray())
        {
            if (tower.towerType == type)
            {
                Vector3 pos = tower.transform.position;
                Quaternion rot = tower.transform.rotation;

                UnregisterTower(tower);
                Destroy(tower.gameObject);

                GameObject newTower = Instantiate(nextPrefab, pos, rot);
                TowerBehaviour newTowerComp = newTower.GetComponent<TowerBehaviour>();
                RegisterTower(newTowerComp);
            }
        }
            // Increment the level
        towerLevels[type]++;
    }

}
