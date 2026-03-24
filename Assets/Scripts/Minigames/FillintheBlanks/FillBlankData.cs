using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FillBlankData", menuName = "Scriptable Objects/FillBlankData")]
public class FillBlankData : ScriptableObject
{
    [Header("Question")]
    [TextArea(2, 4)]
    public string questionText;        // SQL with blank
    public string blankPlaceholder = "_";  // what blank looks like

    [Header("Choices")]
    public List<string> choices;       // all answer options

    [Header("Answer")]
    public string correctAnswer;       // must match one choice

    [Header("Reward")]
    [TextArea(2, 4)]
    public string rewardFragment;      // fragment clue given on correct ✓

    [Header("Feedback")]
    public string correctFeedback = "Correct!";
    public string wrongFeedback = "Try again!";
}
