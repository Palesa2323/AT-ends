using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuManager : MonoBehaviour
{
    public GameObject upgradePanel; // assign the panel here
    public Button exitButton;

    private void Start()
    {
        upgradePanel.SetActive(false);

        exitButton.onClick.AddListener(CloseUpgradeMenu);
    }

    public void OpenUpgradeMenu()
    {
        upgradePanel.SetActive(true);
        Time.timeScale = 0f; // pause the game if you want
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseUpgradeMenu()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
