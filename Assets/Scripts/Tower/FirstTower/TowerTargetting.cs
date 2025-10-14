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

    /*public static EnemyMovement GetTarget(CoreTower coreTower, TargetType TargetMethod)
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
    }*/

    // In TowerTargetting.cs

    private static float GetDistanceToEnd(EnemyMovement enemy)
    {
        // Try to get the CoreTower reference from the enemy.
        // This is a much safer way to find the end point.
        CoreTower core = enemy.GetComponent<EnemyMovement>().CoreTower; // Assuming CoreTower is exposed via a public property/field in EnemyMovement

        // Fallback if the reference is null (or if you haven't exposed CoreTower in EnemyMovement)
        if (core == null)
        {
            core = GameObject.FindFirstObjectByType<CoreTower>();
        }

        if (core != null)
        {
            // Calculate the straight-line distance to the Core Tower
            return Vector3.Distance(enemy.transform.position, core.transform.position);
        }

        // Should not happen if a CoreTower exists
        return Mathf.Infinity;
    }
}