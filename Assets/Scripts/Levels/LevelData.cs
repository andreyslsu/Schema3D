using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public string levelName;                 // e.g "Level 1"
    public string nextLevelScene;            // Scene name of next level

    [Header("Laptop Puzzle")]
    [TextArea(5, 15)]
    public List<string> correctAnswers;      // Accepted answers for this level

    [Header("Quest Settings")]
    public int fragmentsRequired;

    [Header("Result Visualization")]  // NEW 
    public Sprite resultImage;         // drag your table image 
    [TextArea(2, 5)]
    public string resultTitle;         // e.g "Table Created!" 
    [TextArea(3, 8)]
    public string resultDescription;   // explanation text 
}
