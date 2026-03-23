using UnityEngine;
using System.Collections.Generic;

public class LevelDialogue : MonoBehaviour
{
    [Header("Character Reference")]
    public DialogueCharacter character;  // drag any character ✓

    [Header("Auto Trigger")]
    public bool showIntroOnStart = true;
    public float introDelay = 0.5f;

    // Shown flags
    private bool hasShownIntro = false;
    private bool hasShownFragmentHint = false;
    private bool hasShownLaptopHint = false;
    private bool hasShownKeycardHint = false;

    private void Start()
    {
        // Auto find character if not assigned
        if (character == null)
            character = FindFirstObjectByType<DialogueCharacter>();

        if (showIntroOnStart)
            Invoke("ShowIntroDialogue", introDelay);
    }

    // =========================================
    // INTRO
    // =========================================

    public void ShowIntroDialogue()
    {
        if (hasShownIntro) return;
        hasShownIntro = true;

        if (character == null) return;

        var lines = new List<DialogueCharacter.DialogueLine>
        {
            new DialogueCharacter.DialogueLine(
                "Welcome Detective! I am here to guide you!",
                DialogueCharacter.Mood.Happy),

            new DialogueCharacter.DialogueLine(
                "Find all DATA FRAGMENTS hidden in this level.",
                DialogueCharacter.Mood.Normal),

            new DialogueCharacter.DialogueLine(
                "Each fragment is a clue for the SQL query!",
                DialogueCharacter.Mood.Thinking),

            new DialogueCharacter.DialogueLine(
                "CONTROLS:\n" +
                "Move → Left Joystick\n" +
                "Look → Right Joystick\n" +
                "Interact → Tap highlighted objects",
                DialogueCharacter.Mood.Normal),

            new DialogueCharacter.DialogueLine(
                "Find fragments, solve the terminal, " +
                "get the keycard and reach the elevator!",
                DialogueCharacter.Mood.Happy),

            new DialogueCharacter.DialogueLine(
                "Good luck Detective! " +
                "The database is counting on you!",
                DialogueCharacter.Mood.Happy),
        };

        character.ShowDialogue(lines, () =>
        {
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.StartTracking();

            if (QuestManager.Instance != null)
                QuestManager.Instance.ShowQuestPanel();
        });
    }

    // =========================================
    // HINTS
    // =========================================

    public void ShowFirstFragmentHint()
    {
        if (hasShownFragmentHint) return;
        hasShownFragmentHint = true;
        if (character == null) return;

        var lines = new List<DialogueCharacter.DialogueLine>
        {
            new DialogueCharacter.DialogueLine(
                "Great find! Check your hotbar " +
                "at the bottom to review fragments!",
                DialogueCharacter.Mood.Happy),

            new DialogueCharacter.DialogueLine(
                "Keep searching for more!",
                DialogueCharacter.Mood.Normal),
        };

        character.ShowDialogue(lines);
    }

    public void ShowLaptopHint()
    {
        if (hasShownLaptopHint) return;
        hasShownLaptopHint = true;
        if (character == null) return;

        var lines = new List<DialogueCharacter.DialogueLine>
        {
            new DialogueCharacter.DialogueLine(
                "You have all the fragments!",
                DialogueCharacter.Mood.Surprised),

            new DialogueCharacter.DialogueLine(
                "Now find the TERMINAL and type " +
                "the correct SQL query!",
                DialogueCharacter.Mood.Thinking),
        };

        character.ShowDialogue(lines);
    }

    public void ShowKeycardHint()
    {
        if (hasShownKeycardHint) return;
        hasShownKeycardHint = true;
        if (character == null) return;

        var lines = new List<DialogueCharacter.DialogueLine>
        {
            new DialogueCharacter.DialogueLine(
                "You cracked it! The keycard is yours!",
                DialogueCharacter.Mood.Happy),

            new DialogueCharacter.DialogueLine(
                "Find the ELEVATOR and use " +
                "the keycard on the panel!",
                DialogueCharacter.Mood.Normal),
        };

        character.ShowDialogue(lines);
    }

    public void ShowWrongAnswerHint()
    {
        if (character == null) return;

        var lines = new List<DialogueCharacter.DialogueLine>
        {
            new DialogueCharacter.DialogueLine(
                "That is not quite right!",
                DialogueCharacter.Mood.Angry),

            new DialogueCharacter.DialogueLine(
                "Check your fragments again carefully!",
                DialogueCharacter.Mood.Normal),
        };

        character.ShowDialogue(lines);
    }

    public void ShowGameOverHint()
    {
        if (character == null) return;

        var lines = new List<DialogueCharacter.DialogueLine>
        {
            new DialogueCharacter.DialogueLine(
                "Oh no! You ran out of time!",
                DialogueCharacter.Mood.Sad),

            new DialogueCharacter.DialogueLine(
                "Don't worry! Give it another go!",
                DialogueCharacter.Mood.Normal),
        };

        character.ShowDialogue(lines);
    }
}