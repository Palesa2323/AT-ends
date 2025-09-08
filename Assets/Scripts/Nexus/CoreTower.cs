using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CoreTower : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float CurrentHealth;
    public float damageAmount = 10f;

    public Slider healthSlider;
    public Image healthFill; // Drag the Fill component here

    void Start()
    {
        CurrentHealth = MaxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth;
            healthSlider.value = CurrentHealth;
        }

        // Set initial color to green
        if (healthFill != null)
        {
            healthFill.color = Color.green;
        }
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

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