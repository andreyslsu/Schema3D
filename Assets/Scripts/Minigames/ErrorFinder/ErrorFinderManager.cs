using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ErrorFinderManager : MonoBehaviour
{
    public static ErrorFinderManager Instance;

    [Header("UI Panel")]
    public GameObject errorFinderPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI instructionText;

    [Header("SQL Display")]
    public Transform wordsContainer;
    public GameObject wordButtonPrefab;

    [Header("Progress")]
    public TextMeshProUGUI progressText;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;
    public float feedbackDuration = 1.5f;

    [Header("Effects")]
    public Image panelImage;
    public float flashDuration = 0.3f;
    private Color originalPanelColor;

    [Header("Colors")]
    public Color errorFoundColor = Color.green;
    public Color wrongClickColor = Color.red;
    public Color defaultColor = Color.white;
    public Color errorColor = new Color(1f, 0.5f, 0f);

    // State
    private ErrorFinderData currentData;
    private System.Action onSolvedCallback;
    private int errorsFound = 0;
    private int totalErrors = 0;
    private List<Button> wordButtons = new List<Button>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (errorFinderPanel != null)
            errorFinderPanel.SetActive(false);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (panelImage != null)
            originalPanelColor = panelImage.color;
    }

    // =========================================
    // OPEN / CLOSE
    // =========================================

    public void OpenErrorFinder(
        ErrorFinderData data,
        System.Action onSolved = null)
    {
        if (data == null) return;

        currentData = data;
        onSolvedCallback = onSolved;
        errorsFound = 0;
        totalErrors = 0;

        // Count total errors
        foreach (ErrorFinderData.SQLWord word in data.sqlWords)
            if (word.isError) totalErrors++;

        // Pause game
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (UIManager.Instance != null)
            UIManager.Instance.HideGameplayUI();

        SetupPuzzle();

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (titleText != null)
            titleText.text = data.puzzleTitle;

        if (instructionText != null)
            instructionText.text = data.instruction;

        UpdateProgress();

        errorFinderPanel.SetActive(true);
    }

    public void CloseErrorFinder()
    {
        errorFinderPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameplayUI();
    }

    // =========================================
    // SETUP
    // =========================================

    private void SetupPuzzle()
    {
        foreach (Transform child in wordsContainer)
            Destroy(child.gameObject);

        wordButtons.Clear();

        foreach (ErrorFinderData.SQLWord word in currentData.sqlWords)
        {
            GameObject obj = Instantiate(
                wordButtonPrefab, wordsContainer);
            Button btn = obj.GetComponent<Button>();
            TextMeshProUGUI text =
                obj.GetComponentInChildren<TextMeshProUGUI>();

            if (text != null) text.text = word.word;
            btn.image.color = defaultColor;

            ErrorFinderData.SQLWord captured = word;
            btn.onClick.AddListener(
                () => OnWordTapped(btn, captured));

            wordButtons.Add(btn);
        }
    }

    // =========================================
    // WORD TAP LOGIC
    // =========================================

    private void OnWordTapped(
        Button btn,
        ErrorFinderData.SQLWord word)
    {
        if (!btn.interactable) return;

        if (word.isError)
            StartCoroutine(HandleCorrectError(btn, word));
        else
            StartCoroutine(HandleWrongTap(btn));
    }

    private IEnumerator HandleCorrectError(
        Button btn,
        ErrorFinderData.SQLWord word)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayErrorFound();

        // Mark as found
        btn.image.color = errorFoundColor;
        btn.interactable = false;

        // Show correction
        TextMeshProUGUI text =
            btn.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
            text.text = word.word + " → " + word.correction;

        errorsFound++;
        UpdateProgress();

        ShowFeedback(
            currentData.correctFeedback +
            " '" + word.word + "' should be '" +
            word.correction + "'",
            Color.green);

        yield return new WaitForSecondsRealtime(0.5f);

        if (errorsFound >= totalErrors)
            StartCoroutine(HandleAllErrorsFound());
    }

    private IEnumerator HandleWrongTap(Button btn)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayWrongAnswer();

        btn.image.color = wrongClickColor;

        ShowFeedback(currentData.wrongFeedback, Color.red);

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddError();
            if (ScoreUI.Instance != null)
                ScoreUI.Instance.ForceRefresh();
        }

        yield return new WaitForSecondsRealtime(0.5f);
        btn.image.color = defaultColor;
    }

    private IEnumerator HandleAllErrorsFound()
    {
        if (panelImage != null)
        {
            panelImage.color = Color.green;
            yield return new WaitForSecondsRealtime(flashDuration);
            panelImage.color = originalPanelColor;
        }

        ShowFeedback(currentData.completedFeedback, Color.green);

        yield return new WaitForSecondsRealtime(feedbackDuration);

        CloseErrorFinder();

        // Give reward fragment
        if (!string.IsNullOrEmpty(currentData.rewardFragment))
        {
            InventoryManager.Instance.AddFragment(
                currentData.rewardFragment);
            UIManager.Instance.UpdateFragmentHotbar();
            UIManager.Instance.ShowFragment(
                currentData.rewardFragment);

            if (QuestManager.Instance != null)
                QuestManager.Instance.OnFragmentCollected();

            if (ScoreManager.Instance != null)
                ScoreManager.Instance.OnFragmentCollected();
        }

        // Update minigame quest ✓
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnMinigameCompleted();

        onSolvedCallback?.Invoke();
        onSolvedCallback = null;
    }

    // =========================================
    // PROGRESS
    // =========================================

    private void UpdateProgress()
    {
        if (progressText != null)
            progressText.text =
                "Errors Found: " +
                errorsFound + "/" + totalErrors;
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
}