using UnityEngine;

// Fragment object that displays a clue when picked up
public class Fragment : Interactable
{
    [TextArea]
    public string fragmentClue; // Text to show in fragment panel

    public override void Interact()
    {
        // Add to inventory
        InventoryManager.Instance.AddFragment(fragmentClue);

        // Show fragment clue panel
        UIManager.Instance.ShowFragment(fragmentClue);

        // Update quest tracker
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnFragmentCollected();
        else
            Debug.LogWarning("QuestManager not found!");

        // Track score bonus for fragment collection speed
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnFragmentCollected();
        else
            Debug.LogWarning("ScoreManager not found!");

        LevelDialogue levelDialogue =
        FindFirstObjectByType<LevelDialogue>();
        if (levelDialogue != null)
            levelDialogue.ShowFirstFragmentHint();

        gameObject.SetActive(false);
    }
}