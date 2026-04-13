using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TypeMatchManager : MonoBehaviour
{
    public static TypeMatchManager Instance;

    [Header("UI Panel")]
    public GameObject typeMatchPanel;
    public TextMeshProUGUI titleText;

    [Header("Left Column (Items)")]
    public Transform leftContainer;
    public GameObject leftButtonPrefab;

    [Header("Right Column (Types)")]
    public Transform rightContainer;
    public GameObject rightButtonPrefab;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;
    public float feedbackDuration = 1.5f;

    [Header("Effects")]
    public Image panelImage;
    public float flashDuration = 0.3f;
    private Color originalPanelColor;

    [Header("Colors")]
    public Color selectedColor = Color.yellow;
    public Color matchedColor = Color.green;
    public Color wrongColor = Color.red;
    public Color defaultColor = Color.white;

    // State
    private TypeMatchData currentData;
    private System.Action onSolvedCallback;

    private Button selectedLeftButton = null;
    private string selectedLeftText = "";
    private List<Button> leftButtons = new List<Button>();
    private List<Button> rightButtons = new List<Button>();
    private int matchesCompleted = 0;
    private int totalMatches = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (typeMatchPanel != null)
            typeMatchPanel.SetActive(false);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (panelImage != null)
            originalPanelColor = panelImage.color;
    }

    // =========================================
    // OPEN / CLOSE
    // =========================================

    public void OpenTypeMatch(
        TypeMatchData data,
        System.Action onSolved = null)
    {
        if (data == null) return;

        currentData = data;
        onSolvedCallback = onSolved;
        matchesCompleted = 0;
        totalMatches = data.pairs.Count;
        selectedLeftButton = null;
        selectedLeftText = "";

        // Pause game
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Hide hotbar
        if (UIManager.Instance != null)
            UIManager.Instance.HideGameplayUI();

        // Setup UI
        SetupPuzzle();

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (titleText != null)
            titleText.text = data.puzzleTitle;

        typeMatchPanel.SetActive(true);
    }

    public void CloseTypeMatch()
    {
        typeMatchPanel.SetActive(false);

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
        // Clear old buttons
        foreach (Transform child in leftContainer)
            Destroy(child.gameObject);
        foreach (Transform child in rightContainer)
            Destroy(child.gameObject);

        leftButtons.Clear();
        rightButtons.Clear();

        // Shuffle right items
        List<string> rightItems = new List<string>();
        foreach (TypeMatchData.MatchPair pair in currentData.pairs)
            rightItems.Add(pair.rightItem);

        for (int i = rightItems.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = rightItems[i];
            rightItems[i] = rightItems[j];
            rightItems[j] = temp;
        }

        // Create left buttons (column names)
        foreach (TypeMatchData.MatchPair pair in currentData.pairs)
        {
            GameObject obj = Instantiate(leftButtonPrefab, leftContainer);
            Button btn = obj.GetComponent<Button>();
            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();

            if (text != null) text.text = pair.leftItem;
            btn.image.color = defaultColor;

            string captured = pair.leftItem;
            btn.onClick.AddListener(() => OnLeftSelected(btn, captured));
            leftButtons.Add(btn);
        }

        // Create right buttons (data types)
        for (int i = 0; i < rightItems.Count; i++)
        {
            GameObject obj = Instantiate(rightButtonPrefab, rightContainer);
            Button btn = obj.GetComponent<Button>();
            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();

            if (text != null) text.text = rightItems[i];
            btn.image.color = defaultColor;

            string captured = rightItems[i];
            btn.onClick.AddListener(() => OnRightSelected(btn, captured));
            rightButtons.Add(btn);
        }
    }

    // =========================================
    // MATCHING LOGIC
    // =========================================

    private void OnLeftSelected(Button btn, string itemText)
    {
        // Deselect previous
        if (selectedLeftButton != null)
            selectedLeftButton.image.color = defaultColor;

        // Select new
        selectedLeftButton = btn;
        selectedLeftText = itemText;
        btn.image.color = selectedColor;

        ShowFeedback("Now select the matching data type!", Color.white);
    }

    private void OnRightSelected(Button btn, string typeText)
    {
        if (selectedLeftButton == null)
        {
            ShowFeedback("Select a column first!", Color.yellow);
            return;
        }

        // Check if match is correct
        bool correct = false;
        foreach (TypeMatchData.MatchPair pair in currentData.pairs)
        {
            if (pair.leftItem == selectedLeftText &&
                pair.rightItem == typeText)
            {
                correct = true;
                break;
            }
        }

        if (correct)
            StartCoroutine(HandleCorrectMatch(
                selectedLeftButton, btn, typeText));
        else
            StartCoroutine(HandleWrongMatch(btn));
    }

    private IEnumerator HandleCorrectMatch(
        Button leftBtn, Button rightBtn, string typeText)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMatchCorrect();

        // Mark both as matched
        leftBtn.image.color = matchedColor;
        rightBtn.image.color = matchedColor;

        // Disable matched buttons
        leftBtn.interactable = false;
        rightBtn.interactable = false;

        selectedLeftButton = null;
        selectedLeftText = "";

        matchesCompleted++;

        ShowFeedback(
            currentData.correctFeedback + " " +
            matchesCompleted + "/" + totalMatches,
            Color.green);

        yield return new WaitForSecondsRealtime(0.5f);

        // Check if all matched
        if (matchesCompleted >= totalMatches)
            StartCoroutine(HandleAllMatched());
    }

    private IEnumerator HandleWrongMatch(Button rightBtn)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMatchWrong();
        // Flash wrong color
        rightBtn.image.color = wrongColor;
        selectedLeftButton.image.color = wrongColor;

        ShowFeedback(currentData.wrongFeedback, Color.red);

        // Add error to score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddError();
            if (ScoreUI.Instance != null)
                ScoreUI.Instance.ForceRefresh();
        }

        yield return new WaitForSecondsRealtime(0.5f);

        // Reset colors
        rightBtn.image.color = defaultColor;
        if (selectedLeftButton != null)
            selectedLeftButton.image.color = selectedColor;

        // Show wrong hint
        LevelDialogue levelDialogue =
            FindFirstObjectByType<LevelDialogue>();
        if (levelDialogue != null)
            levelDialogue.ShowWrongAnswerHint();
    }

    private IEnumerator HandleAllMatched()
    {
        // Flash panel green
        if (panelImage != null)
        {
            panelImage.color = Color.green;
            yield return new WaitForSecondsRealtime(flashDuration);
            panelImage.color = originalPanelColor;
        }

        ShowFeedback(currentData.completedFeedback, Color.green);

        yield return new WaitForSecondsRealtime(feedbackDuration);

        // Close panel
        CloseTypeMatch();

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

        // Fire callback
        onSolvedCallback?.Invoke();
        onSolvedCallback = null;
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