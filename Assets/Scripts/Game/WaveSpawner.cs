using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    public float BaseDR_Multiplier = 1.2f;
    public float TimeBetweenWaves = 10f;
    public float MinSpawnInterval = 0.4f;
    public float MaxSpawnInterval = 1.0f;

    [Header("Enemy Power Costs")]
    public float NormalCost = 1.0f;
    public float RunnerCost = 1.5f;
    public float HealerCost = 2.5f;

    [Header("Progression")]
    public int MaxWaves = 6;

    private int currentWave = 0;
    private bool wavesFinished = false;
    private float damageTakenLastWave = 0f;

    private GameLoop gameLoop;
    private CoreTower coreTower;

    private float[] laneThreatRatings; // adaptive lane weighting

    void Start()
    {
        gameLoop = FindFirstObjectByType<GameLoop>();
        coreTower = gameLoop.coreTower;

        // If you ever add multiple waypoint parents, adjust this
        laneThreatRatings = new float[1] { 1f };

        StartCoroutine(WaveManager());
    }

    IEnumerator WaveManager()
    {
        yield return new WaitForSeconds(3f); // short start delay

        while (!wavesFinished)
        {
            currentWave++;
            Debug.Log($"[Procedural] Starting Wave {currentWave}");

            // 1. Calculate adaptive difficulty
            float totalDR = CalculateAdaptiveDifficulty();

            // 2. Distribute across lanes (right now you have only one)
            for (int lane = 0; lane < laneThreatRatings.Length; lane++)
            {
                StartCoroutine(SpawnProceduralWave(totalDR * laneThreatRatings[lane], lane));
            }

            // 3. Wait before next wave
            yield return new WaitForSeconds(TimeBetweenWaves);

            // End condition
            if (currentWave >= MaxWaves)
            {
                wavesFinished = true;
                yield return new WaitForSeconds(3f);
                if (coreTower.CurrentHealth > 0)
                    gameLoop.YouWin();
            }
        }
    }

    // ---------------- PROCEDURAL WAVE SPAWNING ----------------
    private IEnumerator SpawnProceduralWave(float budget, int lane)
    {
        Debug.Log($"[WaveSpawner] Lane {lane} DR Budget: {budget}");

        float remaining = budget;
        List<Vector3> path = new List<Vector3>(GameLoop.NodePositions);

        while (remaining > 0)
        {
            // Randomly choose enemy type based on probability
            int type = Random.Range(0, 3);
            float cost = (type == 0) ? NormalCost : (type == 1 ? RunnerCost : HealerCost);

            if (remaining - cost < 0.1f)
                break;

            EnemyMovement e = EntitySummoner.SummonEnemy(type);
            if (e != null)
            {
                e.Init(path, coreTower);
            }

            remaining -= cost;

            float waitTime = Random.Range(MinSpawnInterval, MaxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    // ---------------- ADAPTIVE DIFFICULTY SYSTEM ----------------
    private float CalculateAdaptiveDifficulty()
    {
        float baseDR = currentWave * BaseDR_Multiplier;
        float adaptiveDR = 0f;

        float healthRatio = coreTower.CurrentHealth / coreTower.MaxHealth;

        // If player is doing great (core health high)
        if (healthRatio > 0.8f)
        {
            adaptiveDR += baseDR * 0.15f; // make it harder
        }

        // If player took a beating last wave
        if (damageTakenLastWave > 10f)
        {
            adaptiveDR -= baseDR * 0.25f; // give them a breather
        }

        // If player has many towers, spawn more healers to counter
        int towerCount = GameLoop.TowersInGame.Count;
        if (towerCount > 5)
        {
            adaptiveDR += towerCount * 0.3f;
        }

        float totalDR = baseDR + adaptiveDR;
        totalDR = Mathf.Max(totalDR, 5f); // minimum challenge floor

        Debug.Log($"[Adaptive DR] Wave {currentWave}: Base={baseDR:F1}, Adaptive={adaptiveDR:F1}, Total={totalDR:F1}");

        // Reset damage tracker for next round
        damageTakenLastWave = 0f;
        return totalDR;
    }

    // ---------------- PERFORMANCE FEEDBACK ----------------
    public void RecordDamage(float amount)
    {
        damageTakenLastWave += amount;
    }

    public void RecordLaneBreach(int laneIndex)
    {
        if (laneIndex >= laneThreatRatings.Length) return;
        laneThreatRatings[laneIndex] += 0.5f;
        laneThreatRatings[laneIndex] = Mathf.Min(laneThreatRatings[laneIndex], 5f);
    }
}
