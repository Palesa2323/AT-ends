using UnityEngine;

public class UpgradeUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject upgradePanel;

    private void Start()
    {
        if (upgradePanel != null)
            upgradePanel.SetActive(false); // make sure it starts hidden
    }

    public void OpenUpgradePanel()
    {
        if (upgradePanel == null)
        {
            Debug.LogError("⚠️ UpgradePanel not assigned in the Inspector!");
            return;
        }

        upgradePanel.SetActive(true);
        Debug.Log("✅ Upgrade panel opened!");
    }

    public void CloseUpgradePanel()
    {
        if (upgradePanel == null)
        {
            Debug.LogError("⚠️ UpgradePanel not assigned in the Inspector!");
            return;
        }

        upgradePanel.SetActive(false);
        Debug.Log("❌ Upgrade panel closed!");
    }
}

