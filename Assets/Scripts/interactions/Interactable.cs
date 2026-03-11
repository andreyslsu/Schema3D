using UnityEngine;
using TMPro;

// Base class for any object the player can interact with
public class Interactable : MonoBehaviour
{
    [Header("Highlight")]
    protected Renderer rend;
    protected Material originalMaterial;
    public Material highlightMaterial;

    [Header("Interaction Label")]
    public string interactLabel = "Press to Interact";

    [Header("World Space Label (Optional)")]
    public GameObject labelObject;        // Optional world space canvas
    public TextMeshProUGUI labelText;     // Optional world space TMP
                                          // Leave empty if not needed

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
            originalMaterial = rend.material;

        // Hide world space label at start if assigned
        if (labelObject != null)
            labelObject.SetActive(false);
    }

    // Called when player looks at object
    public virtual void OnFocus()
    {
        // Highlight glow
        if (rend != null && highlightMaterial != null)
            rend.material = highlightMaterial;

        // Show HUD label on screen center
        if (UIManager.Instance != null)
            UIManager.Instance.ShowInteractLabel(interactLabel);

        // Show world space label if assigned
        // Only ElevatorPanel uses this
        if (labelObject != null)
        {
            labelObject.SetActive(true);
            if (labelText != null)
                labelText.text = interactLabel;
        }
    }

    // Called when player looks away
    public virtual void OnLoseFocus()
    {
        // Remove glow
        if (rend != null && originalMaterial != null)
            rend.material = originalMaterial;

        // Hide HUD label
        if (UIManager.Instance != null)
            UIManager.Instance.HideInteractLabel();

        // Hide world space label if assigned
        if (labelObject != null)
            labelObject.SetActive(false);
    }

    // Called when player interacts
    public virtual void Interact()
    {
        Debug.Log("Interacted with: " + gameObject.name);
    }
}