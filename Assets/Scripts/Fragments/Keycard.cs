using UnityEngine;

// Tracks whether player has keycard
// Stored in hotbar just like fragments
public class Keycard : MonoBehaviour
{
    public static Keycard Instance;

    private bool hasKeycard = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Called by LaptopManager when correct answer submitted
    public void GiveKeycard()
    {
        hasKeycard = true;
        Debug.Log("Keycard received!");

        // Show keycard panel just like fragment panel
        if (UIManager.Instance != null)
        {
            // Add keycard to hotbar
            UIManager.Instance.AddKeycardToHotbar();

            // Show keycard panel
            UIManager.Instance.ShowKeycardPanel();
        }
        else
            Debug.LogWarning("UIManager not found!");
    }

    // Returns whether player has keycard
    public bool HasKeycard()
    {
        return hasKeycard;
    }

    // Resets keycard after used
    public void UseKeycard()
    {
        hasKeycard = false;

        // Remove keycard from hotbar after use
        if (UIManager.Instance != null)
            UIManager.Instance.RemoveKeycardFromHotbar();

        Debug.Log("Keycard used!");
    }
}