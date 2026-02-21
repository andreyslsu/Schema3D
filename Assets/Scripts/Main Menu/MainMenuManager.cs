using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel; // Settings UI panel

    // =========================================
    // START GAME
    // Loads Level 1
    // =========================================
    public void StartGame()
    {
        SceneManager.LoadScene("Level1"); 
        // Make sure scene name matches exactly
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