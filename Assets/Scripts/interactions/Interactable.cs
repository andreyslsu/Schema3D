using UnityEngine;

// Base class for any object the player can interact with
public class Interactable : MonoBehaviour
{
    private Renderer rend;               // Reference to mesh renderer
    private Material originalMaterial;   // Original material
    public Material highlightMaterial;   // Material for glow effect

    private void Awake()
    {
        // Cache renderer and material
        rend = GetComponent<Renderer>();
        if (rend != null)
            originalMaterial = rend.material;
    }

    // Called when player looks at object
    public virtual void OnFocus()
    {
        if (rend != null && highlightMaterial != null)
            rend.material = highlightMaterial; // Apply glow
    }

    // Called when player looks away
    public virtual void OnLoseFocus()
    {
        if (rend != null && originalMaterial != null)
            rend.material = originalMaterial; // Remove glow
    }

    // Called when player interacts
    public virtual void Interact()
    {
        Debug.Log("Interacted with: " + gameObject.name);
    }
}