using UnityEngine;
using System.Collections;

public class TowerBehaviour : MonoBehaviour
{
    public TowerType towerType = TowerType.Normal;

    [Header("Tower Stats (from SO)")]
    public TowerData towerData;

    public float Range;
    public LayerMask EnemiesLayer;
    public EnemyMovement Target;
    public Transform TowerPivot;

    public float fireTimer;
    public float delay;
    public float Damage;

    [Header("Upgrade Prefabs")]
    public GameObject nextLevelPrefab;   // assign in inspector
    public int upgradeCost = 100;        // resources needed for upgrade

    public LineRenderer lineRenderer;

    private void Start()
    {
        if (towerData == null)
        {
            Debug.LogError($"{name} has no TowerData assigned!");
            return;
        }

        fireTimer = 0f;
        delay = 1f / towerData.fireRate;

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null) lineRenderer.enabled = false;

        // Register tower globally
        TowerManager.Instance?.RegisterTower(this);
    }

    private void OnDestroy()
    {
        TowerManager.Instance?.UnregisterTower(this);
    }

    private void Update()
    {
        if (Target == null || Target.Health <= 0 || !Target.gameObject.activeSelf || Vector3.Distance(transform.position, Target.transform.position) > towerData.range)
        {
            Target = TowerTargetting.GetTarget(this, TowerTargetting.TargetType.First);

            // Skip healers
            if (Target != null && Target.enemyType == EnemyMovement.EnemyType.Healer)
                Target = null;

            if (Target == null)
            {
                if (lineRenderer != null) lineRenderer.enabled = false;
                return;
            }
        }

        RotateTower();
        HandleFiring();
    }

    private void RotateTower()
    {
        if (TowerPivot != null && Target != null)
        {
            Vector3 direction = Target.transform.position - TowerPivot.position;
            direction.y = 0;
            if (direction != Vector3.zero)
                TowerPivot.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void HandleFiring()
    {
        if (Target == null) return;

        fireTimer += Time.deltaTime;
        if (fireTimer >= 1f / towerData.fireRate)
        {
            Target.TakeDamage(towerData.damage);
            fireTimer = 0f;

            if (lineRenderer != null)
                StartCoroutine(FireLaser());
        }
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

    public void UpgradeTower()
    {
        if (TowerManager.Instance == null) return;

        if (TowerManager.Instance.playerResources < upgradeCost)
        {
            Debug.Log("Not enough resources to upgrade!");
            return;
        }

        TowerManager.Instance.SpendResources(upgradeCost);

        if (nextLevelPrefab != null)
        {
            // Spawn upgraded tower prefab
            GameObject upgradedTower = Instantiate(nextLevelPrefab, transform.position, transform.rotation);

            // Optional: Copy over dynamic state (scale, etc.)
            upgradedTower.transform.localScale = transform.localScale;

            // Destroy current tower
            Destroy(gameObject);

            Debug.Log("Tower upgraded via prefab swap!");
        }
    }
}
