using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel; // Settings UI panel
    public GameObject levelSelectPanel;   // Level selection panel


    // OPEN LEVEL SELECT
    // Shows level selection panel
    public void OpenLevelSelect()
    {
        levelSelectPanel.SetActive(true);
    }
    // CLOSE LEVEL SELECT
    // Hides level selection panel
    // =========================================
    public void CloseLevelSelect()
    {
        levelSelectPanel.SetActive(false);
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene("Level1");

    }

    // =========================================
    // OPEN SETTINGS
    // Shows settings panel
    // =========================================
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    // =========================================
    // CLOSE SETTINGS
    // Hides settings panel
    // =========================================
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    // =========================================
    // QUIT GAME
    // Exits application
    // =========================================
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}