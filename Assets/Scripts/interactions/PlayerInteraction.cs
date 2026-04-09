using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.EventSystems; 

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f;   
    public LayerMask interactLayer;         

    private Camera cam;                    
    private Interactable currentInteractable; 

    private void Awake()
    {
        cam = Camera.main; // Auto-find main camera
    }

    private void Update()
    {
//=======================================
// 1 Raycast forward to detect interactables
//=======================================
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

// =======================================
//  Detect interaction input (PC + Mobile)
// =======================================
        bool isClicked = false;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            isClicked = true;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            int fingerId = Touchscreen.current.primaryTouch.touchId.ReadValue(); 
            if (!EventSystem.current.IsPointerOverGameObject(fingerId))
            {
                isClicked = true;
            }
        }

        if (currentInteractable != null && isClicked)
        {
            currentInteractable.Interact();
        }
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (currentInteractable != null)
                currentInteractable.Interact();
        }
    }
}
