using UnityEngine;
using System.Collections;

public class TowerBehaviour : MonoBehaviour
{
    public float Range;
    public LayerMask EnemiesLayer;

    public EnemyMovement Target;
    public Transform TowerPivot;
    public float Damage;
    public float FireRate;

    public float delay;
    public float fireTimer;
    public LineRenderer lineRenderer;

    public int towerLevel = 1;
    public int maxTowerLevel = 3;
    public int upgradeCost = 100;

    void Start()
    {
        delay = 1f / FireRate;
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    void Update()
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
            {
                StartCoroutine(FireLaser());
            }
        }
    }
    public virtual void UpgradeTower()
    {
        // If the tower is already at max level, do nothing
        if (towerLevel >= maxTowerLevel)
        {
            Debug.Log($"{name} is already maxed out!");
            return;
        }

        // Check if player has enough money
        if (GameManager.Instance.money < upgradeCost)
        {
            Debug.Log("Not enough money to upgrade!");
            return;
        }

        // Deduct money and upgrade stats
        GameManager.Instance.money -= upgradeCost;
        towerLevel++;

        // Stats increase (you can modify this for each tower type)
        Damage *= 1.5f;
        FireRate *= 1.2f;
        Range *= 1.2f;

        // Recalculate delay based on fire rate
        delay = 1f / FireRate;

        // Increase the cost of the next upgrade
        upgradeCost *= 2;

        // Optionally, grow the tower a little visually
        transform.localScale *= 1.1f;

        Debug.Log($"{name} upgraded to Level {towerLevel}!");
    }
    void OnMouseDown()
    {
        TowerUI.Instance.Show(this);
    }


    IEnumerator FireLaser()
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, TowerPivot.position);
        lineRenderer.SetPosition(1, Target.transform.position);
        yield return new WaitForSeconds(0.1f);
        lineRenderer.enabled = false;
    }
}
