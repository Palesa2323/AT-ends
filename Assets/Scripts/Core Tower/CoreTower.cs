using UnityEngine;
using UnityEngine.UI;

public class CoreTower : MonoBehaviour, ITakeDamage
{
    public float MaxHealth = 100f;
    public float CurrentHealth;

    public Slider healthSlider;
    public Image healthFill;

    // If anyone calls the parameterless version, use this default
    public const float DamagePerEnemy = 0.5f;

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

    // MAIN overload used by enemies/towers
    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(0, CurrentHealth);

        if (healthSlider != null) healthSlider.value = CurrentHealth;
        if (healthFill != null) healthFill.color = Color.Lerp(Color.red, Color.green, CurrentHealth / MaxHealth);

        if (CurrentHealth <= 0)
        {
            if (gameLoop != null)
            {
                Debug.Log("Core Tower destroyed — Game Over.");
                gameLoop.GameOver();
            }
        }
    }

    // Backward-compatible wrapper (in case something still calls TakeDamage())
    public void TakeDamage()
    {
        TakeDamage(DamagePerEnemy);
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