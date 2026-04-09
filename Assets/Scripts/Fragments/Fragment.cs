using UnityEngine;

public class Fragment : Interactable
{
    [TextArea]
    public string fragmentClue; // string text to show as clue for fragment

    public override void Interact()
    {
        // add inventory
        InventoryManager.Instance.AddFragment(fragmentClue);

        // show inventory ins from ui man
        UIManager.Instance.ShowFragment(fragmentClue);

        // fragment tracker
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnFragmentCollected();
        else
            Debug.LogWarning("QuestManager not found!");

        // track for bonus points for every fragment
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnFragmentCollected();
        else
            Debug.LogWarning("ScoreManager not found!");

        // dialogue for found fragments

        LevelDialogue levelDialogue =
        FindFirstObjectByType<LevelDialogue>();
        if (levelDialogue != null)
            levelDialogue.ShowFirstFragmentHint();

        gameObject.SetActive(false);
    }
}