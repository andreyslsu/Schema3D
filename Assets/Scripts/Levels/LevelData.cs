using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public string levelName;                 
    public string nextLevelScene;           

    [Header("Laptop Puzzle")]
    [TextArea(5, 15)]
    public List<string> correctAnswers;     

    [Header("Quest Settings")]
    public int fragmentsRequired;

    [Header("Result Visualization")]  
    public Sprite resultImage;          
    [TextArea(2, 5)]
    public string resultTitle;          
    [TextArea(3, 8)]
    public string resultDescription;   
}
