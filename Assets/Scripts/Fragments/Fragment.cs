using UnityEngine;

// Fragment object that displays a clue when picked up
public class Fragment : Interactable
{
    [TextArea]
    public string fragmentClue; // Text to show in fragment panel

    public override void Interact()
    {
        // Add fragment to inventory
        InventoryManager.Instance.AddFragment(fragmentClue);

        // Show fragment clue panel via UIManager
        UIManager.Instance.ShowFragment(fragmentClue);

        // Hide the fragment object in the scene after pickup
        gameObject.SetActive(false);
    }
}