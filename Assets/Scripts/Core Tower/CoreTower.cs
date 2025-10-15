using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CoreTower : MonoBehaviour, ITakeDamage
{
    [Header("Core Health Settings")]
    public float MaxHealth = 100f;
    public float CurrentHealth;
    public Slider healthSlider;
    public Image healthFill;
    private GameLoop gameLoop;
    // Removed: fireTimer

    void Start()
    {
        CurrentHealth = MaxHealth;
        // Use FindAnyObjectByType for stability across Unity versions
        gameLoop = FindAnyObjectByType<GameLoop>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth;
            healthSlider.value = CurrentHealth;
        }

        if (healthFill != null)
            healthFill.color = Color.green;

       
    }
    void Update()
    {
        // Removed: All attack and fireTimer logic
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(0f, CurrentHealth);

        // UI Updates
        if (healthSlider != null)
            healthSlider.value = CurrentHealth;

        if (healthFill != null)
            healthFill.color = Color.Lerp(Color.red, Color.green, CurrentHealth / MaxHealth);

        // Game Over Check
        if (CurrentHealth <= 0)
        {
            Debug.Log("Core Tower Destroyed! Game Over!");
            if (gameLoop != null)
                gameLoop.GameOver();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        EnemyMovement enemy = other.GetComponent<EnemyMovement>();
        if (enemy != null && enemy.gameObject.activeInHierarchy)
        {
            // 1. Apply core damage based on the enemy's damage value
            TakeDamage(enemy.damageToCore);

     
            EntitySummoner.RemoveEnemy(enemy);

            // Ensure the enemy is visually gone if EntitySummoner doesn't handle the disable
            enemy.gameObject.SetActive(false);
        }
    }
}
