using UnityEngine;

public class ErrorFinderInteractable : Interactable
{
    [Header("Puzzle Data")]
    public ErrorFinderData errorFinderData;

    [Header("Solved State")]
    public GameObject solvedIndicator;
    public string solvedLabel = "Already Solved";

    private bool isSolved = false;

    private void Start()
    {
        interactLabel = "Find the Errors";

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

        if (ErrorFinderManager.Instance == null)
        {
            Debug.LogWarning("ErrorFinderManager not found!");
            return;
        }

        if (errorFinderData == null)
        {
            Debug.LogWarning("ErrorFinderData not assigned!");
            return;
        }

        ErrorFinderManager.Instance.OpenErrorFinder(
            errorFinderData, OnPuzzleSolved);
    }

    private void OnPuzzleSolved()
    {
        isSolved = true;
        interactLabel = solvedLabel;

        if (solvedIndicator != null)
            solvedIndicator.SetActive(true);
    }
}