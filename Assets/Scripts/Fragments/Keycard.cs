using UnityEngine;

public class Keycard : MonoBehaviour
{
    public static Keycard Instance;

    private bool hasKeycard = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GiveKeycard()
    {
        hasKeycard = true;
        Debug.Log("Keycard received!");

       
        if (UIManager.Instance != null)
        {           
            UIManager.Instance.AddKeycardToHotbar();
  
            UIManager.Instance.ShowKeycardPanel();
        }
        else
            Debug.LogWarning("UIManager not found!");

        LevelDialogue levelDialogue =
        FindFirstObjectByType<LevelDialogue>();
        if (levelDialogue != null)
            levelDialogue.ShowKeycardHint();
    }

    public bool HasKeycard()
    {
        return hasKeycard;
    }

    public void UseKeycard()
    {
        hasKeycard = false;

        if (UIManager.Instance != null)
            UIManager.Instance.RemoveKeycardFromHotbar();

        Debug.Log("Keycard used!");
    }
}