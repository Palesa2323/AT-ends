using UnityEngine;

[CreateAssetMenu(fileName = "TowerUpgradeData", menuName = "Tower/Tower Upgrade Data")]
public class TowerUpgradeData : ScriptableObject
{
    [System.Serializable]
    public class TowerLevelData
    {
        public TowerType towerType;
        public TowerData[] levels; // Index 0 = level 1, index 1 = level 2, etc.
    }

    public TowerLevelData[] allTowerLevels;

    public TowerData GetTowerData(TowerType type, int level)
    {
        foreach (var t in allTowerLevels)
        {
            if (t.towerType == type)
            {
                if (level - 1 < t.levels.Length)
                    return t.levels[level - 1];
                else
                    return null; // max level
            }
        }
        return null;
    }
}
public enum TowerType
{
    Normal,
    Bomb,
    Crypto
}

