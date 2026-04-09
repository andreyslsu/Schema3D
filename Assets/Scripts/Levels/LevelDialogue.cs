using UnityEngine;
using System.Collections.Generic;

public class LevelDialogue : MonoBehaviour
{
    [Header("Characters")]
    public DialogueCharacter professor;
    // DITO PWEDE MAG ADD NG CHARACTERS

    [Header("Intro Dialogue")]
    public DialogueData introDialogue;

    [Header("Hint Dialogues")]
    public DialogueData fragmentHintDialogue;
    public DialogueData laptopHintDialogue;
    public DialogueData keycardHintDialogue;
    public DialogueData wrongAnswerDialogue;
    public DialogueData gameOverDialogue;

    [Header("Custom Dialogues")]
    // Add any extra dialogues here 
    // Just drag new DialogueData assets 
    public List<DialogueData> extraDialogues;

    [Header("Auto Trigger")]
    public bool showIntroOnStart = true;
    public float introDelay = 0.5f;

    // Shown flags
    private bool hasShownIntro = false;
    private bool hasShownFragmentHint = false;
    private bool hasShownLaptopHint = false;
    private bool hasShownKeycardHint = false;

    [Header("Title Card")]
    public TitleCard titleCard;
    public string locationName = "Floor 1";
    public string locationSubtitle = "The Lobby";

    private void Start()
    {
        if (professor == null)
            professor = FindFirstObjectByType<DialogueCharacter>();

        // Use direct reference instead of singleton 
        if (titleCard != null)
        {
            titleCard.Show(
                locationName,
                locationSubtitle,
                () =>
                {
                    if (showIntroOnStart)
                        Invoke("ShowIntroDialogue", introDelay);
                });
        }
        else
        {
            if (showIntroOnStart)
                Invoke("ShowIntroDialogue", introDelay);
        }
    }

    // =========================================
    // INTRO
    // =========================================

    public void ShowIntroDialogue()
    {
        if (hasShownIntro) return;
        hasShownIntro = true;
        if (professor == null || introDialogue == null) return;

        professor.ShowDialogue(introDialogue, () =>
        {
            // Start score after intro 
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.StartTracking();

            // Show quest panel 
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
        if (professor == null || fragmentHintDialogue == null) return;
        professor.ShowDialogue(fragmentHintDialogue);
    }

    public void ShowLaptopHint()
    {
        if (hasShownLaptopHint) return;
        hasShownLaptopHint = true;
        if (professor == null || laptopHintDialogue == null) return;

        professor.ShowDialogue(laptopHintDialogue, () =>
        {
            if (QuestManager.Instance != null)
                QuestManager.Instance.ShowQuestPanel();
        });
    }

    public void ShowKeycardHint()
    {
        if (hasShownKeycardHint) return;
        hasShownKeycardHint = true;
        if (professor == null || keycardHintDialogue == null) return;
        professor.ShowDialogue(keycardHintDialogue);
    }

    public void ShowWrongAnswerHint()
    {
        if (professor == null || wrongAnswerDialogue == null) return;
        professor.ShowDialogue(wrongAnswerDialogue);
    }

    public void ShowGameOverHint()
    {
        if (professor == null || gameOverDialogue == null) return;
        professor.ShowDialogue(gameOverDialogue);
    }

    // =========================================
    // CUSTOM DIALOGUES
    // =========================================

    
    public void ShowExtraDialogue(int index)
    {
        if (professor == null) return;
        if (index >= extraDialogues.Count) return;
        if (extraDialogues[index] == null) return;

        professor.ShowDialogue(extraDialogues[index]);
    }

    // Call with CHARACTER GAGAMITIN NEXT CHARACTER
    public void ShowCustomDialogue(
        DialogueCharacter character,
        DialogueData data,
        System.Action onComplete = null)
    {
        if (character == null || data == null) return;
        character.ShowDialogue(data, onComplete);
    }
}