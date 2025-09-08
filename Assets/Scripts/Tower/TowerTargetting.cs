using System.Collections.Generic;
using UnityEngine;

public static class TowerTargetting
{
    public enum TargetType
    {
        First,
        Last,
        Close,
    }

    public static EnemyMovement GetTarget(TowerBehaviour currentTower, TargetType TargetMethod)
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(currentTower.transform.position, currentTower.Range, currentTower.EnemiesLayer);
        if (enemiesInRange.Length == 0) return null;

        EnemyMovement bestTarget = null;
        float bestValue = (TargetMethod == TargetType.Last) ? Mathf.NegativeInfinity : Mathf.Infinity;

        foreach (Collider collider in enemiesInRange)
        {
            EnemyMovement currentEnemy = collider.GetComponent<EnemyMovement>();
            if (currentEnemy == null) continue;

            float currentValue = 0;
            switch (TargetMethod)
            {
                case TargetType.First:
                    currentValue = GetDistanceToEnd(currentEnemy);
                    if (currentValue < bestValue)
                    {
                        bestValue = currentValue;
                        bestTarget = currentEnemy;
                    }
                    break;
                case TargetType.Last:
                    currentValue = GetDistanceToEnd(currentEnemy);
                    if (currentValue > bestValue)
                    {
                        bestValue = currentValue;
                        bestTarget = currentEnemy;
                    }
                    break;
                case TargetType.Close:
                    currentValue = Vector3.Distance(currentTower.transform.position, currentEnemy.transform.position);
                    if (currentValue < bestValue)
                    {
                        bestValue = currentValue;
                        bestTarget = currentEnemy;
                    }
                    break;
            }
        }
        return bestTarget;
    }

    public static EnemyMovement GetTarget(CoreTower coreTower, TargetType TargetMethod)
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(coreTower.transform.position, coreTower.Range, coreTower.EnemiesLayer);
        if (enemiesInRange.Length == 0) return null;

        EnemyMovement bestTarget = null;
        float bestValue = (TargetMethod == TargetType.Last) ? Mathf.NegativeInfinity : Mathf.Infinity;

        foreach (Collider collider in enemiesInRange)
        {
            EnemyMovement currentEnemy = collider.GetComponent<EnemyMovement>();
            if (currentEnemy == null) continue;

            float currentValue = 0;
            switch (TargetMethod)
            {
                case TargetType.First:
                    currentValue = GetDistanceToEnd(currentEnemy);
                    if (currentValue < bestValue)
                    {
                        bestValue = currentValue;
                        bestTarget = currentEnemy;
                    }
                    break;
                case TargetType.Last:
                    currentValue = GetDistanceToEnd(currentEnemy);
                    if (currentValue > bestValue)
                    {
                        bestValue = currentValue;
                        bestTarget = currentEnemy;
                    }
                    break;
                case TargetType.Close:
                    currentValue = Vector3.Distance(coreTower.transform.position, currentEnemy.transform.position);
                    if (currentValue < bestValue)
                    {
                        bestValue = currentValue;
                        bestTarget = currentEnemy;
                    }
                    break;
            }
        }
        return bestTarget;
    }

    private static float GetDistanceToEnd(EnemyMovement enemy)
    {
        // This method has been updated
        // You'll need to pass the Enemy's NodeIndex and the global waypoint data
        // For a simple fix, we'll use a direct distance check, as the global waypoint data is no longer accessible here.
        // A better long-term solution would be to refactor how path data is stored.
        return Vector3.Distance(enemy.transform.position, GameObject.FindFirstObjectByType<CoreTower>().transform.position);
    }
}