using UnityEngine;

public class RunnerEnemy : EnemyMovement
{
    void Awake()
    {
        // Runner-specific attributes
        enemyType = EnemyType.Runner;
        MaxHealth = 60f;
        Speed = 3.5f;
        resourcesToAward = 5;
        damageToCore = 10f;
    }
}

