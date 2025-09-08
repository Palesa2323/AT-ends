using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // The Slider component that will be our health bar
    public Slider healthSlider;

    // This method is called to set the maximum value of the health bar
    public void SetMaxHealth(float health)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = health;
            healthSlider.value = health;
        }
    }

    // This method updates the current value of the health bar
    public void SetCurrentHealth(float health)
    {
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }
    }
}
