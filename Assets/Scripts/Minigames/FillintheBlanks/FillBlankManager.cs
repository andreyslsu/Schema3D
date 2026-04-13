using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class FillBlankManager : MonoBehaviour
{
    public static FillBlankManager Instance;

    [Header("UI Panel")]
    public GameObject fillBlankPanel;

    [Header("Question")]
    public TextMeshProUGUI questionText;

    [Header("Choice Buttons")]
    public List<Button> choiceButtons;
    public List<TextMeshProUGUI> choiceTexts;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;
    public float feedbackDuration = 1.5f;

    [Header("Effects")]
    public Image panelImage;
    public float flashDuration = 0.3f;
    private Color originalPanelColor;

    // State
    private FillBlankData currentData;
    private bool isSolved = false;
    private System.Action onSolvedCallback;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (fillBlankPanel != null)
            fillBlankPanel.SetActive(false);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (panelImage != null)
            originalPanelColor = panelImage.color;
    }

    // =========================================
    // OPEN/CLOSE UI
    // =========================================

    public void OpenFillBlank(
        FillBlankData data,
        System.Action onSolved = null)
    {
        if (data == null) return;

        currentData = data;
        onSolvedCallback = onSolved;

        // Pause game
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Hide hotbar
        if (UIManager.Instance != null)
            UIManager.Instance.HideGameplayUI();

        // Setup UI
        SetupQuestion();
        SetupChoices();

        // Hide feedback
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        fillBlankPanel.SetActive(true);
    }

    public void CloseFillBlank()
    {
        fillBlankPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Show hotbar
        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameplayUI();
    }

    // =========================================
    // INSPECTOR QUESTION
    // =========================================

    private void SetupQuestion()
    {
        if (questionText == null || currentData == null) return;

        // Highlight the blank in question
        string display = currentData.questionText.Replace(
            currentData.blankPlaceholder,
            "<color=yellow>___</color>");

        questionText.text = display;
    }

    private void SetupChoices()
    {
        if (currentData == null) return;

        // Shuffle choices
        List<string> shuffled = new List<string>(currentData.choices);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = shuffled[i];
            shuffled[i] = shuffled[j];
            shuffled[j] = temp;
        }

        // Setup each button
        for (int i = 0; i < choiceButtons.Count; i++)
        {
            if (i >= shuffled.Count)
            {
                // Hide unused buttons
                choiceButtons[i].gameObject.SetActive(false);
                continue;
            }

            choiceButtons[i].gameObject.SetActive(true);

            // Set text
            if (i < choiceTexts.Count)
                choiceTexts[i].text = shuffled[i];

            // Reset button color
            choiceButtons[i].image.color = Color.white;

            // Wire button click
            string captured = shuffled[i];
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(
                () => OnChoiceSelected(captured));
        }
    }

    // =========================================
    // ANSWER CHECK
    // =========================================

    private void OnChoiceSelected(string choice)
    {
        if (currentData == null) return;

        bool correct = choice.ToLower().Trim() ==
            currentData.correctAnswer.ToLower().Trim();

        if (correct)
            StartCoroutine(HandleCorrect(choice));
        else
            StartCoroutine(HandleWrong(choice));
    }

    private IEnumerator HandleCorrect(string choice)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayCorrectAnswer();
        // Flash green CORRECT ANSWER
        if (panelImage != null)
        {
            panelImage.color = Color.green;
            yield return new WaitForSecondsRealtime(flashDuration);
            panelImage.color = originalPanelColor;
        }

        // Show feedback
        ShowFeedback(currentData.correctFeedback, Color.green);

        yield return new WaitForSecondsRealtime(feedbackDuration);

        // Mark as solved
        isSolved = true;

        // Close panel
        CloseFillBlank();

        // Give reward fragment
        if (!string.IsNullOrEmpty(currentData.rewardFragment))
        {
            InventoryManager.Instance.AddFragment(
                currentData.rewardFragment);
            UIManager.Instance.UpdateFragmentHotbar();

            // Show fragment panel
            UIManager.Instance.ShowFragment(
                currentData.rewardFragment);

            // Update quest
            if (QuestManager.Instance != null)
                QuestManager.Instance.OnFragmentCollected();

            // Update score
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.OnFragmentCollected();
        }

        // Fire callback
        onSolvedCallback?.Invoke();
        onSolvedCallback = null;
    }

    private IEnumerator HandleWrong(string choice)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayWrongAnswer();

        // Flash red IF MALI
        if (panelImage != null)
        {
            panelImage.color = Color.red;
            yield return new WaitForSecondsRealtime(flashDuration);
            panelImage.color = originalPanelColor;
        }

        // Show feedback
        ShowFeedback(currentData.wrongFeedback, Color.red);

        // Add error to score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddError();
            if (ScoreUI.Instance != null)
                ScoreUI.Instance.ForceRefresh();
        }

        // Show wrong answer hint from professor
        LevelDialogue levelDialogue =
            FindFirstObjectByType<LevelDialogue>();
        if (levelDialogue != null)
            levelDialogue.ShowWrongAnswerHint();
    }

    // =========================================
    // FEEDBACK
    // =========================================

    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;

        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);

        StopCoroutine("HideFeedback");
        StartCoroutine(HideFeedback());
    }

    private IEnumerator HideFeedback()
    {
        yield return new WaitForSecondsRealtime(feedbackDuration);
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    // =========================================
    // UTILITIES
    // =========================================

    public bool IsSolved() => isSolved;

    public void ResetPuzzle()
    {
        isSolved = false;
    }
}