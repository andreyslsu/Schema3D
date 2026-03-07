using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Handles all laptop puzzle logic separately from UIManager
// This makes it easy to have different answers per level
public class LaptopManager : MonoBehaviour
{
    public static LaptopManager Instance;

    [Header("Laptop UI")]
    public GameObject laptopPanel;            // The laptop panel
    public TMP_InputField laptopInput;        // Input field player types in
    public TextMeshProUGUI feedbackText;      // Shows correct/wrong feedback
    public TextMeshProUGUI highlightDisplay;  // Overlay for word color highlighting


    [Header("Highlighting Colors")]
    public Color correctWordColor = Color.green;  // Color for correct words
    public Color wrongWordColor = Color.red;       // Color for wrong words
    public Color neutralWordColor = Color.white;   // Color while still typing

    [Header("Feedback Settings")]
    public float feedbackDuration = 2f;       // How long feedback shows

    [Header("Level Data")]
    public LevelData currentLevelData;        // Drag current level's LevelData here

    [Header("Wrong Answer Effects")]
    public Image laptopPanelImage;        // Background image of laptop panel
    public float wrongFlashDuration = 0.3f; // How long red flash shows
    private Color originalPanelColor;       // Stores original panel color

    // Expected answer split into words for highlighting
    private string[] expectedWords;

    // Whether word highlighting is active
    private bool isHighlighting = false;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (laptopPanelImage != null)
            originalPanelColor = laptopPanelImage.color;

        // laptopInput.onValueChanged.AddListener(OnInputChanged);
        laptopInput.lineType = TMP_InputField.LineType.MultiLineNewline; 

        // Hide feedback at start but keep it in scene
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
            Debug.Log("FeedbackText found and hidden");
        }
        else
            Debug.LogWarning("FeedbackText is NULL! Drag it into LaptopManager Inspector!");
    }

    // =========================================
    // OPEN / CLOSE LAPTOP
    // =========================================

    // Opens laptop panel and sets up everything
    public void OpenLaptop()
    {
        laptopPanel.SetActive(true);
        Time.timeScale = 0f;

        // Clear input and highlight
        laptopInput.text = "";
     //   highlightDisplay.text = "";

        // Make sure input field settings are correct for multiline
        laptopInput.lineType = TMP_InputField.LineType.MultiLineNewline;
        laptopInput.interactable = true;

        // Load expected answer for word highlighting
        // Player figures out the answer from fragments they collected
      /*  if (currentLevelData != null && currentLevelData.correctAnswers.Count > 0)
            SetExpectedAnswer(currentLevelData.correctAnswers[0]);
        else
            Debug.LogWarning("No LevelData assigned to LaptopManager!"); *//////////////

        // Focus input field so blinking cursor appears
        StartCoroutine(FocusInputField());
    }

    // Closes laptop panel and resets everything
    public void CloseLaptop()
    {
        laptopPanel.SetActive(false);
        Time.timeScale = 1f;

        // Reset highlighting
        ResetHighlight();

        // Hide feedback
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    // =========================================
    // BLINKING CURSOR FOCUS
    // =========================================

    // Waits one frame then focuses input to show blinking cursor
    private IEnumerator FocusInputField()
    {
        // Wait for panel to fully open
        yield return new WaitForSecondsRealtime(0.3f);

        if (laptopInput == null)
        {
            Debug.LogError("laptopInput is NULL!");
            yield break;
        }

        // TMP handles everything natively
        laptopInput.ActivateInputField();
        laptopInput.Select();
    }


    // =========================================
    // ANSWER CHECKING
    // =========================================

    // Called by Submit button
    // Checks player answer against all accepted answers in LevelData
    public void CheckAnswer()
    {
        if (currentLevelData == null)
        {
            Debug.LogWarning("No LevelData assigned!");
            return;
        }

        // Get and normalize player input
        // Removes extra spaces and newlines for fair comparison
        string playerAnswer = laptopInput.text.Trim();
        playerAnswer = System.Text.RegularExpressions.Regex
            .Replace(playerAnswer, @"\s+", " ").ToLower();

        bool correct = false;

        // Check against every accepted answer in LevelData
        foreach (string ans in currentLevelData.correctAnswers)
        {
            // Normalize the correct answer too for fair comparison
            string normalizedAns = System.Text.RegularExpressions.Regex
                .Replace(ans.Trim(), @"\s+", " ").ToLower();

            if (playerAnswer == normalizedAns)
            {
                correct = true;
                break;
            }
        }

        if (correct)
        {
            // Green feedback for correct answer
            ShowFeedback("Yessir! Level Complete!", Color.green);

            // Flash panel green
            StartCoroutine(FlashPanel(Color.green));
            StartCoroutine(TriggerLevelComplete());
        }
        else
        {
            // Red feedback for wrong answer
            ShowFeedback("FAHHHH! Check your fragments!", Color.red);

            // Flash panel red so player clearly sees they are wrong
            StartCoroutine(FlashPanel(Color.red));

            // Shake the panel for extra feedback
            StartCoroutine(ShakePanel());

            // Track error for scoring
            /*  if (ScoreManager.Instance != null)
                  ScoreManager.Instance.AddError(); */ // Score manager standby 11111222333
        }
    }

    // Triggers level complete screen after delay
    private IEnumerator TriggerLevelComplete()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        CloseLaptop();

        // Show level complete UI
        /* if (LevelCompleteUI.Instance != null)
            LevelCompleteUI.Instance.ShowLevelComplete(currentLevelData);
        else
            Debug.LogWarning("LevelCompleteUI not found!"); */ // levelcompleteui standby  111112223333
    }

    // Flashes the laptop panel a color then returns to normal
