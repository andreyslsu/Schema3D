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
        if (ScoreManager.Instance == null) return;

        // Always update timer regardless of tracking
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateRate)
        {
            updateTimer = 0f;

            // Only refresh if tracking
            if (ScoreManager.Instance.IsTracking())
                RefreshUI();
            else
                // Show base score when not tracking yet
                ShowBaseScore();
        }
    }

    // Refreshes all score UI elements
    private void RefreshUI()
    {
        if (ScoreManager.Instance == null) return;

        // Update score
        int score = ScoreManager.Instance.CalculateFinalScore();
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
            if (score >= 350) scoreText.color = Color.green;
            else if (score >= 250) scoreText.color = Color.yellow;
            else scoreText.color = Color.red;
        }

        // Update timer with warning color
        float time = ScoreManager.Instance.GetTime();
        float maxTime = ScoreManager.Instance.maxTime;
        float timeLeft = maxTime - time;

        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);

        if (timerText != null)
        {
            timerText.text = "Time: " +
                string.Format("{0:00}:{1:00}", minutes, seconds);

            // Warning colors
            if (timeLeft <= 10f)
                timerText.color = Color.red;      // urgent 
            else if (timeLeft <= 30f)
                timerText.color = Color.yellow;   // warning 
            else
                timerText.color = Color.white;    // normal 
        }

        // Update errors
        if (errorsText != null)
        {
            int errors = ScoreManager.Instance.GetErrors();
            errorsText.text = "Errors: " + errors;
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

    private void ShowBaseScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + ScoreManager.Instance.baseScore;
            scoreText.color = Color.green;
        }

        if (timerText != null)
            timerText.text = "Time: 00:00";

        if (errorsText != null)
        {
            errorsText.text = "Errors: 0";
            errorsText.color = Color.white;
        }

        if (bonusText != null)
        {
            bonusText.text = "Bonus: +0";
            bonusText.color = Color.yellow;
        }
    }

}