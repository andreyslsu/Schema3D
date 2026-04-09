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
    
        interactLabel = openLabel;
    }

    public override void OnFocus()
    {
        
        if (doorAnimator != null)
        {
            if (doorAnimator.IsOpen())
                interactLabel = closeLabel;
            else
                interactLabel = openLabel;
        }

       
        base.OnFocus();
    }

    public override void OnLoseFocus()
    {
        
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

       
        interactLabel = doorAnimator.IsOpen()
            ? closeLabel
            : openLabel;

 
        if (UIManager.Instance != null)
            UIManager.Instance.ShowInteractLabel(interactLabel);
    }
}