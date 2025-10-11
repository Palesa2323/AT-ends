using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "TD/Enemy Database", order = 1)]
public class EnemyDatabase : ScriptableObject
{
    public List<EnemySummonData> entries = new List<EnemySummonData>();
}

