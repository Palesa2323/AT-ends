using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeUIManager : MonoBehaviour
{
    public GameObject upgradePanel;

    public void OpenUpgradeMenu()
    {
        upgradePanel.SetActive(true);
        Time.timeScale = 0f; // pause game
    }

    public void CloseUpgradeMenu()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f; // resume
    }

    public void UpgradeNormal() => TowerManager.Instance.TryUpgradeTowerType(TowerType.Normal);
    public void UpgradeBomb() => TowerManager.Instance.TryUpgradeTowerType(TowerType.Bomb);
    public void UpgradeCrypto() => TowerManager.Instance.TryUpgradeTowerType(TowerType.Crypto);
}
