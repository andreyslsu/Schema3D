using UnityEngine;


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
        if (introPanel != null)
            introPanel.SetActive(false);

        if (ScoreManager.Instance != null)
        {
            // Always fresh start when closing intro
            ScoreManager.Instance.StartTracking();
            Debug.Log("Score tracking started fresh!");
        }
        else
            Debug.LogWarning("ScoreManager not found!");
    }
}