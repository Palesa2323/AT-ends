using UnityEngine;

public class CoreTower : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float CurrentHealth;

    // UI reference for the health bar
    public UnityEngine.UI.Slider healthSlider;

    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        // Update the UI health bar
        if (healthSlider != null)
        {
            healthSlider.value = CurrentHealth / MaxHealth;
        }

        // Check for game over
        if (CurrentHealth <= 0)
        {
            // This can be used to signal the game loop
            Debug.Log("Core Tower Destroyed! Game Over!");
            GameLoop.GameOver();
        }
    }
}
