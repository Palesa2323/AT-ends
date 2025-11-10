using UnityEngine;
using UnityEngine.UI;

public class TowerUpgradeUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public Image towerIcon;
    public Text towerNameText;
    public Text upgradeCostText;
    public Button upgradeButton;

    private TowerType currentTowerType;
    private int currentLevel;

    private void Start()
    {
        // Hide panel initially
        panel.SetActive(false);

        // Hook up button
        upgradeButton.onClick.AddListener(OnUpgradePressed);
    }

    /// <summary>
    /// Call this when the player selects a tower type to upgrade
    /// </summary>
    public void ShowUpgradeUI(TowerType type)
    {
        currentTowerType = type;
        currentLevel = TowerManager.Instance.GetUpgradeLevel(type);

        // Get next level data
        int nextLevel = currentLevel + 1;
        TowerData towerData = TowerManager.Instance.upgradeData.GetTowerData(type, nextLevel);

        if (towerData == null)
        {
            Debug.Log($"{type} is already max level!");
            panel.SetActive(false);
            return;
        }

        towerIcon.sprite = towerData.icon;
        towerNameText.text = $"{type} Tower Lv {nextLevel}";
        upgradeCostText.text = $"Cost: {towerData.upgradeCost}";

        panel.SetActive(true);
    }

    private void OnUpgradePressed()
    {
        int nextLevel = currentLevel + 1;
        TowerData towerData = TowerManager.Instance.upgradeData.GetTowerData(currentTowerType, nextLevel);

        if (towerData == null)
        {
            Debug.Log("Tower is max level!");
            panel.SetActive(false);
            return;
        }

        // Try spending resources
        if (TowerManager.Instance.SpendResources(towerData.upgradeCost))
        {
            // Upgrade all towers of this type
            TowerManager.Instance.UpgradeExistingTowers(currentTowerType);

            Debug.Log($"{currentTowerType} upgraded to level {nextLevel}!");
            ShowUpgradeUI(currentTowerType); // Refresh UI for next upgrade
        }
        else
        {
            Debug.Log("Not enough resources!");
        }
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }

}
