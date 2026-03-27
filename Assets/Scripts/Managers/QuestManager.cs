using UnityEngine;
using TMPro;


// Manages quest progression and UI panels
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Quest Panel")]
    public GameObject questPanel;               // Main quest panel

    [Header("Quest 1 - Collect Fragments")]
    public GameObject quest1Panel;              // Panel for collect fragments quest
    public TextMeshProUGUI quest1CountText;     // Shows "0/3" count
    public int fragmentsRequired;               // Set per level in Inspector

    [Header("Quest 2 - Mini Games")]
    public GameObject quest2Panel;
    public TextMeshProUGUI quest2CountText;
    public int minigamesRequired = 1;
    private int minigamesCompleted = 0;

    [Header("Quest 3 - Use Laptop")]
    public GameObject quest3Panel;              // Panel for laptop quest

    // Private tracking variables
    private int fragmentsCollected = 0;          // Current fragment count
    private bool quest1Complete = false;         // True when all fragments collected
    private bool quest2Complete = false;         
    private bool quest3Complete = false;         

    private void Awake()
    {
        // Singleton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Quest 1 panel visible by default
        if (quest1Panel != null)
            quest1Panel.SetActive(true);

        // Quest 2 panel hidden until quest 1 complete
        if (quest2Panel != null)
            quest2Panel.SetActive(false);

        if (quest3Panel != null)
            quest3Panel.SetActive(false);

        // Set initial count
        UpdateQuest1UI();

        questPanel.SetActive(false); // to make sure its close bruh

    }

    // =========================================
    // OPEN / CLOSE QUEST PANEL
    // =========================================

    // Opens main quest panel
    public void ShowQuestPanel()
    {
        questPanel.SetActive(true);
    }

    // Closes main quest panel
    public void CloseQuestPanel()
    {
        questPanel.SetActive(false);
    }

    // =========================================
    // QUEST 1 - COLLECT FRAGMENTS
    // =========================================

    // Called by Fragment.cs every time player picks up a fragment
    public void OnFragmentCollected()
    {
        // Ignore if quest already complete
        if (quest1Complete) return;

        // Increment count
        fragmentsCollected++;

        // Update count UI
        UpdateQuest1UI();

        // Check if all collected
        if (fragmentsCollected >= fragmentsRequired)
            CompleteQuest1();

        Debug.Log("Fragment collected: " + fragmentsCollected + "/" + fragmentsRequired);
    }

    private void UpdateQuest1UI()
    {
        if (quest1CountText == null)
        {
            Debug.LogWarning("Quest1CountText is NULL! Drag it into QuestManager Inspector!");
            return;
        }

        quest1CountText.text = fragmentsCollected + "/" + fragmentsRequired;
        quest1CountText.color = Color.white;
    }

    // Called when all fragments are collected
    private void CompleteQuest1()
    {
        quest1Complete = true;

        // Turn count text green
        if (quest1CountText != null)
        {
            quest1CountText.text = fragmentsRequired + "/" + fragmentsRequired;
            quest1CountText.color = Color.green;
        }

        // Show quest 2 panel
        if (quest2Panel != null)
            quest2Panel.SetActive(true);

        ShowQuestPanel();
    }
    // quest 2 mini games
    public void OnMinigameCompleted()
    {
        if (quest2Complete) return;

        minigamesCompleted++;

        if (quest2CountText != null)
            quest2CountText.text =
                minigamesCompleted + "/" + minigamesRequired;

        if (minigamesCompleted >= minigamesRequired)
        {
            quest2Complete = true;

            if (quest2CountText != null)
                quest2CountText.color = Color.green;
        }

        // Show quest 2 panel
        if (quest3Panel != null)
            quest3Panel.SetActive(true);

        ShowQuestPanel();

        // Show laptop hint when all fragments found 
        LevelDialogue levelDialogue =
            FindFirstObjectByType<LevelDialogue>();
        if (levelDialogue != null)
            levelDialogue.ShowLaptopHint();
    }

    // =========================================
    // QUEST 3 - USE LAPTOP
    // =========================================
    public void OnLaptopQuestComplete()
    {
        quest3Complete = true;

        Debug.Log("Quest 3 complete! Level done!");
    }


    // =========================================
    // GETTERS
    // =========================================

    // pang return sa quests
    public bool IsQuest1Complete()
    {
        return quest1Complete;
    }

    // Returns whether quest 2 is complete
    public bool IsQuest2Complete()
    {
        return quest2Complete;
    }

    // Returns current fragment count
    public int GetFragmentsCollected()
    {
        return fragmentsCollected;
    }
}