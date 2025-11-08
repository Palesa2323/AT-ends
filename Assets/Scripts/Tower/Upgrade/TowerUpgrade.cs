using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;

    [Header("Upgrade Data")]
    public TowerUpgradeData upgradeData;

    [Header("Upgrade Costs")]
    public int normalUpgradeCost = 100;
    public int bombUpgradeCost = 150;
    public int cryptoUpgradeCost = 200;

    private Dictionary<TowerType, int> towerLevels = new Dictionary<TowerType, int>();
    private List<TowerBehaviour> allTowers = new List<TowerBehaviour>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // start all at level 1
        foreach (TowerType type in System.Enum.GetValues(typeof(TowerType)))
            towerLevels[type] = 1;
    }

    public void RegisterTower(TowerBehaviour tower)
    {
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

        int cost = type switch
        {
            TowerType.Normal => normalUpgradeCost,
            TowerType.Bomb => bombUpgradeCost,
            TowerType.Crypto => cryptoUpgradeCost,
            _ => 0
        };

       
    }

    private void UpgradeExistingTowers(TowerType type)
    {
        GameObject nextPrefab = upgradeData.GetTowerPrefab(type, GetUpgradeLevel(type));

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
    }
}
