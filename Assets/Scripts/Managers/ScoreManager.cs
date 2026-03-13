using UnityEngine;

// Handles all scoring logic for the game
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score Settings")]
    public int baseScore = 1000;              // Starting score per level
    public int errorPenalty = 10;             // Points lost per wrong answer
    public float timePenaltyRate = 1f;        // Points lost per second
    public int fragmentSpeedBonus = 50;       // Bonus per fragment collected quickly

    [Header("Speed Bonus Settings")]
    public float speedBonusTimeLimit = 10f;   // Seconds to collect fragment for bonus
                                              // e.g collect within 10s = bonus

    // Private tracking variables
    private int errorCount = 0;               // Total wrong answers submitted
    private float timeElapsed = 0f;           // Total time elapsed
    private bool isTracking = false;          // Whether timer is running
    private int bonusPoints = 0;              // Total bonus points earned
    private float lastFragmentTime = 0f;      // Time when last fragment collected

    private void Awake()
    {
        // Singleton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        // Only count time during active gameplay
        if (isTracking)
            timeElapsed += Time.deltaTime;
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

        // 3 stars = 80% or above of base score
        // 2 stars = 50% or above
        // 1 star  = any score above 0
        if (score >= baseScore * 0.8f) return 3; // 3 stars
        else if (score >= baseScore * 0.5f) return 2; // 2 stars
        else if (score > 0) return 1; // 1 star
        else return 0; // 0 stars
    }

    public void ResumeTracking()
    {
        isTracking = true;
        Debug.Log("Score tracking resumed!");
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