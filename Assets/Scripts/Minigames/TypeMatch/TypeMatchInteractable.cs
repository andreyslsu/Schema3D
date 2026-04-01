using UnityEngine;

public class TypeMatchInteractable : Interactable
{
    [Header("Puzzle Data")]
    public TypeMatchData typeMatchData;

    [Header("Solved State")]
    public GameObject solvedIndicator;
    public string solvedLabel = "Already Solved";

    private bool isSolved = false;

    private void Start()
    {
        interactLabel = "Match the Types";

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

        if (TypeMatchManager.Instance == null)
        {
            Debug.LogWarning("TypeMatchManager not found!");
            return;
        }

        if (typeMatchData == null)
        {
            Debug.LogWarning("TypeMatchData not assigned!");
            return;
        }

        TypeMatchManager.Instance.OpenTypeMatch(
            typeMatchData, OnPuzzleSolved);
    }

    private void OnPuzzleSolved()
    {
        isSolved = true;
        interactLabel = solvedLabel;

        if (solvedIndicator != null)
            solvedIndicator.SetActive(true);
    }
}