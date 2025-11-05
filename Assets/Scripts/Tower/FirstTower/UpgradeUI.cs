using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panel; // the upgrade panel GameObject (enable/disable)
    public Button normalUpgradeBtn;
    public Button bombUpgradeBtn;
    public Button cryoUpgradeBtn;

    public TMP_Text normalCostText;
    public TMP_Text bombCostText;
    public TMP_Text cryoCostText;

    public TMP_Text normalLevelText;
    public TMP_Text bombLevelText;
    public TMP_Text cryoLevelText;

    private void Start()
    {
        if (normalUpgradeBtn != null) normalUpgradeBtn.onClick.AddListener(() => OnUpgradePressed(TowerType.Normal));
        if (bombUpgradeBtn != null) bombUpgradeBtn.onClick.AddListener(() => OnUpgradePressed(TowerType.Bomb));
        if (cryoUpgradeBtn != null) cryoUpgradeBtn.onClick.AddListener(() => OnUpgradePressed(TowerType.Cryo));

        UpdateUI();
    }

    public void TogglePanel()
    {
        if (panel != null) panel.SetActive(!panel.activeSelf);
        UpdateUI();
    }

    void OnUpgradePressed(TowerType type)
    {
        bool ok = TowerManager.Instance.TryUpgradeTowerType(type);
        if (ok) UpdateUI();
        // Optionally show feedback (not enough money, success sound etc.)
    }

    public void UpdateUI()
    {
        if (TowerManager.Instance == null) return;

        normalCostText.text = TowerManager.Instance.GetUpgradeCost(TowerType.Normal).ToString();
        bombCostText.text = TowerManager.Instance.GetUpgradeCost(TowerType.Bomb).ToString();
        cryoCostText.text = TowerManager.Instance.GetUpgradeCost(TowerType.Cryo).ToString();

        normalLevelText.text = "Level " + TowerManager.Instance.GetUpgradeLevel(TowerType.Normal);
        bombLevelText.text = "Level " + TowerManager.Instance.GetUpgradeLevel(TowerType.Bomb);
        cryoLevelText.text = "Level " + TowerManager.Instance.GetUpgradeLevel(TowerType.Cryo);

        // Disable buttons when not enough money
        if (normalUpgradeBtn) normalUpgradeBtn.interactable = (GameManager.Instance.money >= TowerManager.Instance.GetUpgradeCost(TowerType.Normal));
        if (bombUpgradeBtn) bombUpgradeBtn.interactable = (GameManager.Instance.money >= TowerManager.Instance.GetUpgradeCost(TowerType.Bomb));
        if (cryoUpgradeBtn) cryoUpgradeBtn.interactable = (GameManager.Instance.money >= TowerManager.Instance.GetUpgradeCost(TowerType.Cryo));
    }
}

