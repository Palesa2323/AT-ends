using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider healthSliderPrefab; // Drag the prefab here
    private Slider healthSliderInstance;
    public Vector3 offset = new Vector3(0, 2f, 0);
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (healthSliderPrefab != null)
        {
            // Instantiate a copy of the prefab for this enemy
            healthSliderInstance = Instantiate(healthSliderPrefab, GameObject.Find("Canvas").transform);
        }
    }

    void LateUpdate()
    {
        if (healthSliderInstance != null && mainCamera != null)
        {
            Vector3 worldPos = transform.position + offset;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
            healthSliderInstance.transform.position = screenPos;
        }
    }

    public void SetMaxHealth(float health)
    {
        if (healthSliderInstance != null)
        {
            healthSliderInstance.maxValue = health;
            healthSliderInstance.value = health;
        }
    }

    public void SetCurrentHealth(float health)
    {
        if (healthSliderInstance != null)
        {
            healthSliderInstance.value = health;
        }
    }

    void OnDestroy()
    {
        if (healthSliderInstance != null)
        {
            Destroy(healthSliderInstance.gameObject); // now safe, it's a scene instance
        }
    }
}
