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
    public GameObject labelObject;       
    public TextMeshProUGUI labelText;     
                                          

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
            originalMaterial = rend.material;

        if (labelObject != null)
            labelObject.SetActive(false);
    }

    public virtual void OnFocus()
    {
        if (rend != null && highlightMaterial != null)
            rend.material = highlightMaterial;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowInteractLabel(interactLabel);

        if (labelObject != null)
        {
            labelObject.SetActive(true);
            if (labelText != null)
                labelText.text = interactLabel;
        }
    }

    public virtual void OnLoseFocus()
    {
        if (rend != null && originalMaterial != null)
            rend.material = originalMaterial;

        if (UIManager.Instance != null)
            UIManager.Instance.HideInteractLabel();

        if (labelObject != null)
            labelObject.SetActive(false);
    }

    public virtual void Interact()
    {
        Debug.Log("Interacted with: " + gameObject.name);
    }
}