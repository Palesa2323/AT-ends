using UnityEngine;

[CreateAssetMenu(fileName = "EnemySummonData", menuName = "TD/Enemy Summon Data", order = 0)]
public class EnemySummonData : ScriptableObject
{
    [Range(0, 100)] public int EnemyID;
    public GameObject EnemyPrefab;
    [Min(0)] public int PoolSize = 10;
}
