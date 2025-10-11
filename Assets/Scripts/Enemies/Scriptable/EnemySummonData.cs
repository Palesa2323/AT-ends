using UnityEngine;

[CreateAssetMenu(fileName = "New EnemySummonData", menuName = "CreateEnemySummonData")]
public class EnemySummonData : ScriptableObject
{
   public EnemyMovement EnemyPrefab;
    public int EnemyID;
    [Min(0)] public int poolSize = 10; // if you want pooling; ignore if not used
}
