using UnityEngine;

public class SupportTower : TowerBehaviour
{
    [Header("Support Settings")]
    public float AuraRange = 8f;
    public float FireRateBoost = 0.5f; // E.g., a multiplier for the fire rate

    // Support towers don't attack enemies, so we override Update to remove targeting/attack logic.
    protected override void Update()
    {
        // 1. NO TARGETING/ROTATION LOGIC
        // We only use the timer from the base class for the buff cycle

        // 2. Buff Logic (Runs on the inherited fireTimer cycle)
        fireTimer += Time.deltaTime;
        if (fireTimer >= delay)
        {
            fireTimer = 0f;
            ApplyAuraBuff();
        }
    }

    // Since the logic is in Update(), this attack method is left empty.
    protected override void ExecuteAttack()
    {
        // Does nothing
    }

    private void ApplyAuraBuff()
    {
        // Find all TOWER COLLIDERS (not enemies) within the aura range
        // Assuming all towers are on a layer named "Tower"
        Collider[] towersInRange = Physics.OverlapSphere(transform.position, AuraRange, LayerMask.GetMask("Tower"));

        foreach (Collider col in towersInRange)
        {
            // Check for the base class so it can buff all tower types
            TowerBehaviour tower = col.GetComponent<TowerBehaviour>();

            if (tower != null && tower != this) // Must be a tower and not self
            {
                // To apply a permanent buff:
                // Tower must store an original FireRate and a current FireRate.

                // For a simple demonstration, we just log the action:
                Debug.Log($"Support Tower buffing {tower.gameObject.name}: Apply temporary FireRate Boost!");

                // A production system would use a separate BuffManager component on the tower
                // to manage buff stacking and duration.
            }
        }
    }
}