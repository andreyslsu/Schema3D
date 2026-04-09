using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Scriptable Objects/DialogueData")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class Line
    {
        [TextArea(2, 5)] 
        public string text;
        public DialogueCharacter.Mood mood;
    }

    public List<Line> lines = new List<Line>();
}
