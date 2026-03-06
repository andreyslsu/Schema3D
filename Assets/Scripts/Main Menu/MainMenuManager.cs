using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel; // Settings UI panel
    public GameObject levelSelectPanel;   // Level selection panel

    [Header("Settings Sliders")]
    public Slider volumeSlider;      // Drag main menu volume slider here
    public Slider sensitivitySlider; // Drag main menu sensitivity slider here

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

        // Only sync sliders when opening settings panel
        // SyncSlidersToSaved reads from PlayerPrefs directly
        if (SettingsManager.Instance != null)
        {
            // Register sliders first so SettingsManager knows which ones to update
            SettingsManager.Instance.RegisterSliders(volumeSlider, sensitivitySlider);

            // Then sync them to whatever is saved in PlayerPrefs
            SettingsManager.Instance.SyncSlidersToSaved();
        }
        else
            Debug.LogWarning("SettingsManager not found!");
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
    // Called by volume slider On Value Changed (Dynamic float)
    public void SetVolume(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.SetVolume(value);
        else
            Debug.LogWarning("SettingsManager not found!");
    }

    // Called by sensitivity slider On Value Changed (Dynamic float)
    // Forwards to SettingsManager which saves and applies it
    public void SetSensitivity(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.SetSensitivity(value);
        else
            Debug.LogWarning("SettingsManager not found!");
    }



}