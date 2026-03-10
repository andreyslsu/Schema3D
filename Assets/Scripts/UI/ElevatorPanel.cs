using UnityEngine;
using TMPro;

// Attach this to ANY object you want to act as keycard reader
// Works on elevator panel, door panel, terminal, anything
public class ElevatorPanel : Interactable
{
    [Header("Panel UI")]
    public GameObject panelUI;               // UI that shows when interacting
    public TextMeshProUGUI statusText;        // Shows status text

    [Header("References")]
    public Elevator elevator;                 // Drag elevator here

    [Header("Feedback Colors")]
    public Color lockedColor = Color.red;
    public Color unlockedColor = Color.green;

    [Header("Messages")]
    public string noKeycardMessage = "Access Denied!\nSolve the laptop puzzle first.";
    public string accessGrantedMessage = "Access Granted!";

    // Called when player interacts with this object
    public override void Interact()
    {
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

    // Player has keycard
    private void OnKeycardAccepted()
    {
        // Use keycard
        Keycard.Instance.UseKeycard();

        // Update status
        if (statusText != null)
        {
            statusText.text = accessGrantedMessage;
            statusText.color = unlockedColor;
        }

        // Show panel UI briefly
        if (panelUI != null)
            panelUI.SetActive(true);

        // Open elevator
        if (elevator != null)
            elevator.OpenElevator();
        else
            Debug.LogWarning("Elevator not assigned!");

        Debug.Log("Keycard accepted!");
    }

    // Player has no keycard
    private void OnKeycardDenied()
    {
        // Show panel UI with denied message
        if (panelUI != null)
            panelUI.SetActive(true);

        if (statusText != null)
        {
            statusText.text = noKeycardMessage;
            statusText.color = lockedColor;
        }

        Debug.Log("No keycard! Solve laptop first!");
    }
}