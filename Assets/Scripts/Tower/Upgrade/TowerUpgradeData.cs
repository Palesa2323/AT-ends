using UnityEngine;

[CreateAssetMenu(fileName = "TowerUpgradeData", menuName = "ScriptableObjects/TowerUpgradeData")]
public class TowerUpgradeData : ScriptableObject
{
    [System.Serializable]
    public class TowerLevelSet
    {
        public TowerType towerType;
        public GameObject[] levelPrefabs; // [0]=Lv1, [1]=Lv2, [2]=Lv3
    }

    public TowerLevelSet[] towerSets;

    public GameObject GetTowerPrefab(TowerType type, int level)
    {
        foreach (var set in towerSets)
        {
            if (set.towerType == type)
            {
                int index = Mathf.Clamp(level - 1, 0, set.levelPrefabs.Length - 1);
                return set.levelPrefabs[index];
            }
        }
        Debug.LogWarning($"No tower prefab found for {type} level {level}");
        return null;
    }
}
public enum TowerType
{
    Normal,
    Bomb,
    Crypto
}

