using UnityEngine;
using UnityEngine.UI;

public class CoreTower : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float CurrentHealth;

    public Slider healthSlider;
    public Image healthFill;

    // These are now public fields
    public float Range;
    public LayerMask EnemiesLayer;
    public float Damage;
    public float FireRate;

    public float damageAmount = 10f; // Separate from attack damage

    void Start()
    {
        CurrentHealth = MaxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth;
            healthSlider.value = CurrentHealth;
        }

        if (healthFill != null)
        {
            healthFill.color = Color.green;
        }
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        if (healthSlider != null)
        {
            healthSlider.value = CurrentHealth;
        }

        if (healthFill != null)
        {
            healthFill.color = Color.Lerp(Color.red, Color.green, CurrentHealth / MaxHealth);
        }

        if (CurrentHealth <= 0)
        {
            Debug.Log("Core Tower Destroyed! Game Over!");
            FindFirstObjectByType<GameLoop>().GameOver();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyMovement enemy = other.GetComponent<EnemyMovement>();
        if (enemy != null)
        {
            TakeDamage(damageAmount);
            EntitySummoner.RemoveEnemy(enemy);
        }
    }
}