using UnityEngine;
using UnityEngine.InputSystem; // Unity 6 new Input System
using UnityEngine.EventSystems; // Required for IsPointerOverGameObject

// Handles all player interactions: fragments and laptop
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f;     // Max distance to interact
    public LayerMask interactLayer;         // Layer for fragments/laptop

    private Camera cam;                     // Reference to player's camera
    private Interactable currentInteractable; // Object player is looking at

    private void Awake()
    {
        cam = Camera.main; // Auto-find main camera
    }

    private void Update()
    {
        // -----------------------------
        // 1?? Raycast forward to detect interactables
        // -----------------------------
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                // New interactable in view
                if (currentInteractable != interactable)
                {
                    if (currentInteractable != null)
                        currentInteractable.OnLoseFocus(); // Remove glow from old

                    currentInteractable = interactable;
                    currentInteractable.OnFocus(); // Apply glow
                }
            }
            else if (currentInteractable != null)
            {
                // Nothing interactable hit ? remove glow
                currentInteractable.OnLoseFocus();
                currentInteractable = null;
            }
        }
        else if (currentInteractable != null)
        {
            // Nothing hit ? remove glow
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }

        // -----------------------------
        // 2?? Detect interaction input (PC + Mobile)
        // -----------------------------
        bool isClicked = false;

        // PC: Left mouse click
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            isClicked = true;

        // Mobile: Touch screen tap
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            int fingerId = Touchscreen.current.primaryTouch.touchId.ReadValue(); 
            if (!EventSystem.current.IsPointerOverGameObject(fingerId))
            {
                isClicked = true;
            }
        }

        // Trigger interaction
        if (currentInteractable != null && isClicked)
        {
            currentInteractable.Interact();
        }
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Ignore interaction if touching UI
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (currentInteractable != null)
                currentInteractable.Interact();
        }
    }
}