private IEnumerator FlashPanel(Color flashColor)
{
    if (laptopPanelImage == null) yield break;

    // Flash to color
    laptopPanelImage.color = flashColor;

    // Wait
    yield return new WaitForSecondsRealtime(wrongFlashDuration);

    // Return to original color
    laptopPanelImage.color = originalPanelColor;
}

// Shakes the laptop panel left and right
private IEnumerator ShakePanel()
{
    if (laptopPanel == null) yield break;

    Vector3 originalPos = laptopPanel.transform.localPosition;
    float elapsed = 0f;
    float shakeDuration = 0.4f;   // How long shake lasts
    float shakeMagnitude = 10f;   // How far it shakes

    while (elapsed < shakeDuration)
    {
        // Move panel randomly left and right
        float x = originalPos.x + Random.Range(-shakeMagnitude, shakeMagnitude);
        laptopPanel.transform.localPosition = new Vector3(x, originalPos.y, originalPos.z);

        elapsed += Time.unscaledDeltaTime; // Use unscaled because timeScale is 0
        yield return null;
    }

    // Return panel to original position
    laptopPanel.transform.localPosition = originalPos;
}

    // =========================================
    // FEEDBACK
    // =========================================

    // Shows feedback text then hides it after duration
    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null)
        {
            Debug.LogWarning("FeedbackText is NULL!");
            return;
        }

        Debug.Log("Showing feedback: " + message);
        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);

        StopCoroutine("HideFeedback");
        StartCoroutine(HideFeedback());
    }

    // Hides feedback after set duration
    private IEnumerator HideFeedback()
    {
        yield return new WaitForSecondsRealtime(feedbackDuration);
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    // =========================================
    // WORD HIGHLIGHTING
    // =========================================

    // Sets the expected answer and prepares highlighting
    public void SetExpectedAnswer(string correctAnswer)
    {
        // Split expected answer into words
        // Handles spaces, newlines, and tabs
        expectedWords = correctAnswer.Trim().ToLower()
            .Split(new char[] { ' ', '\n', '\r', '\t' },
            System.StringSplitOptions.RemoveEmptyEntries);

        isHighlighting = true;
        highlightDisplay.text = "";

        Debug.Log("Loaded " + expectedWords.Length + " expected words");
    }

    // Fires every time player types a character
    private void OnInputChanged(string currentInput)
    {
        // Only run if highlighting is active
        if (!isHighlighting || expectedWords == null) return;

        // Split typed input into words
        string[] typedWords = currentInput
            .Split(new char[] { ' ', '\n', '\r', '\t' },
            System.StringSplitOptions.RemoveEmptyEntries);

        // Build colored result string word by word
        string result = "";

        for (int i = 0; i < typedWords.Length; i++)
        {
            string typedWord = typedWords[i];

            if (i < expectedWords.Length)
            {
                string expected = expectedWords[i];

                if (typedWord.ToLower() == expected.ToLower())
                {
                    // Exact match: green
                    result += ColorWord(typedWord, correctWordColor);
                }
                else if (expected.ToLower().StartsWith(typedWord.ToLower()))
                {
                    // Still typing this word (prefix): neutral white
                    result += ColorWord(typedWord, neutralWordColor);
                }
                else
                {
                    // Wrong word: red
                    result += ColorWord(typedWord, wrongWordColor);
                }
            }
            else
            {
                // More words than expected: red
                result += ColorWord(typedWord, wrongWordColor);
            }

            // Add space between words
            if (i < typedWords.Length - 1)
                result += " ";
        }

        // Update overlay with colored text
        highlightDisplay.text = result;
    }

    // Wraps a word in TMP rich text color tags
    private string ColorWord(string word, Color color)
    {
        string hex = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{hex}>{word}</color>";
    }

    // Resets highlighting state
    public void ResetHighlight()
    {
        highlightDisplay.text = "";
        isHighlighting = false;
        expectedWords = null;
    }
}