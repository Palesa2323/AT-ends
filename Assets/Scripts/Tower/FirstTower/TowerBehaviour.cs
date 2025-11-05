using UnityEngine;
using System.Collections;

public class TowerBehaviour : MonoBehaviour
{
    public TowerType towerType = TowerType.Normal; // 👈 add this (Normal, Bomb, Cryo)


    public float Range;
    public LayerMask EnemiesLayer;
    public EnemyMovement Target;
    public Transform TowerPivot;

    public float Damage;
    public float FireRate;
    public float delay;
    public float fireTimer;


    public LineRenderer lineRenderer;

    // Internal level tracking (auto-applied by TowerManager)
    private int appliedUpgradeLevel = 0;

    private void Start()
    {
        delay = 1f / FireRate;
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
            lineRenderer.enabled = false;

        // Register this tower globally
        if (TowerManager.Instance != null)
            TowerManager.Instance.RegisterTower(this);

        // Apply any global upgrades (so newly spawned towers are buffed too)
        ApplyUpgradeStats();
    }

    private void OnDestroy()
    {
        if (TowerManager.Instance != null)
            TowerManager.Instance.UnregisterTower(this);
    }

    private void Update()
    {
        if (Target == null || Target.Health <= 0 || !Target.gameObject.activeSelf || Vector3.Distance(transform.position, Target.transform.position) > Range)
        {
            Target = TowerTargetting.GetTarget(this, TowerTargetting.TargetType.First);

            // ✅ Skip healers entirely
            if (Target != null && Target.enemyType == EnemyMovement.EnemyType.Healer)
                Target = null;

            if (Target == null)
            {
                if (lineRenderer != null) lineRenderer.enabled = false;
                return;
            }
        }

        if (TowerPivot != null && Target != null)
        {
            Vector3 direction = Target.transform.position - TowerPivot.position;
            direction.y = 0;
            TowerPivot.rotation = Quaternion.LookRotation(direction);
        }

        fireTimer += Time.deltaTime;
        if (fireTimer >= delay && Target != null)
        {
            Target.TakeDamage(Damage);
            fireTimer = 0f;

            if (lineRenderer != null)
                StartCoroutine(FireLaser());
        }
    }

    // 🔧 This method is called when the tower manager upgrades this tower type
    public void ApplyUpgradeStats()
    {
        if (TowerManager.Instance == null)
            return;

        int level = TowerManager.Instance.GetUpgradeLevel(towerType);

        if (level == appliedUpgradeLevel)
            return; // No change

        appliedUpgradeLevel = level;

        // Example scaling — tweak freely
        Damage *= 1f + (0.5f * level);      // +50% damage per level
        FireRate *= 1f + (0.2f * level);    // +20% fire rate per level
        Range *= 1f + (0.1f * level);       // +10% range per level

        delay = 1f / FireRate;

        // Optional: make tower grow or visually change each upgrade
        transform.localScale = Vector3.one * (1f + 0.1f * level);

        Debug.Log($"{name} ({towerType}) upgraded globally to Level {level}!");
    }

    IEnumerator FireLaser()
    {
        if (lineRenderer == null || Target == null) yield break;

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, TowerPivot.position);
        lineRenderer.SetPosition(1, Target.transform.position);
        yield return new WaitForSeconds(0.1f);
        lineRenderer.enabled = false;
    }
}

