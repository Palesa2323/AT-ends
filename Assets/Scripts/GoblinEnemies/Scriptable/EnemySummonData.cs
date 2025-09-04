using UnityEngine;

[CreateAssetMenu(fileName = "New EnemySummonData", menuName = "CreateEnemySummonData")]
public class EnemySummonData : ScriptableObject
{
    public GameObject EnemyPrefab;
    public int EnemyID;
}
