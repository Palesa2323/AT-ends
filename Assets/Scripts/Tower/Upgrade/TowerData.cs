using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Tower/Tower Data")]
public class TowerData : ScriptableObject
{
    public TowerType towerType;
    public GameObject towerPrefab; // The prefab to spawn
    public Sprite icon;           // UI icon

    // Tower stats
    public float damage;
    public float fireRate;
    public float range;

    public int upgradeCost; // Cost to upgrade to this tower
}
