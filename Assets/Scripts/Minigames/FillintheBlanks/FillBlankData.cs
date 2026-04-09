using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FillBlankData", menuName = "Scriptable Objects/FillBlankData")]
public class FillBlankData : ScriptableObject
{
    [Header("Question")]
    [TextArea(2, 4)]
    public string questionText;
    public string blankPlaceholder = "_";  

    [Header("Choices")]
    public List<string> choices;     

    [Header("Answer")]
    public string correctAnswer;      

    [Header("Reward")]
    [TextArea(2, 4)]
    public string rewardFragment;     

    [Header("Feedback")]
    public string correctFeedback = "Correct!";
    public string wrongFeedback = "Try again!";
}
