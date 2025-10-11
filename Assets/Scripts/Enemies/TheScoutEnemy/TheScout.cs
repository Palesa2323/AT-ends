using UnityEngine;

public class HealerEnemy : EnemyMovement
{
    void Awake()
    {
        // Healer-specific attributes
        enemyType = EnemyType.Healer;
        MaxHealth = 150f;
        Speed = 1.0f;
        HealRadius = 6f;
        HealAmount = 12f;
        resourcesToAward = 15;
        damageToCore = 0f; // Healers don't attack the core
    }
}

