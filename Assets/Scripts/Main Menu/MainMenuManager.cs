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
    public GameObject profilePanel;
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
    public Button level6Button;
    public Button level7Button;
    public Button level8Button;
    public Button level9Button;
    public Button level10Button;

    [Header("Level Button Locks")]
    public GameObject level1LockIcon;  
    public GameObject level2LockIcon;  
    public GameObject level3LockIcon;  
    public GameObject level4LockIcon; 
    public GameObject level5LockIcon;
    public GameObject level6LockIcon;
    public GameObject level7LockIcon;
    public GameObject level8LockIcon;
    public GameObject level9LockIcon;
    public GameObject level10LockIcon;

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

    public Image level6Star1;
    public Image level6Star2;
    public Image level6Star3;

    public Image level7Star1;
    public Image level7Star2;
    public Image level7Star3;

    public Image level8Star1;
    public Image level8Star2;
    public Image level8Star3;

    public Image level9Star1;
    public Image level9Star2;
    public Image level9Star3;

    public Image level10Star1;
    public Image level10Star2;
    public Image level10Star3;

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

        bool level6Unlocked =
            PlayerPrefs.GetInt("Level6_Unlocked", 0) == 1;
        SetupLevelButton(
            level6Button, level6LockIcon,
            level6Star1, level6Star2, level6Star3,
            "Level6", level6Unlocked);

        bool level7Unlocked =
           PlayerPrefs.GetInt("Level7_Unlocked", 0) == 1;
        SetupLevelButton(
            level7Button, level7LockIcon,
            level7Star1, level7Star2, level7Star3,
            "Level7", level7Unlocked);

        bool level8Unlocked =
           PlayerPrefs.GetInt("Level8_Unlocked", 0) == 1;
        SetupLevelButton(
            level8Button, level8LockIcon,
            level8Star1, level8Star2, level8Star3,
            "Level8", level8Unlocked);

        bool level9Unlocked =
           PlayerPrefs.GetInt("Level9_Unlocked", 0) == 1;
        SetupLevelButton(
            level9Button, level9LockIcon,
            level9Star1, level9Star2, level9Star3,
            "Level9", level9Unlocked);

        bool level10Unlocked =
           PlayerPrefs.GetInt("Level10_Unlocked", 0) == 1;
        SetupLevelButton(
            level10Button, level10LockIcon,
            level10Star1, level10Star2, level10Star3,
            "Level10", level10Unlocked);

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

    public void LoadSign(string SignIn)
    {
        SceneManager.LoadScene(SignIn);
    }


    // =========================================
    // player profile
    // =========================================
    public void OpenProfile()
    {
        profilePanel.SetActive(true);
    }

    public void CloseProfile()
    {
        profilePanel.SetActive(false);
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