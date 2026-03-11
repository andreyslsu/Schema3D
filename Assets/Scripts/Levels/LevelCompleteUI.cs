using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

// Shows level complete screen with score breakdown
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
    public GameObject star1;                 // First star
    public GameObject star2;                 // Second star
    public GameObject star3;                 // Third star

    [Header("Buttons")]
    public Button nextLevelButton;           // Load next level
    public Button retryButton;               // Reload current level
    public Button mainMenuButton;            // Return to main menu

    [Header("Animation Settings")]
    public float fadeDuration = 1.5f;        // How long fade to black takes
    public float resultDelay = 0.3f;         // Delay between each result appearing

    [Header("Star Settings")]
    public Color starActiveColor = Color.yellow;
    public Color starInactiveColor = Color.grey;

    private LevelData currentLevelData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Hide everything at start
        levelCompletePanel.SetActive(false);

        // Fade overlay starts fully transparent
        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
            fadeOverlay.gameObject.SetActive(false);
        }

        // Wire up buttons
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(LoadNextLevel);

        if (retryButton != null)
            retryButton.onClick.AddListener(RetryLevel);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    // =========================================
    // SHOW LEVEL COMPLETE
    // =========================================

    // Called by Elevator when player enters
    public void ShowLevelComplete(LevelData levelData)
    {
        currentLevelData = levelData;

        // Stop score tracking
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.StopTracking();

        // Hide hotbar and pause button
        if (UIManager.Instance != null)
            UIManager.Instance.HideGameplayUI();

        // Show panel
        levelCompletePanel.SetActive(true);

        // Show cursor for buttons
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Start fade then show results
        StartCoroutine(FadeInThenShowResults());
    }

    // =========================================
    // FADE IN ANIMATION
    // =========================================

    // Fades screen to black then shows results
    private IEnumerator FadeInThenShowResults()
    {
        // Enable fade overlay
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);

            float elapsed = 0f;

            // Fade from transparent to black
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

        // Small pause at full black
        yield return new WaitForSecondsRealtime(0.3f);

        // Show level complete panel
        levelCompletePanel.SetActive(true);

        // Animate results appearing
        StartCoroutine(AnimateResults());
    }

    // =========================================
    // RESULTS ANIMATION
    // =========================================

    // Shows results one by one with delay
    private IEnumerator AnimateResults()
    {
        // Hide all results first
        SetAllResultsVisible(false);

        yield return new WaitForSecondsRealtime(resultDelay);

        // Show level name
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
            timeText.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
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

        // Show final score counting up
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

    // Counts score up from 0 to final value
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

            // Color based on score
            if (targetScore >= 800)
                finalScoreText.color = Color.green;
            else if (targetScore >= 500)
                finalScoreText.color = Color.yellow;
            else
                finalScoreText.color = Color.red;
        }
    }

    // =========================================
    // STAR ANIMATION
    // =========================================

    // Stars pop in one by one
    private IEnumerator AnimateStars(int starCount)
    {
        // Hide all stars first
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

    // Sets star color based on whether earned
    private void SetStarColor(GameObject star, bool earned)
    {
        if (star == null) return;
        Image img = star.GetComponent<Image>();
        if (img != null)
            img.color = earned ? starActiveColor : starInactiveColor;
    }

    // Sets star active state
    private void SetStarActive(GameObject star, bool active)
    {
        if (star != null)
            star.SetActive(active);
    }

    // Hides all result texts
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

        string levelKey = currentLevelData.levelName;

        // Save best score
        int finalScore = ScoreManager.Instance.CalculateFinalScore();
        int savedBest = PlayerPrefs.GetInt(levelKey + "_BestScore", 0);

        if (finalScore > savedBest)
        {
            PlayerPrefs.SetInt(levelKey + "_BestScore", finalScore);
            Debug.Log("New best score: " + finalScore);
        }

        // Save best stars
        int stars = ScoreManager.Instance.GetStarRating();
        int savedStars = PlayerPrefs.GetInt(levelKey + "_Stars", 0);

        if (stars > savedStars)
            PlayerPrefs.SetInt(levelKey + "_Stars", stars);

        // Unlock next level
        if (!string.IsNullOrEmpty(currentLevelData.nextLevelScene))
        {
            PlayerPrefs.SetInt(currentLevelData.nextLevelScene + "_Unlocked", 1);
            Debug.Log("Unlocked: " + currentLevelData.nextLevelScene);
        }

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

        // Show gameplay UI again if retrying
        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameplayUI();
    }
}