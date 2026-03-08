using UnityEngine;

// Separate intro panel that shows when level starts
// Player manually closes it when ready
// Score tracking starts after player closes it
public class IntroQuestPanel : MonoBehaviour
{
    public static IntroQuestPanel Instance;

    [Header("Intro Panel")]
    public GameObject introPanel; // Separate intro panel GameObject

    private void Awake()
    {
        // Singleton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Show intro panel when level loads
        if (introPanel != null)
            introPanel.SetActive(true);
        else
            Debug.LogWarning("IntroPanel is NULL!");
    }

    // Called by close button
    // Starts score tracking when player is ready
    public void CloseIntroPanel()
    {
        // Hide intro panel
        if (introPanel != null)
            introPanel.SetActive(false);

        // Start score tracking now that player has read the quests
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.StartTracking();
            Debug.Log("Score tracking started! Player closed intro panel.");
        }
        else
            Debug.LogWarning("ScoreManager not found!");
    }
}