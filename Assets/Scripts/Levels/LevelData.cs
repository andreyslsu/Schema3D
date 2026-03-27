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
}
