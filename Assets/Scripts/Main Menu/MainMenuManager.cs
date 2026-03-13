using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject levelSelectPanel;

    [Header("Settings Sliders")]
    public Slider volumeSlider;
    public Slider sensitivitySlider;

    [Header("Level Select Buttons")]
    public Button level1Button;
    public Button level2Button;
    // Add more as you add levels 

    [Header("Level Button Locks")]
    public GameObject level1LockIcon;   // Lock icon on level 1 button
    public GameObject level2LockIcon;   // Lock icon on level 2 button

    [Header("Level Button Stars")]
    public TextMeshProUGUI level1StarsText; 
    public TextMeshProUGUI level2StarsText;

    // =========================================
    // LEVEL SELECT
    // =========================================

    public void OpenLevelSelect()
    {
        levelSelectPanel.SetActive(true);
        RefreshLevelSelect(); // Update locks and stars 
    }

    public void CloseLevelSelect()
    {
        levelSelectPanel.SetActive(false);
    }

    // Updates each level button based on PlayerPrefs
    private void RefreshLevelSelect()
    {
        // Level 1 always unlocked
        SetupLevelButton(
            level1Button,
            level1LockIcon,
            level1StarsText,
            "Level1",
            true // always unlocked 
        );

        // Level 2 unlocked only if Level 1 completed
        bool level2Unlocked = PlayerPrefs.GetInt("Level2_Unlocked", 0) == 1;
        SetupLevelButton(
            level2Button,
            level2LockIcon,
            level2StarsText,
            "Level2",
            level2Unlocked
        );
    }

    // Sets up a single level button
    private void SetupLevelButton(
        Button button,
        GameObject lockIcon,
        TextMeshProUGUI starsText,
        string sceneName,
        bool isUnlocked)
    {
        if (button == null) return;

        // Enable or disable button based on unlock
        button.interactable = isUnlocked;

        // Show or hide lock icon
        if (lockIcon != null)
            lockIcon.SetActive(!isUnlocked);

        // Show stars if unlocked and played before
        if (starsText != null)
        {
            if (isUnlocked)
            {
                int stars = PlayerPrefs.GetInt(sceneName + "_Stars", 0);
                bool hasPlayed = PlayerPrefs.GetInt(sceneName + "_Played", 0) == 1;

                if (hasPlayed)
                {
                    // Show stars only after level has been played
                    starsText.text = GetStarString(stars);
                    starsText.gameObject.SetActive(true);
                }
                else
                {
                    // Never played → hide stars completely
                    starsText.gameObject.SetActive(false);
                }
            }
            else
            {
                // Locked → hide stars
                starsText.gameObject.SetActive(false);
            }
        }

        // Wire button to load scene
        button.onClick.RemoveAllListeners();
        if (isUnlocked)
            button.onClick.AddListener(() => LoadLevel(sceneName));
    }

    // Converts int stars to star string
    private string GetStarString(int stars)
    {
        switch (stars)
        {
            case 3: return "★★★";
            case 2: return "★★☆";
            case 1: return "★☆☆";
            default: return "☆☆☆";
        }
    }

    // Loads a level by scene name
    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // =========================================
    // SETTINGS
    // =========================================

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);

        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.RegisterSliders(volumeSlider, sensitivitySlider);
            SettingsManager.Instance.SyncSlidersToSaved();
        }
        else
            Debug.LogWarning("SettingsManager not found!");
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    // =========================================
    // SETTINGS SLIDERS
    // =========================================

    public void SetVolume(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.SetVolume(value);
        else
            Debug.LogWarning("SettingsManager not found!");
    }

    public void SetSensitivity(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.SetSensitivity(value);
        else
            Debug.LogWarning("SettingsManager not found!");
    }

    // =========================================
    // QUIT
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