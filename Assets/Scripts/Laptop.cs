using UnityEngine;

// Laptop object opens coding panel for the player
public class Laptop : Interactable
{
    public override void Interact()
    {
        // Show laptop coding panel
        UIManager.Instance.ShowLaptop();

        Debug.Log("Laptop opened: " + gameObject.name);
    }
}