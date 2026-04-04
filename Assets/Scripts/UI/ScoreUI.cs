using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance;

    [Header("Score UI")]
    public GameObject scorePanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI errorsText;
    public TextMeshProUGUI bonusText;

    [Header("Settings")]
    public float updateRate = 0.5f;
    private float updateTimer = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (ScoreManager.Instance == null) return;

        updateTimer += Time.deltaTime;
        if (updateTimer >= updateRate)
        {
            updateTimer = 0f;

            if (ScoreManager.Instance.IsTracking())
                RefreshUI();
            else
                ShowBaseScore();
        }
    }

    private void RefreshUI()
    {
        if (ScoreManager.Instance == null) return;

        // Update score ✓
        int score = ScoreManager.Instance
            .CalculateFinalScore();

        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;

            // Color based on errors 
            if (score >= 900)
                scoreText.color = Color.green;
            else if (score >= 750)
                scoreText.color = Color.yellow;
            else
                scoreText.color = Color.red;
        }

        // Timer counts UP ✓
        // Shows time elapsed ✓
        float time = ScoreManager.Instance.GetTime();
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        if (timerText != null)
        {
            timerText.text = "Time: " +
                string.Format("{0:00}:{1:00}",
                minutes, seconds);

            // No warning colors ✓
            // Time just counts up ✓
            timerText.color = Color.white;
        }

        // Update errors ✓
        if (errorsText != null)
        {
            int errors = ScoreManager.Instance.GetErrors();
            errorsText.text = "Errors: " + errors;
            errorsText.color = errors > 0 ?
                Color.red : Color.white;
        }

        // Update bonus ✓
        if (bonusText != null)
        {
            int bonus = ScoreManager.Instance
                .GetBonusPoints();
            bonusText.text = "Bonus: +" + bonus;
            bonusText.color = Color.yellow;
        }
    }

    public void ForceRefresh()
    {
        RefreshUI();
    }

    private void ShowBaseScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " +
                ScoreManager.Instance.baseScore;
            scoreText.color = Color.green;
        }

        if (timerText != null)
        {
            timerText.text = "Time: 00:00";
            timerText.color = Color.white;
        }

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