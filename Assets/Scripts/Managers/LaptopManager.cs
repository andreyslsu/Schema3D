using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class LaptopManager : MonoBehaviour
{
    public static LaptopManager Instance;

    [Header("Laptop UI")]
    public GameObject laptopPanel;
    public TMP_InputField laptopInput;
    public TextMeshProUGUI feedbackText;

    [Header("Feedback Settings")]
    public float feedbackDuration = 2f;

    [Header("Level Data")]
    public LevelData currentLevelData;

    [Header("Wrong Answer Effects")]
    public Image laptopPanelImage;
    public float wrongFlashDuration = 0.5f;
    private Color originalPanelColor;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (laptopPanelImage != null)
            originalPanelColor = laptopPanelImage.color;

        laptopInput.lineType = TMP_InputField.LineType.MultiLineNewline;

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
        else
            Debug.LogWarning("FeedbackText is NULL!");
    }

    // =========================================
    // OPEN / CLOSE LAPTOP
    // =========================================

    public void OpenLaptop()
    {
        laptopPanel.SetActive(true);

        laptopInput.text = "";
        laptopInput.lineType = TMP_InputField.LineType.MultiLineNewline;
        laptopInput.interactable = true;

        StartCoroutine(FocusInputField());
    }

    public void CloseLaptop()
    {
        laptopPanel.SetActive(false);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    // =========================================
    // CURSOR FOCUS
    // =========================================

    private IEnumerator FocusInputField()
    {
        yield return new WaitForSecondsRealtime(0.3f);

        if (laptopInput == null)
        {
            Debug.LogError("laptopInput is NULL!");
            yield break;
        }

        laptopInput.ActivateInputField();
        laptopInput.Select();
    }

    // =========================================
    // ANSWER CHECKING
    // =========================================

    public void CheckAnswer()
    {
        if (currentLevelData == null)
        {
            Debug.LogWarning("No LevelData assigned!");
            return;
        }

        // Normalize player input
        string playerAnswer = laptopInput.text.Trim();
        playerAnswer = System.Text.RegularExpressions.Regex
            .Replace(playerAnswer, @"\s+", " ").ToLower();

        bool correct = false;

        foreach (string ans in currentLevelData.correctAnswers)
        {
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
            ShowFeedback("Yessir! Level Complete!", Color.green);
            StartCoroutine(FlashPanel(Color.green));
            StartCoroutine(TriggerLevelComplete());
        }
        else
        {
            ShowFeedback("FAHHHH! Check your fragments!", Color.red);
            StartCoroutine(FlashPanel(Color.red));
            StartCoroutine(ShakePanel());

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddError();

                if (ScoreUI.Instance != null)
                    ScoreUI.Instance.ForceRefresh();
            }
            else
                Debug.LogWarning("ScoreManager not found!");
        }
    }

    // =========================================
    // LEVEL COMPLETE
    // =========================================

    private IEnumerator TriggerLevelComplete()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        CloseLaptop();
 
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnLaptopQuestComplete();
        else
            Debug.LogWarning("QuestManager not found!");

        if (ResultVisualizationUI.Instance != null &&
            currentLevelData != null)
        {
            ResultVisualizationUI.Instance.ShowResult(
                currentLevelData,
                () =>
                {   
                    GiveKeycardAfterResult();
                });
        }
        else
        { 
            GiveKeycardAfterResult();
        }
    }

    private void GiveKeycardAfterResult()
    {
        if (Keycard.Instance != null)
            Keycard.Instance.GiveKeycard();
        else
            Debug.LogWarning("Keycard not found!");
    }

    // =========================================
    // PANEL EFFECTS
    // =========================================

    private IEnumerator FlashPanel(Color flashColor)
    {
        if (laptopPanelImage == null) yield break;

        laptopPanelImage.color = flashColor;
        yield return new WaitForSecondsRealtime(wrongFlashDuration);
        laptopPanelImage.color = originalPanelColor;
    }
    private IEnumerator ShakePanel()
    {
        if (laptopPanel == null) yield break;

        Vector3 originalPos = laptopPanel.transform.localPosition;
        float elapsed = 0f;
        float shakeDuration = 0.4f;
        float shakeMagnitude = 10f;

        while (elapsed < shakeDuration)
        {
            float x = originalPos.x + Random.Range(-shakeMagnitude, shakeMagnitude);
            laptopPanel.transform.localPosition =
                new Vector3(x, originalPos.y, originalPos.z);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        laptopPanel.transform.localPosition = originalPos;
    }

    // =========================================
    // FEEDBACK
    // =========================================

    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null)
        {
            Debug.LogWarning("FeedbackText is NULL!");
            return;
        }

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