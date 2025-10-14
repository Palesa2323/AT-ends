using UnityEngine;
using UnityEngine.UI;

public class CoreTower : MonoBehaviour // Ensure this inherits from ITakeDamage if you use that
{
    public float MaxHealth = 100f;
    public float CurrentHealth;

    public Slider healthSlider;
    public Image healthFill;

    // Fixed damage amount per enemy reaching the core
    public const float DamagePerEnemy = 0.5f;

    // Removing: Range, EnemiesLayer, FireRate, Damage (they belong to an attacking tower)

    private GameLoop gameLoop;

    void Start()
    {
        CurrentHealth = MaxHealth;
        gameLoop = FindFirstObjectByType<GameLoop>();

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

    public void TakeDamage() // Removed float parameter, using fixed DamagePerEnemy instead
    {
        CurrentHealth -= CoreTower.DamagePerEnemy;
        CurrentHealth = Mathf.Max(0f, CurrentHealth);

        if (healthSlider != null)
        {
            healthSlider.value = CurrentHealth; // CRITICAL: Updates the current position
        }

        if (healthFill != null)
        {
            // Updates the color (red to green) based on health ratio
            healthFill.color = Color.Lerp(Color.red, Color.green, CurrentHealth / MaxHealth);
        }

        // --- Game Over Check ---
        if (CurrentHealth <= 0)
        {
            Debug.Log("Core Tower Destroyed! Game Over!");
            if (gameLoop != null)
            {
                gameLoop.GameOver(); // Call the game loop's game over method
            }
            // Optionally, destroy or deactivate the core visually
            // gameObject.SetActive(false); 
        }
    }

    // Use this simpler OnTriggerEnter for immediate damage and removal
    void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to an active enemy
        EnemyMovement enemy = other.GetComponent<EnemyMovement>();
        if (enemy != null && enemy.gameObject.activeInHierarchy)
        {
            TakeDamage(); // Deduct the fixed 0.5 damage
            EntitySummoner.RemoveEnemy(enemy); // Remove enemy
        }
    }
}