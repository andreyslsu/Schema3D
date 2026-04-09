using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject levelSelectPanel;
    public GameObject leaderboardPanel;
    public GameObject signinPanel;
    public GameObject quitPanel;

    [Header("Settings Sliders")]
    public Slider volumeSlider;
    public Slider sensitivitySlider;

    [Header("Level Select Buttons")]
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;
    public Button level4Button;
    public Button level5Button;

    [Header("Level Button Locks")]
    public GameObject level1LockIcon;  
    public GameObject level2LockIcon;  
    public GameObject level3LockIcon;  
    public GameObject level4LockIcon; 
    public GameObject level5LockIcon;

    [Header("Level Button Stars")]
    public Image level1Star1;
    public Image level1Star2;
    public Image level1Star3;

    public Image level2Star1;
    public Image level2Star2;
    public Image level2Star3;

    public Image level3Star1;
    public Image level3Star2;
    public Image level3Star3;

    public Image level4Star1;
    public Image level4Star2;
    public Image level4Star3;

    public Image level5Star1;
    public Image level5Star2;
    public Image level5Star3;

    [Header("Star Sprites")]
    public Sprite starFilledSprite;  
    public Sprite starEmptySprite;   

    // =========================================
    // LEVEL SELECT
    // =========================================

    public void OpenLevelSelect()
    {
        levelSelectPanel.SetActive(true);
        RefreshLevelSelect(); 
    }

    public void CloseLevelSelect()
    {
        levelSelectPanel.SetActive(false);
    }

    private void RefreshLevelSelect()
    {
        SetupLevelButton(
            level1Button, level1LockIcon,
            level1Star1, level1Star2, level1Star3,
            "Level1", true);

        bool level2Unlocked =
            PlayerPrefs.GetInt("Level2_Unlocked", 0) == 1;
        SetupLevelButton(
            level2Button, level2LockIcon,
            level2Star1, level2Star2, level2Star3,
            "Level2", level2Unlocked);

        bool level3Unlocked =
            PlayerPrefs.GetInt("Level3_Unlocked", 0) == 1;
        SetupLevelButton(
            level3Button, level3LockIcon,
            level3Star1, level3Star2, level3Star3,
            "Level3", level3Unlocked);

        bool level4Unlocked =
            PlayerPrefs.GetInt("Level4_Unlocked", 0) == 1;
        SetupLevelButton(
            level4Button, level4LockIcon,
            level4Star1, level4Star2, level4Star3,
            "Level4", level4Unlocked);

        bool level5Unlocked =
            PlayerPrefs.GetInt("Level5_Unlocked", 0) == 1;
        SetupLevelButton(
            level5Button, level5LockIcon,
            level5Star1, level5Star2, level5Star3,
            "Level5", level5Unlocked);
    }

    private void SetupLevelButton(
    Button button,
    GameObject lockIcon,
    Image star1, Image star2, Image star3,
    string sceneName,
    bool isUnlocked)
    {
        if (button == null) return;

        button.interactable = isUnlocked;

        if (lockIcon != null)
            lockIcon.SetActive(!isUnlocked);

        bool hasPlayed = PlayerPrefs.GetInt(sceneName + "_Played", 0) == 1;
        int stars = PlayerPrefs.GetInt(sceneName + "_Stars", 0);

        if (isUnlocked && hasPlayed)
        {
            SetLevelSelectStar(star1, stars >= 1);
            SetLevelSelectStar(star2, stars >= 2);
            SetLevelSelectStar(star3, stars >= 3);

            if (star1 != null) star1.gameObject.SetActive(true);
            if (star2 != null) star2.gameObject.SetActive(true);
            if (star3 != null) star3.gameObject.SetActive(true);
        }
        else
        {
            if (star1 != null) star1.gameObject.SetActive(false);
            if (star2 != null) star2.gameObject.SetActive(false);
            if (star3 != null) star3.gameObject.SetActive(false);
        }

        button.onClick.RemoveAllListeners();
        if (isUnlocked)
            button.onClick.AddListener(() => LoadLevel(sceneName));
    }

    private void SetLevelSelectStar(Image star, bool earned)
    {
        if (star == null) return;
        star.sprite = earned ? starFilledSprite : starEmptySprite;
        star.color = Color.white; 
    }

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
    // LEADERBOARD
    // =========================================
    public void OpenLeaderboard()
    {
        leaderboardPanel.SetActive(true);
    }

    public void CloseLeaderboard()
    {
        leaderboardPanel.SetActive(false);
    }

    // =========================================
    // signin
    // =========================================
    public void OpenSignin()
    {
        signinPanel.SetActive(true);
    }

    public void CloseSignin()
    {
        signinPanel.SetActive(false);
    }

    // =========================================
    // QUIT
    // =========================================

    public void OpenQuit()
    { 
        quitPanel.SetActive(true);
    }

    public void CloseQuit()
    {
        quitPanel.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}