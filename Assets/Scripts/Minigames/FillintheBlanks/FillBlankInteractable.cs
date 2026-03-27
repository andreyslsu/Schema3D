using UnityEngine;

public class FillBlankInteractable : Interactable
{
    [Header("Puzzle Data")]
    public FillBlankData fillBlankData;

    [Header("Solved State")]
    public GameObject solvedIndicator;  // optional glow or checkmark
    public string solvedLabel = "Already Solved";

    private bool isSolved = false;

    private void Start()
    {
        interactLabel = "Solve Puzzle";

        // Hide solved indicator at start
        if (solvedIndicator != null)
            solvedIndicator.SetActive(false);
    }

    public override void Interact()
    {
        if (isSolved)
        {
            if (UIManager.Instance != null)
                UIManager.Instance.ShowInteractLabel(solvedLabel);
            return;
        }

        if (FillBlankManager.Instance == null)
        {
            Debug.LogWarning("FillBlankManager not found!");
            return;
        }

        if (fillBlankData == null)
        {
            Debug.LogWarning("FillBlankData not assigned!");
            return;
        }

        FillBlankManager.Instance.OpenFillBlank(
            fillBlankData, OnPuzzleSolved);
    }

    private void OnPuzzleSolved()
    {
        isSolved = true;
        interactLabel = solvedLabel;

        // Show solved indicator
        if (solvedIndicator != null)
            solvedIndicator.SetActive(true);

        if (QuestManager.Instance != null)
            QuestManager.Instance.OnMinigameCompleted();

        Debug.Log("Puzzle solved!");
    }
}