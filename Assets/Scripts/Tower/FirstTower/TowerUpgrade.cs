using System.Collections.Generic;
using UnityEngine;

public enum TowerType { Normal, Bomb, Cryo }

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;

    [Header("Upgrade Levels (global per tower type)")]
    public int normalTowerLevel = 0;
    public int bombTowerLevel = 0;
    public int cryoTowerLevel = 0;

    [Header("Upgrade Costs")]
    public int normalUpgradeCost = 100;
    public int bombUpgradeCost = 200;
    public int cryoUpgradeCost = 300;

    // registered towers currently in scene
    private readonly List<TowerBehaviour> allTowers = new List<TowerBehaviour>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // safe register/unregister
    public void RegisterTower(TowerBehaviour tower)
    {
        if (tower == null) return;
        if (!allTowers.Contains(tower)) allTowers.Add(tower);
    }

    public void UnregisterTower(TowerBehaviour tower)
    {
        if (tower == null) return;
        if (allTowers.Contains(tower)) allTowers.Remove(tower);
    }

    // Public method called by the UI
    public bool TryUpgradeTowerType(TowerType type)
    {
        int cost = GetCost(type);
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager.Instance is null — cannot check money. Upgrade aborted.");
            return false;
        }

        if (GameManager.Instance.money < cost)
        {
            Debug.Log("Not enough money to upgrade " + type);
            return false;
        }

        // Deduct cost
        GameManager.Instance.money -= cost;

        // Increase level
        IncrementLevel(type);

        // Apply effects to all towers of this type
        ApplyUpgradesToAll(type);

        Debug.Log($"{type} towers upgraded. New level: {GetLevel(type)}");
        return true;
    }

    int GetCost(TowerType type)
    {
        switch (type)
        {
            case TowerType.Normal: return normalUpgradeCost;
            case TowerType.Bomb: return bombUpgradeCost;
            case TowerType.Cryo: return cryoUpgradeCost;
            default: return 9999;
        }
    }

    void IncrementLevel(TowerType type)
    {
        switch (type)
        {
            case TowerType.Normal: normalTowerLevel++; break;
            case TowerType.Bomb: bombTowerLevel++; break;
            case TowerType.Cryo: cryoTowerLevel++; break;
        }
    }

    int GetLevel(TowerType type)
    {
        switch (type)
        {
            case TowerType.Normal: return normalTowerLevel;
            case TowerType.Bomb: return bombTowerLevel;
            case TowerType.Cryo: return cryoTowerLevel;
            default: return 0;
        }
    }

    void ApplyUpgradesToAll(TowerType type)
    {
        foreach (var t in allTowers)
        {
            if (t.towerType == type)
            {
                t.ApplyUpgradeStats(); // TowerBehaviour will read TowerManager.Instance levels and update itself
            }
        }
    }

    // Helper for UI: expose cost & level
    public int GetUpgradeCost(TowerType type) => GetCost(type);
    public int GetUpgradeLevel(TowerType type) => GetLevel(type);
}
