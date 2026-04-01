using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TypeMatchData", menuName = "Scriptable Objects/TypeMatchData")]
public class TypeMatchData : ScriptableObject
{
    [System.Serializable]
    public class MatchPair
    {
        public string leftItem;   // column name 
        public string rightItem;  // data type 
    }

    [Header("Puzzle Info")]
    public string puzzleTitle = "Match the column to its data type!";

    [Header("Pairs")]
    public List<MatchPair> pairs;

    [Header("Reward")]
    [TextArea(2, 4)]
    public string rewardFragment;

    [Header("Feedback")]
    public string correctFeedback = "Perfect match!";
    public string wrongFeedback = "Wrong match! Try again!";
    public string completedFeedback = "All matched! Great work!";
}
