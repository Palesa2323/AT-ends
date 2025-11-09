using UnityEngine;
public static class StatMutator
{
    private const float BASE_HEALTH = 10f;
    private const float BASE_SPEED = 3.5f;
    private const float BASE_DAMAGE = 5f;
    private const float BASE_REWARD = 2f;
    public struct MutatedStats
    {
        public float Health;
        public float Speed;
        public float Damage;
        public float Reward;
        public Color VisualColor;
    }

    public static MutatedStats GenerateMutation()
    {
        float roll = Random.Range(0f, 1f);
        float multiplier; // This will define the min/max multiplier range

        if (roll < 0.05f) // 5% chance
        {
            multiplier = Random.Range(0.7f, 1.3f);
        }
        else if (roll < 0.30f) // 25% chance
        {
            // Tier 2 (Significant): Multiplier between 0.8 and 1.2 (±20%)
            multiplier = Random.Range(0.8f, 1.2f);
        }
        else // 70% chance (Tier 1: Subtle)
        {
            // Tier 1 (Subtle): Multiplier between 0.9 and 1.1 (±10%)
            multiplier = Random.Range(0.9f, 1.1f);
        }

        // Ensure the multiplier is not too close to 1.0 to guarantee a change
        if (multiplier > 0.98f && multiplier < 1.02f)
        {
            // If the roll was too close to normal, push it slightly toward an increase or decrease
            multiplier = (Random.Range(0, 2) == 0) ? 1.02f : 0.98f;
        }

        // 2. Apply Multiplier to Stats
        MutatedStats stats = new MutatedStats
        {
            Health = BASE_HEALTH * multiplier,
            Speed = BASE_SPEED * multiplier,
            Damage = BASE_DAMAGE * multiplier,
            // Reward is also randomized, providing a payoff for defeating tougher enemies
            Reward = BASE_REWARD * multiplier
        };

        // 3. Determine Visual Color based on Multiplier
        stats.VisualColor = GetColorFromMultiplier(multiplier);

        Debug.Log($"Goblin Mutated! Multiplier: {multiplier:F2}. Health: {stats.Health:F1}, Speed: {stats.Speed:F2}");
        return stats;
    }

    /// <summary>
    /// Maps the mutation strength/direction to a visual color cue.
    /// </summary>
    private static Color GetColorFromMultiplier(float multiplier)
    {
        if (multiplier > 1.2f) return new Color(1f, 0.2f, 0.2f);       // Strong Red (Major Boost)
        if (multiplier > 1.0f) return new Color(1f, 0.6f, 0.2f);       // Orange (Minor Boost)
        if (multiplier < 0.8f) return new Color(0.2f, 0.2f, 1f);       // Strong Blue (Major Weakness)
        if (multiplier < 1.0f) return new Color(0.5f, 0.5f, 0.8f);       // Light Blue (Minor Weakness)

        return new Color(0.2f, 0.8f, 0.2f); // Green/Default (Neutral/Subtle Change)
    }
}