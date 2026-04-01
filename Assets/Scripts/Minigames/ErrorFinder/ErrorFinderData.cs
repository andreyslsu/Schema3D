using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ErrorFinderData", menuName = "Scriptable Objects/ErrorFinderData")]
public class ErrorFinderData : ScriptableObject
{
    [System.Serializable]
    public class SQLWord
    {
        public string word;
        public bool isError;
        public string correction; // what it should be
    }

    [Header("Puzzle Info")]
    public string puzzleTitle = "Find the SQL errors!";

    [TextArea(3, 6)]
    public string instruction;

    [Header("SQL Words")]
    public List<SQLWord> sqlWords;

    [Header("Reward")]
    [TextArea(2, 4)]
    public string rewardFragment;

    [Header("Feedback")]
    public string correctFeedback = "Found an error!";
    public string wrongFeedback = "That is correct SQL!";
    public string completedFeedback = "All errors found!";
}
