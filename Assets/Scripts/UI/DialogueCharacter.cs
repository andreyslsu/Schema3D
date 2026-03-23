using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueCharacter : MonoBehaviour
{
    public static DialogueCharacter Instance;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public Image characterImage;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public GameObject tapToContinueText;
    public Button panelButton;

    [Header("Character Info")]
    public string characterName = "Professor Schema";

    [Header("Character Sprites")]
    public Sprite moodNormal;
    public Sprite moodHappy;
    public Sprite moodSurprised;
    public Sprite moodThinking;
    public Sprite moodAngry;
    public Sprite moodSad;

    [Header("Dialogue Settings")]
    public float typeSpeed = 0.05f;

    // State
    private Queue<DialogueLine> dialogueQueue
        = new Queue<DialogueLine>();
    private bool isTyping = false;
    private bool isShowing = false;
    private string currentFullText = "";
    private Coroutine typingCoroutine;
    private System.Action onDialogueComplete;

    // =========================================
    // DIALOGUE DATA
    // =========================================

    [System.Serializable]
    public class DialogueLine
    {
        public string text;
        public Mood mood;

        public DialogueLine(
            string text,
            Mood mood = Mood.Normal)
        {
            this.text = text;
            this.mood = mood;
        }
    }

    public enum Mood
    {
        Normal,
        Happy,
        Surprised,
        Thinking,
        Angry,
        Sad
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (panelButton != null)
            panelButton.onClick.AddListener(OnPanelTapped);

        if (tapToContinueText != null)
            tapToContinueText.SetActive(false);

        if (nameText != null)
            nameText.text = characterName;
    }

    // =========================================
    // PUBLIC SHOW DIALOGUE
    // =========================================

    // Show single line
    public void ShowDialogue(
        string text,
        Mood mood = Mood.Normal,
        System.Action onComplete = null)
    {
        ShowDialogue(new List<DialogueLine>
        {
            new DialogueLine(text, mood)
        }, onComplete);
    }

    // Show multiple lines
    public void ShowDialogue(
        List<DialogueLine> lines,
        System.Action onComplete = null)
    {
        StopAllCoroutines();
        dialogueQueue.Clear();

        foreach (DialogueLine line in lines)
            dialogueQueue.Enqueue(line);

        onDialogueComplete = onComplete;

        // Pause game
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        dialoguePanel.SetActive(true);
        isShowing = true;

        ShowNextLine();
    }

    // =========================================
    // DIALOGUE FLOW
    // =========================================

    private void ShowNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueQueue.Dequeue();
        UpdateMood(line.mood);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(line.text));
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        currentFullText = text;
        dialogueText.text = "";

        if (tapToContinueText != null)
            tapToContinueText.SetActive(false);

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }

        isTyping = false;

        if (tapToContinueText != null)
            tapToContinueText.SetActive(true);
    }

    public void OnPanelTapped()
    {
        if (!isShowing) return;

        if (isTyping)
        {
            // Skip to full text
            StopCoroutine(typingCoroutine);
            isTyping = false;
            dialogueText.text = currentFullText;

            if (tapToContinueText != null)
                tapToContinueText.SetActive(true);
        }
        else
        {
            ShowNextLine();
        }
    }

    private void EndDialogue()
    {
        isShowing = false;
        dialoguePanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (tapToContinueText != null)
            tapToContinueText.SetActive(false);

        onDialogueComplete?.Invoke();
        onDialogueComplete = null;
    }

    // =========================================
    // MOOD
    // =========================================

    private void UpdateMood(Mood mood)
    {
        if (characterImage == null) return;

        switch (mood)
        {
            case Mood.Normal:
                if (moodNormal != null)
                    characterImage.sprite = moodNormal;
                break;
            case Mood.Happy:
                if (moodHappy != null)
                    characterImage.sprite = moodHappy;
                break;
            case Mood.Surprised:
                if (moodSurprised != null)
                    characterImage.sprite = moodSurprised;
                break;
            case Mood.Thinking:
                if (moodThinking != null)
                    characterImage.sprite = moodThinking;
                break;
            case Mood.Angry:
                if (moodAngry != null)
                    characterImage.sprite = moodAngry;
                break;
            case Mood.Sad:
                if (moodSad != null)
                    characterImage.sprite = moodSad;
                break;
        }
    }

    // =========================================
    // UTILITIES
    // =========================================

    public void HideDialogue()
    {
        StopAllCoroutines();
        isShowing = false;
        isTyping = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public bool IsShowing() => isShowing;
}