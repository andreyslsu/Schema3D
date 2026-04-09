using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ResultVisualizationUI : MonoBehaviour
{
    public static ResultVisualizationUI Instance;

    [Header("Panel")]
    public GameObject resultPanel;
    public Image fadeOverlay;

    [Header("Content")]
    public Image resultImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    [Header("Button")]
    public Button closeButton;
    public TextMeshProUGUI closeButtonText;

    [Header("Animation")]
    public float fadeInDuration = 0.5f;
    public CanvasGroup panelCanvasGroup;

    // Callback when player closes ✓
    private System.Action onClosedCallback;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(
                CloseResultPanel);

        // Hide fade overlay ✓
        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
        }
    }

    // =========================================
    // SHOW RESULT
    // =========================================

    public void ShowResult(
        LevelData levelData,
        System.Action onClosed = null)
    {
        if (levelData == null)
        {
            // No result image assigned
            // just fire callback 
            onClosed?.Invoke();
            return;
        }

        // If no result image assigned
        // skip panel and give keycard 
        if (levelData.resultImage == null)
        {
            onClosed?.Invoke();
            return;
        }

        onClosedCallback = onClosed;

        // Set content ✓
        if (resultImage != null)
            resultImage.sprite = levelData.resultImage;

        if (titleText != null)
            titleText.text = string.IsNullOrEmpty(
                levelData.resultTitle)
                ? "Query Executed Successfully!"
                : levelData.resultTitle;

        if (descriptionText != null)
            descriptionText.text = string.IsNullOrEmpty(
                levelData.resultDescription)
                ? "Your SQL query was executed.\nCheck the result below."
                : levelData.resultDescription;

        // Show panel ✓
        resultPanel.SetActive(true);

        // Pause game ✓
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Hide gameplay UI ✓
        if (UIManager.Instance != null)
            UIManager.Instance.HideGameplayUI();

        // Fade in ✓
        StartCoroutine(FadeIn());
    }

    // =========================================
    // CLOSE RESULT
    // =========================================

    public void CloseResultPanel()
    {
        StartCoroutine(FadeOutAndClose());
    }

    private IEnumerator FadeOutAndClose()
    {
        // Fade out ✓
        if (panelCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                panelCanvasGroup.alpha =
                    1f - Mathf.Clamp01(
                        elapsed / fadeInDuration);
                yield return null;
            }
            panelCanvasGroup.alpha = 0f;
        }

        // Hide panel 
        resultPanel.SetActive(false);

        // Resume game 
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Show gameplay UI 
        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameplayUI();

        // Fire callback → gives keycard 
        onClosedCallback?.Invoke();
        onClosedCallback = null;
    }

    // =========================================
    // FADE IN
    // =========================================

    private IEnumerator FadeIn()
    {
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;

            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                panelCanvasGroup.alpha =
                    Mathf.Clamp01(
                        elapsed / fadeInDuration);
                yield return null;
            }

            panelCanvasGroup.alpha = 1f;
        }
    }
}