using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TitleCard : MonoBehaviour
{
    public static TitleCard Instance;

    [Header("UI References")]
    public GameObject titleCardPanel;
    public TextMeshProUGUI locationText;
    public TextMeshProUGUI subtitleText;
    public Image topLine;
    public Image bottomLine;
    public Image backgroundOverlay;

    [Header("Animation Settings")]
    public float lineDuration = 0.3f;
    public float textFadeDuration = 0.4f;
    public float displayDuration = 2.5f;
    public float fadeOutDuration = 0.5f;

    [Header("Style")]
    public Color lineColor = Color.white;
    public Color textColor = Color.white;
    public Color overlayColor = new Color(0, 0, 0, 0.4f);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Debug.Log("hey hey");
    }

    private void Start()
    {
        Debug.Log("helllllo");
        if (titleCardPanel != null)
            titleCardPanel.SetActive(false);
    }

    // =========================================
    // PUBLIC SHOW
    // =========================================

    // Show with location only
    public void Show(string location,
        System.Action onComplete = null)
    {
        Show(location, "", onComplete);
    }

    // Show with location and subtitle
    public void Show(
        string location,
        string subtitle,
        System.Action onComplete = null)
    {
        StopAllCoroutines();
        StartCoroutine(
            PlayTitleCard(location, subtitle, onComplete));
    }

    // =========================================
    // ANIMATION
    // =========================================

    private IEnumerator PlayTitleCard(
        string location,
        string subtitle,
        System.Action onComplete)
    {
        // Setup
        titleCardPanel.SetActive(true);

        // Hide everything
        SetAlpha(backgroundOverlay, 0f);
        SetAlpha(locationText, 0f);
        SetAlpha(subtitleText, 0f);
        SetLineWidth(topLine, 0f);
        SetLineWidth(bottomLine, 0f);

        // Set text
        if (locationText != null)
        {
            locationText.text = location;
            locationText.color = textColor;
        }

        if (subtitleText != null)
        {
            subtitleText.text = subtitle;
            subtitleText.color = textColor;
            subtitleText.gameObject.SetActive(
                !string.IsNullOrEmpty(subtitle));
        }

        // Fade in overlay
        yield return StartCoroutine(
            FadeGraphic(backgroundOverlay,
            0f, overlayColor.a, 0.2f));

        // Animate lines expanding
        yield return StartCoroutine(
            ExpandLines(lineDuration));

        // Fade in location text
        yield return StartCoroutine(
            FadeGraphic(locationText,
            0f, 1f, textFadeDuration));

        // Fade in subtitle if exists
        if (!string.IsNullOrEmpty(subtitle))
        {
            yield return StartCoroutine(
                FadeGraphic(subtitleText,
                0f, 1f, textFadeDuration * 0.7f));
        }

        // Hold
        yield return new WaitForSecondsRealtime(
            displayDuration);

        // Fade out everything
        yield return StartCoroutine(
            FadeOutAll(fadeOutDuration));

        titleCardPanel.SetActive(false);

        onComplete?.Invoke();
    }

    // =========================================
    // ANIMATION HELPERS
    // =========================================

    private IEnumerator ExpandLines(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);

            SetLineWidth(topLine, t);
            SetLineWidth(bottomLine, t);

            yield return null;
        }

        SetLineWidth(topLine, 1f);
        SetLineWidth(bottomLine, 1f);
    }

    private IEnumerator FadeGraphic(
        Graphic graphic,
        float from,
        float to,
        float duration)
    {
        if (graphic == null) yield break;

        float elapsed = 0f;
        Color c = graphic.color;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            c.a = Mathf.Lerp(from, to, t);
            graphic.color = c;
            yield return null;
        }

        c.a = to;
        graphic.color = c;
    }

    private IEnumerator FadeOutAll(float duration)
    {
        float elapsed = 0f;

        Color locColor = locationText != null ?
            locationText.color : Color.white;
        Color subColor = subtitleText != null ?
            subtitleText.color : Color.white;
        Color topColor = topLine != null ?
            topLine.color : Color.white;
        Color botColor = bottomLine != null ?
            bottomLine.color : Color.white;
        Color bgColor = backgroundOverlay != null ?
            backgroundOverlay.color : Color.black;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float a = 1f - t;

            if (locationText != null)
            {
                locColor.a = a;
                locationText.color = locColor;
            }

            if (subtitleText != null)
            {
                subColor.a = a;
                subtitleText.color = subColor;
            }

            if (topLine != null)
            {
                topColor.a = a;
                topLine.color = topColor;
            }

            if (bottomLine != null)
            {
                botColor.a = a;
                bottomLine.color = botColor;
            }

            if (backgroundOverlay != null)
            {
                bgColor.a = overlayColor.a * (1f - t);
                backgroundOverlay.color = bgColor;
            }

            yield return null;
        }
    }

    // =========================================
    // UTILITIES
    // =========================================

    private void SetAlpha(Graphic graphic, float alpha)
    {
        if (graphic == null) return;
        Color c = graphic.color;
        c.a = alpha;
        graphic.color = c;
    }

    private void SetLineWidth(Image line, float percent)
    {
        if (line == null) return;
        line.fillAmount = percent;
    }
}