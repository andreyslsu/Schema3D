using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public Image fadeOverlay;

    [Header("Text")]
    public TextMeshProUGUI gameOverTitleText;
    public TextMeshProUGUI gameOverReasonText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI errorsText;

    [Header("Buttons")]
    public Button retryButton;
    public Button mainMenuButton;

    [Header("Animation")]
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);

        // Hide fade overlay
        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
            fadeOverlay.gameObject.SetActive(false);
        }

        // Hide ALL elements at start 
        HideAllElements();

        if (retryButton != null)
            retryButton.onClick.AddListener(RetryLevel);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void HideAllElements()
    {
        if (gameOverTitleText != null)
            gameOverTitleText.gameObject.SetActive(false);

        if (gameOverReasonText != null)
            gameOverReasonText.gameObject.SetActive(false);

        if (timeText != null)
            timeText.gameObject.SetActive(false);

        if (errorsText != null)
            errorsText.gameObject.SetActive(false);

        if (retryButton != null)
            retryButton.gameObject.SetActive(false);

        if (mainMenuButton != null)
            mainMenuButton.gameObject.SetActive(false);
    }

    // =========================================
    // SHOW GAME OVER
    // =========================================

    public void ShowGameOver(string reason)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayGameOverJingle();

        LevelDialogue levelDialogue =
        FindFirstObjectByType<LevelDialogue>();
        if (levelDialogue != null)
            levelDialogue.ShowGameOverHint();

        // Stop score tracking
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.StopTracking();

        // Hide gameplay UI
        if (UIManager.Instance != null)
            UIManager.Instance.HideGameplayUI();

        // Show cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Show panel
        gameOverPanel.SetActive(true);

        // Start fade
        StartCoroutine(FadeInThenShow(reason));
    }

    private IEnumerator FadeInThenShow(string reason)
    {
        // Make sure everything hidden first
        HideAllElements();

        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);

            float elapsed = 0f;
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

        yield return new WaitForSecondsRealtime(0.3f);

        // Show title
        if (gameOverTitleText != null)
        {
            gameOverTitleText.gameObject.SetActive(true);
            gameOverTitleText.text = "GAME OVER";
        }

        yield return new WaitForSecondsRealtime(0.3f);

        // Show reason
        if (gameOverReasonText != null)
        {
            gameOverReasonText.gameObject.SetActive(true);
            gameOverReasonText.text = reason;
        }

        yield return new WaitForSecondsRealtime(0.3f);

        if (timeText != null && ScoreManager.Instance != null)
        {
            timeText.gameObject.SetActive(true);
            float time = ScoreManager.Instance.GetTime();
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);

            // Show how long they lasted 
            timeText.text = "Time Elapsed: " +
                string.Format("{0:00}:{1:00}", minutes, seconds);
            timeText.color = Color.white;
        }

        yield return new WaitForSecondsRealtime(0.3f);

        // Show errors
        if (errorsText != null && ScoreManager.Instance != null)
        {
            errorsText.gameObject.SetActive(true);
            int errors = ScoreManager.Instance.GetErrors();
            errorsText.text = "Errors: " + errors;
            errorsText.color = errors > 0 ? Color.red : Color.green;
        }

        yield return new WaitForSecondsRealtime(0.3f);

        // Show buttons
        if (retryButton != null)
            retryButton.gameObject.SetActive(true);

        if (mainMenuButton != null)
            mainMenuButton.gameObject.SetActive(true);
    }

    // =========================================
    // BUTTONS
    // =========================================

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }
}