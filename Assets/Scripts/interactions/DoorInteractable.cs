using UnityEngine;

public class DoorInteractable : Interactable
{
    [Header("Door Reference")]
    public DoorAnimator doorAnimator;

    [Header("Labels")]
    public string openLabel = "Open Door";
    public string closeLabel = "Close Door";
    public string alreadyOpenLabel = "Door is Open";

    private void Start()
    {
        // Set default label
        interactLabel = openLabel;
    }

    public override void OnFocus()
    {
        // Update label based on current door state
        if (doorAnimator != null)
        {
            if (doorAnimator.IsOpen())
                interactLabel = closeLabel;
            else
                interactLabel = openLabel;
        }

        // Call base to show highlight and HUD label
        base.OnFocus();
    }

    public override void OnLoseFocus()
    {
        // Call base to hide highlight and HUD label
        base.OnLoseFocus();
    }

    public override void Interact()
    {
        if (doorAnimator == null)
        {
            Debug.LogWarning("DoorAnimator not assigned!");
            return;
        }

        doorAnimator.ToggleDoor();

        // Update label immediately after interact
        interactLabel = doorAnimator.IsOpen()
            ? closeLabel
            : openLabel;

        // Update HUD label
        if (UIManager.Instance != null)
            UIManager.Instance.ShowInteractLabel(interactLabel);
    }
}