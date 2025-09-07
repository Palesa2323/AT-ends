using UnityEngine;
using UnityEngine.UI; // Must be added to use UI components

public class CoreTower : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float CurrentHealth;

    // UI reference for the health bar
    public Slider healthSlider;

    void Start()
    {
        CurrentHealth = MaxHealth;
        // Set the slider's max value to the core tower's max health
        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth;
            healthSlider.value = CurrentHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        if (healthSlider != null)
        {
            healthSlider.value = CurrentHealth;
        }

        if (CurrentHealth <= 0)
        {
            Debug.Log("Core Tower Destroyed! Game Over!");
            FindFirstObjectByType<GameLoop>().GameOver();
        }
    }
}