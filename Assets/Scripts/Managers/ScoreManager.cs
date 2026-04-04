using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score Settings")]
    public int baseScore = 1000;
    public int errorPenalty = 25;
    public int fragmentSpeedBonus = 30;

    [Header("Speed Bonus Settings")]
    public float speedBonusTimeLimit = 30f;

    [Header("Game Over Settings")]
    public float maxTime = 1200f;  // 10 minutes max ✓
    public bool enableGameOver = true;

    // Private tracking
    private int errorCount = 0;
    private float timeElapsed = 0f;
    private bool isTracking = false;
    private int bonusPoints = 0;
    private float lastFragmentTime = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (!isTracking) return;

        // Timer counts UP ✓
        timeElapsed += Time.deltaTime;

        // Game over only if max time exceeded ✓
        if (enableGameOver && timeElapsed >= maxTime)
        {
            timeElapsed = maxTime;
            isTracking = false;
            TriggerGameOver();
        }
    }

    // =========================================
    // TRACKING
    // =========================================

    public void StartTracking()
    {
        errorCount = 0;
        timeElapsed = 0f;
        bonusPoints = 0;
        lastFragmentTime = 0f;
        isTracking = true;

        if (ScoreUI.Instance != null)
            ScoreUI.Instance.ForceRefresh();

        Debug.Log("Score tracking started!");
    }

    public void StopTracking()
    {
        isTracking = false;
        Debug.Log("Score tracking stopped!");
    }

    public void ResumeTracking()
    {
        isTracking = true;
    }

    // =========================================
    // SCORE EVENTS
    // =========================================

    public void AddError()
    {
        errorCount++;
        Debug.Log("Error! Total: " + errorCount);
    }

    public void OnFragmentCollected()
    {
        // Speed bonus if collected quickly ✓
        if (lastFragmentTime == 0f ||
            timeElapsed - lastFragmentTime
            <= speedBonusTimeLimit)
        {
            bonusPoints += fragmentSpeedBonus;
            Debug.Log("Speed bonus! Total: " + bonusPoints);
        }

        lastFragmentTime = timeElapsed;
    }

    public void AddCompletionBonus()
    {
        bonusPoints += 50;
        Debug.Log("Completion bonus added!");
    }

    // =========================================
    // SCORE CALCULATION
    // =========================================

    public int CalculateFinalScore()
    {
        // Score = base - error penalties + bonuses ✓
        // Time does NOT reduce score ✓
        int errorTotal = errorCount * errorPenalty;

        int finalScore = Mathf.Max(0,
            baseScore
            - errorTotal
            + bonusPoints);

        return finalScore;
    }

    public int GetStarRating()
    {
        int score = CalculateFinalScore();

        // Stars based on score only ✓
       
        if (score >= 900) return 3;     
        else if (score >= 750) return 2; 
        else if (score > 0) return 1;    
        else return 0;
    }

    private void TriggerGameOver()
    {
        Debug.Log("Time ran out!");

        if (GameOverUI.Instance != null)
            GameOverUI.Instance.ShowGameOver(
                "You ran out of time!");
        else
            Debug.LogWarning("GameOverUI not found!");
    }

    // =========================================
    // GETTERS
    // =========================================

    public float GetTime() => timeElapsed;
    public int GetErrors() => errorCount;
    public int GetBonusPoints() => bonusPoints;
    public bool IsTracking() => isTracking;
}