using UnityEngine;
using TMPro;

// Displays live score and stats to player during gameplay
public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance;

    [Header("Score UI")]
    public GameObject scorePanel;            // Panel holding all score UI
    public TextMeshProUGUI scoreText;        // Shows current score
    public TextMeshProUGUI timerText;        // Shows elapsed time
    public TextMeshProUGUI errorsText;       // Shows error count
    public TextMeshProUGUI bonusText;        // Shows bonus points earned

    [Header("Settings")]
    public float updateRate = 0.5f;          // How often UI updates in seconds
                                             // 0.5 = twice per second

    private float updateTimer = 0f;          // Internal update timer

    private void Awake()
    {
        // Singleton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        // Only update if ScoreManager is tracking
        if (ScoreManager.Instance == null) return;
        if (!ScoreManager.Instance.IsTracking()) return;

        // Update UI at set rate to avoid performance issues
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateRate)
        {
            updateTimer = 0f;
            RefreshUI();
        }
    }

    // Refreshes all score UI elements
    private void RefreshUI()
    {
        // Update current score
        if (scoreText != null)
        {
            int currentScore = ScoreManager.Instance.CalculateFinalScore();
            scoreText.text = "Score: " + currentScore;

            // Change color based on score range
            if (currentScore >= 800)
                scoreText.color = Color.green;       // Great score
            else if (currentScore >= 500)
                scoreText.color = Color.yellow;      // Ok score
            else
                scoreText.color = Color.red;         // Low score
        }

        // Update timer
        if (timerText != null)
        {
            float time = ScoreManager.Instance.GetTime();
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }

        // Update errors
        if (errorsText != null)
        {
            int errors = ScoreManager.Instance.GetErrors();
            errorsText.text = "Errors: " + errors;

            // Turn red if player has made errors
            errorsText.color = errors > 0 ? Color.red : Color.white;
        }

        // Update bonus
        if (bonusText != null)
        {
            int bonus = ScoreManager.Instance.GetBonusPoints();
            bonusText.text = "Bonus: +" + bonus;
            bonusText.color = Color.yellow;
        }
    }

    // Call this to force immediate UI update
    // e.g right after an error or bonus
    public void ForceRefresh()
    {
        RefreshUI();
    }
}