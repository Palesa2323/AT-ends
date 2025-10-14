using UnityEngine;
using System.Collections.Generic;

// Defines the structure for a tactical squad to be assigned in the Inspector
[System.Serializable]
public struct EnemySquad
{
    public string Name;
    [Tooltip("Number of Normal enemies in this squad.")]
    public int NormalCount;
    [Tooltip("Number of Runner enemies in this squad.")]
    public int RunnerCount;
    [Tooltip("Number of Healer enemies in this squad.")]
    public int HealerCount;
    [HideInInspector] public float TotalCost;
}

public static class EnemyData
{
    // Define the Power Cost for each enemy type (TEP calculation)
    public const float Cost_Normal = 1.0f;
    public const float Cost_Runner = 1.5f;
    public const float Cost_Healer = 2.5f;

    // Helper method to calculate the cost of a squad
    public static float CalculateSquadCost(EnemySquad squad)
    {
        return (squad.NormalCount * Cost_Normal) + (squad.RunnerCount * Cost_Runner) + (squad.HealerCount * Cost_Healer);
    }
}
