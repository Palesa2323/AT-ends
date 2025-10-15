using UnityEngine;

public class TowerHealth : MonoBehaviour, ITakeDamage
{
    public float MaxHealth = 40f;
    private float currentHealth;
    private Renderer towerRenderer;
    private bool isDead;

    void Start()
    {
        currentHealth = MaxHealth;
        towerRenderer = GetComponentInChildren<Renderer>();
        isDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (towerRenderer != null)
        {
            towerRenderer.material.color = Color.Lerp(Color.red, Color.white, currentHealth / MaxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log($"{name} destroyed (disabled).");

        // Disable everything so enemies stop targeting it
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Optional: slight delay before it vanishes (for feedback)
        StartCoroutine(DisableAfterDelay(0.3f));
    }

    private System.Collections.IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false); // pooled-style removal
    }

    // Optional: call this when you “rebuild” or reuse the tower
    public void ResetTower()
    {
        currentHealth = MaxHealth;
        isDead = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        if (towerRenderer != null)
            towerRenderer.material.color = Color.white;

        gameObject.SetActive(true);
    }
}
