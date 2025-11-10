using UnityEngine;

[System.Serializable]
public class TowerLevelStats
{
    [Header("Upgrade Cost & Reward")]
    public int Cost; // Cost to reach this level (ignored for Level 1)

    [Header("Combat Stats")]
    public float Damage;
    public float AttackRate; // Attacks per second
    public float Range;

    [Header("Visuals")]
    // Optional: Reference a different prefab for Level 2/3 models
    public GameObject VisualPrefab;
    public string Description;
}

[CreateAssetMenu(fileName = "NewTowerConfig", menuName = "Tower Defense/Tower Config")]
public class TowerConfig : ScriptableObject
{
    public string TowerName = "New Tower";
    [Space]
    [Tooltip("Stats for the initial level.")]
    public TowerLevelStats Level1;
    [Tooltip("Stats for the first upgrade (Level 2).")]
    public TowerLevelStats Level2;
    [Tooltip("Stats for the final upgrade (Level 3).")]
    public TowerLevelStats Level3;
    public TowerLevelStats GetStats(int level)
    {
        // For simplicity, we use a switch since we only have 3 levels.
        switch (level)
        {
            case 1: return Level1;
            case 2: return Level2;
            case 3: return Level3;
            default:
                Debug.LogError($"Invalid tower level requested: {level}. Falling back to Level 1.");
                return Level1;
        }
    }
}
