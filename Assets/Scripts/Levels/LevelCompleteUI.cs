using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelCompleteUI : MonoBehaviour
{
    public static LevelCompleteUI Instance;

    [Header("Level Complete Panel")]
    public GameObject levelCompletePanel;    // Main level complete panel
    public Image fadeOverlay;                // Full screen black image for fade

    [Header("Score Display")]
    public TextMeshProUGUI finalScoreText;   // Shows final score
    public TextMeshProUGUI timeText;         // Shows time taken
    public TextMeshProUGUI errorsText;       // Shows error count
    public TextMeshProUGUI bonusText;        // Shows bonus points
    public TextMeshProUGUI levelNameText;    // Shows level name

    [Header("Star Rating")]
    public GameObject star1;                 
    public GameObject star2;                 
    public GameObject star3;                

    [Header("Buttons")]
    public Button nextLevelButton;           // Load next level
    public Button retryButton;               // Reload current level
    public Button mainMenuButton;            // Return to main menu

    [Header("Animation Settings")]
    public float fadeDuration = 1.5f;        
    public float resultDelay = 0.3f;         

    [Header("Star Settings")]
    public Sprite starFilledSprite;  
    public Sprite starEmptySprite;    

    private LevelData currentLevelData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        levelCompletePanel.SetActive(false);

        // Hide fade overlay
        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
            fadeOverlay.gameObject.SetActive(false);
        }
        SetAllResultsVisible(false);
        SetStarActive(star1, false);
        SetStarActive(star2, false);
        SetStarActive(star3, false);

        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(LoadNextLevel);

        if (retryButton != null)
            retryButton.onClick.AddListener(RetryLevel);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    // =========================================
    // SHOW LEVEL COMPLETE triggers when player step foot on the elevator
    // =========================================
    public void ShowLevelComplete(LevelData levelData)
    {
        currentLevelData = levelData;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.StopTracking();

        if (UIManager.Instance != null)
            UIManager.Instance.HideGameplayUI();

        levelCompletePanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        StartCoroutine(FadeInThenShowResults());
    }

    // =========================================
    // FADE IN ANIMATION
    // =========================================

    private IEnumerator FadeInThenShowResults()
    {
        // Enable fade overlay
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);

            float elapsed = 0f;

            //fade to black
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float alpha = Mathf.Clamp01(elapsed / fadeDuration);

                Color c = fadeOverlay.color;
                c.a = alpha;
                fadeOverlay.color = c;

                yield return null;
            }
        }

        // animation dramatic purposes
        yield return new WaitForSecondsRealtime(0.3f);

        levelCompletePanel.SetActive(true);

        StartCoroutine(AnimateResults());
    }

    // =========================================
    // RESULTS ANIMATION
    // =========================================
    private IEnumerator AnimateResults()
    {
        SetAllResultsVisible(false);

        yield return new WaitForSecondsRealtime(resultDelay);

        if (levelNameText != null)
        {
            levelNameText.gameObject.SetActive(true);
            levelNameText.text = currentLevelData != null
                ? currentLevelData.levelName + " Complete!"
                : "Level Complete!";
        }

        yield return new WaitForSecondsRealtime(resultDelay);

        // Show time
        if (timeText != null)
        {
            timeText.gameObject.SetActive(true);
            float time = ScoreManager.Instance.GetTime();
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);

            // Show as completion time ✓
            timeText.text = "Completed in: " +
                string.Format("{0:00}:{1:00}", minutes, seconds);

            // Color based on speed ✓
            if (time <= 120f)           // under 2 mins
                timeText.color = Color.green;
            else if (time <= 300f)      // under 5 mins
                timeText.color = Color.yellow;
            else
                timeText.color = Color.white;
        }

        yield return new WaitForSecondsRealtime(resultDelay);

        // Show errors
        if (errorsText != null)
        {
            errorsText.gameObject.SetActive(true);
            int errors = ScoreManager.Instance.GetErrors();
            errorsText.text = "Errors: " + errors;
            errorsText.color = errors > 0 ? Color.red : Color.green;
        }

        yield return new WaitForSecondsRealtime(resultDelay);

        // Show bonus
        if (bonusText != null)
        {
            bonusText.gameObject.SetActive(true);
            int bonus = ScoreManager.Instance.GetBonusPoints();
            bonusText.text = "Bonus: +" + bonus;
            bonusText.color = Color.yellow;
        }

        yield return new WaitForSecondsRealtime(resultDelay);

        // Show final score
        if (finalScoreText != null)
        {
            finalScoreText.gameObject.SetActive(true);
            int finalScore = ScoreManager.Instance.CalculateFinalScore();
            StartCoroutine(CountUpScore(finalScore));
        }

        yield return new WaitForSecondsRealtime(resultDelay);

        // Show stars popping in
        yield return StartCoroutine(AnimateStars(
            ScoreManager.Instance.GetStarRating()));

        yield return new WaitForSecondsRealtime(resultDelay);

        // Show buttons
        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(true);
        if (retryButton != null)
            retryButton.gameObject.SetActive(true);
        if (mainMenuButton != null)
            mainMenuButton.gameObject.SetActive(true);

        // Save progress
        SaveProgress();

        // Check if next level exists
        CheckNextLevel();
    }
    private IEnumerator CountUpScore(int targetScore)
    {
        float elapsed = 0f;
        float countDuration = 1f;
        int displayScore = 0;

        while (elapsed < countDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / countDuration;
            displayScore = Mathf.FloorToInt(Mathf.Lerp(0, targetScore, t));

            if (finalScoreText != null)
                finalScoreText.text = "Score: " + displayScore;

            yield return null;
        }

        // Make sure final value is exact
        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + targetScore;

            if (targetScore >= 375)
                finalScoreText.color = Color.green;
            else if (targetScore >= 250)
                finalScoreText.color = Color.yellow;
            else
                finalScoreText.color = Color.red;
        }
    }

    // =========================================
    // STAR ANIMATION
    // =========================================
    private IEnumerator AnimateStars(int starCount)
    {
        SetStarActive(star1, false);
        SetStarActive(star2, false);
        SetStarActive(star3, false);

        yield return new WaitForSecondsRealtime(0.2f);

        // Pop in each star
        if (starCount >= 1)
        {
            SetStarColor(star1, true);
            star1.SetActive(true);
            yield return new WaitForSecondsRealtime(0.3f);
        }
        else
        {
            SetStarColor(star1, false);
            star1.SetActive(true);
        }

        if (starCount >= 2)
        {
            SetStarColor(star2, true);
            star2.SetActive(true);
            yield return new WaitForSecondsRealtime(0.3f);
        }
        else
        {
            SetStarColor(star2, false);
            star2.SetActive(true);
        }

        if (starCount >= 3)
        {
            SetStarColor(star3, true);
            star3.SetActive(true);
        }
        else
        {
            SetStarColor(star3, false);
            star3.SetActive(true);
        }

        Debug.Log("Stars shown: " + starCount);
    }

    // Sets star color based on score
    private void SetStarColor(GameObject star, bool earned)
    {
        if (star == null) return;
        Image img = star.GetComponent<Image>();
        if (img != null)
        {
            // Swap sprite based on earned
            img.sprite = earned ? starFilledSprite : starEmptySprite;
            img.color = Color.white; 
        }
    }

    private void SetStarActive(GameObject star, bool active)
    {
        if (star != null)
            star.SetActive(active);
    }

    private void SetAllResultsVisible(bool visible)
    {
        if (levelNameText != null) levelNameText.gameObject.SetActive(visible);
        if (timeText != null) timeText.gameObject.SetActive(visible);
        if (errorsText != null) errorsText.gameObject.SetActive(visible);
        if (bonusText != null) bonusText.gameObject.SetActive(visible);
        if (finalScoreText != null) finalScoreText.gameObject.SetActive(visible);
        if (nextLevelButton != null) nextLevelButton.gameObject.SetActive(visible);
        if (retryButton != null) retryButton.gameObject.SetActive(visible);
        if (mainMenuButton != null) mainMenuButton.gameObject.SetActive(visible);
    }

    // =========================================
    // SAVE PROGRESS
    // =========================================

    private void SaveProgress()
    {
        if (currentLevelData == null) return;

        string levelName = currentLevelData.levelName;
        int finalScore = ScoreManager.Instance.CalculateFinalScore();
        int stars = ScoreManager.Instance.GetStarRating();

        // Save best score
        int savedScore = PlayerPrefs.GetInt(levelName + "_BestScore", 0);
        if (finalScore > savedScore)
            PlayerPrefs.SetInt(levelName + "_BestScore", finalScore);

        // Save stars
        int savedStars = PlayerPrefs.GetInt(levelName + "_Stars", 0);
        if (stars > savedStars)
            PlayerPrefs.SetInt(levelName + "_Stars", stars);

        // Mark level as played ← NEW
        PlayerPrefs.SetInt(levelName + "_Played", 1);

        // Unlock next level
        if (!string.IsNullOrEmpty(currentLevelData.nextLevelScene))
            PlayerPrefs.SetInt(currentLevelData.nextLevelScene + "_Unlocked", 1);

        PlayerPrefs.Save();
    }

    // =========================================
    // BUTTON CHECKS
    // =========================================
    private void CheckNextLevel()
    {
        if (nextLevelButton == null) return;

        bool hasNextLevel = currentLevelData != null
                         && !string.IsNullOrEmpty(currentLevelData.nextLevelScene);

        nextLevelButton.gameObject.SetActive(hasNextLevel);
    }

    // =========================================
    // BUTTONS
    // =========================================
    public void LoadNextLevel()
    {
        if (currentLevelData == null) return;
        if (string.IsNullOrEmpty(currentLevelData.nextLevelScene)) return;

        ResetState();
        SceneManager.LoadScene(currentLevelData.nextLevelScene);
    }

    public void RetryLevel()
    {
        ResetState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        ResetState();
        SceneManager.LoadScene("MainMenu");
    }

    private void ResetState()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        levelCompletePanel.SetActive(false);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameplayUI();
    }
}