using UnityEngine;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{
    public static TowerUI Instance;
    public Button upgradeButton;
    private TowerBehaviour currentTower;

    void Awake()
    {
        Instance = this;
        upgradeButton.gameObject.SetActive(false); // Hide button by default
        upgradeButton.onClick.AddListener(OnUpgradeButtonPressed); // Attach the upgrade method to the button click
    }

    // Show the button above the selected tower
    public void Show(TowerBehaviour tower)
    {
        currentTower = tower;

        // Show the upgrade button
        upgradeButton.gameObject.SetActive(true);

        // Optionally, you can hide any resource text here to prevent overlap (example if you have a Text component for resources)
        //resourceText.gameObject.SetActive(false); // Uncomment if you have resource text to hide

        // Convert world position to screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(tower.transform.position + Vector3.up * 2f);
        upgradeButton.transform.position = screenPos;
    }

    // Hide the upgrade button
    public void Hide()
    {
        upgradeButton.gameObject.SetActive(false);
        currentTower = null;
    }

    // This is called when the upgrade button is pressed
    public void OnUpgradeButtonPressed()
    {
        if (currentTower != null)
        {
            currentTower.UpgradeTower();
        }
    }

    void Update()
    {
        // Keep button above tower (in case it moves or if you want to track it)
        if (currentTower != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(currentTower.transform.position + Vector3.up * 2f);
            upgradeButton.transform.position = screenPos;
        }
    }
}
