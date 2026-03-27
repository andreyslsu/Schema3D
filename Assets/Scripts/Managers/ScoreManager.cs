using UnityEngine;

// Handles all scoring logic for the game
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score Settings")]
    public int baseScore = 500;              // Starting score per level
    public int errorPenalty = 25;             // Points lost per wrong answer
    public float timePenaltyRate = 2f;        // Points lost per second
    public int fragmentSpeedBonus = 50;       // Bonus per fragment collected quickly

    [Header("Speed Bonus Settings")]
    public float speedBonusTimeLimit = 30f;   // Seconds to collect fragment for bonus
                                              // e.g collect within 10s = bonus
    // Private tracking variables
    private int errorCount = 0;               // Total wrong answers submitted
    private float timeElapsed = 0f;           // Total time elapsed
    private bool isTracking = false;          // Whether timer is running
    private int bonusPoints = 0;              // Total bonus points earned
    private float lastFragmentTime = 0f;      // Time when last fragment collected

    [Header("Game Over Settings")]
    public float maxTime = 500f;         // 3 minutes 
    public bool enableGameOver = true;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (!isTracking) return;

        timeElapsed += Time.deltaTime;

        // Check if time ran out
        if (enableGameOver && timeElapsed >= maxTime)
        {
            timeElapsed = maxTime;
            isTracking = false;
            TriggerGameOver();
            return;
        }

        // Check if score hit 0
        if (enableGameOver && CalculateFinalScore() <= 0)
        {
            isTracking = false;
            TriggerGameOver();
            return;
        }
    }

    // =========================================
    // TRACKING CONTROLS
    // =========================================

    // Call this when level starts
    public void StartTracking()
    {
        errorCount = 0;
        timeElapsed = 0f;
        bonusPoints = 0;
        lastFragmentTime = 0f;
        isTracking = true;

        // Force refresh UI immediately
        if (ScoreUI.Instance != null)
            ScoreUI.Instance.ForceRefresh();

        Debug.Log("Score tracking started!");
    }

    private void TriggerGameOver()
    {
        Debug.Log("naubos oras");

        if (GameOverUI.Instance != null)
            GameOverUI.Instance.ShowGameOver(
                "Your base score ran out!");
        else
            Debug.LogWarning("GameOverUI not found!");
    }

    // Call this when level ends
    public void StopTracking()
    {
        isTracking = false;
        Debug.Log("Score tracking stopped!");
    }

    // =========================================
    // SCORE EVENTS
    // =========================================

    // Call this when player submits wrong answer
    public void AddError()
    {
        errorCount++;
        Debug.Log("Error added! Total errors: " + errorCount);
    }

    // Call this when player collects a fragment
    public void OnFragmentCollected()
    {
        // Check if player collected fragment quickly enough for bonus
        if (lastFragmentTime == 0f ||
            timeElapsed - lastFragmentTime <= speedBonusTimeLimit)
        {
            // Award speed bonus
            bonusPoints += fragmentSpeedBonus;
            Debug.Log("Speed bonus earned! Total bonus: " + bonusPoints);
        }

        // Update last fragment time
        lastFragmentTime = timeElapsed;
    }

    // =========================================
    // SCORE CALCULATION
    // =========================================

    // Calculates final score based on time errors and bonuses
    public int CalculateFinalScore()
    {
        // Time penalty: lose points for every second elapsed
        int timePenalty = Mathf.FloorToInt(timeElapsed * timePenaltyRate);

        // Error penalty: lose points per wrong answer
        int errorTotal = errorCount * errorPenalty;

        // Final score: base - penalties + bonuses
        // Mathf.Max ensures score never goes below 0
        int finalScore = Mathf.Max(0, baseScore - timePenalty - errorTotal + bonusPoints);

        Debug.Log("Final Score: " + finalScore
                + " | Time Penalty: " + timePenalty
                + " | Error Penalty: " + errorTotal
                + " | Bonus: " + bonusPoints);

        return finalScore;
    }

    // Returns star rating based on final score
    public int GetStarRating()
    {
        int score = CalculateFinalScore();

        if (score >= baseScore * 0.7f) return 3;  // 350+ 
        else if (score >= baseScore * 0.5f) return 2; // 250+ 
        else if (score > 0) return 1;              // any 
        else return 0;                             // game over 
    }

    public void ResumeTracking()
    {
        isTracking = true;
    }

    // =========================================
    // GETTERS
    // =========================================

    // Returns total time elapsed
    public float GetTime() => timeElapsed;

    // Returns total error count
    public int GetErrors() => errorCount;

    // Returns total bonus points
    public int GetBonusPoints() => bonusPoints;

    // Returns whether tracking is active
    public bool IsTracking() => isTracking;


}