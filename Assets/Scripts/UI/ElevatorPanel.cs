using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class ElevatorPanel : Interactable
{
    [Header("References")]
    public Elevator elevator;

    [Header("Panel Image")]
    public Image panelImage;
    public float flashDuration = 0.5f;

    [Header("Feedback Colors")]
    public Color lockedColor = Color.red;
    public Color unlockedColor = Color.green;

    [Header("Elevator Type")]
    public bool isEntrancePanel = false;

    [Header("Messages")]
    public string noKeycardMessage = "Access Denied!";
    public string accessGrantedMessage = "Access Granted!";

    private Color originalPanelColor;
    private bool showingStatus = false; // Prevents label hiding during status

    private void Start()
    {
        if (panelImage != null)
            originalPanelColor = panelImage.color;
    }

    // Override OnLoseFocus to prevent hiding during status
    public override void OnLoseFocus()
    {
        // Only hide label if not showing status
        if (!showingStatus)
        {
            if (rend != null && originalMaterial != null)
                rend.material = originalMaterial;

            if (UIManager.Instance != null)
                UIManager.Instance.HideInteractLabel();

            if (labelObject != null)
                labelObject.SetActive(false);
        }
    }

    public override void Interact()
    {
        if (isEntrancePanel)
        {
            if (UIManager.Instance != null)
                UIManager.Instance.ShowInteractLabel(
                    "This is the entrance!");
            return;
        }

        if (Keycard.Instance == null)
        {
            Debug.LogWarning("Keycard Instance not found!");
            return;
        }

        if (Keycard.Instance.HasKeycard())
            OnKeycardAccepted();
        else
            OnKeycardDenied();
    }

    private void OnKeycardAccepted()
    {
        Keycard.Instance.UseKeycard();

        // Show status and keep visible
        StartCoroutine(ShowStatus(accessGrantedMessage, unlockedColor));

        // Flash green
        if (panelImage != null)
            StartCoroutine(FlashPanel(unlockedColor));

        StartCoroutine(OpenElevatorAfterFlash());

        Debug.Log("Keycard accepted!");
    }

    private void OnKeycardDenied()
    {
        // Show status and keep visible briefly
        StartCoroutine(ShowStatus(noKeycardMessage, lockedColor));

        // Flash red
        if (panelImage != null)
            StartCoroutine(FlashPanel(lockedColor));

        Debug.Log("No keycard!");
    }

    // Shows status text for duration then resets
    private IEnumerator ShowStatus(string message, Color color)
    {
        showingStatus = true;

        // Show world space label
        if (labelObject != null)
            labelObject.SetActive(true);

        if (labelText != null)
        {
            labelText.text = message;
            labelText.color = color;
        }

        // Show HUD label with status
        if (UIManager.Instance != null)
            UIManager.Instance.ShowInteractLabel(message);

        yield return new WaitForSecondsRealtime(2f);

        // Reset world space label
        if (labelText != null)
        {
            labelText.text = interactLabel;
            labelText.color = Color.white;
        }

        // Hide world space label
        if (labelObject != null)
            labelObject.SetActive(false);

        // Hide HUD label
        if (UIManager.Instance != null)
            UIManager.Instance.HideInteractLabel();

        showingStatus = false;
    }

    private IEnumerator FlashPanel(Color flashColor)
    {
        if (panelImage == null) yield break;

        panelImage.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        panelImage.color = originalPanelColor;
    }

    private IEnumerator OpenElevatorAfterFlash()
    {
        yield return new WaitForSeconds(flashDuration);

        if (elevator != null)
            elevator.OpenElevator();
        else
            Debug.LogWarning("Elevator not assigned!");
    }
}