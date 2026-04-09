using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Quest Panel")]
    public GameObject questPanel;              

    [Header("Quest 1 - Collect Fragments")]
    public GameObject quest1Panel;              
    public TextMeshProUGUI quest1CountText;    
    public int fragmentsRequired;              

    [Header("Quest 2 - Mini Games")]
    public GameObject quest2Panel;
    public TextMeshProUGUI quest2CountText;
    public int minigamesRequired = 1;
    private int minigamesCompleted = 0;

    [Header("Quest 3 - Use Laptop")]
    public GameObject quest3Panel;              

    private int fragmentsCollected = 0;        
    private bool quest1Complete = false;         
    private bool quest2Complete = false;         
    private bool quest3Complete = false;         

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {       
        if (quest1Panel != null)
            quest1Panel.SetActive(true);

        if (quest2Panel != null)
            quest2Panel.SetActive(true);
        // FALSE MUNA Q3
        if (quest3Panel != null)
            quest3Panel.SetActive(false);

        
        UpdateQuest1UI();

        questPanel.SetActive(false);

    }

    // =========================================
    // OPEN / CLOSE QUEST PANEL
    // =========================================

    public void ShowQuestPanel()
    {
        questPanel.SetActive(true);
    }

    public void CloseQuestPanel()
    {
        questPanel.SetActive(false);
    }

    // =========================================
    // QUEST 1 - COLLECT FRAGMENTS
    // =========================================

    public void OnFragmentCollected()
    {
        if (quest1Complete) return;

        fragmentsCollected++;

        UpdateQuest1UI();

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
        quest2CountText.color = Color.white;
    }

    private void CompleteQuest1()
    {
        quest1Complete = true;

        if (quest1CountText != null)
        {
            quest1CountText.text = fragmentsRequired + "/" + fragmentsRequired;
            quest1CountText.color = Color.green;
        }

        if (quest2Panel != null)
            quest2Panel.SetActive(true);

        ShowQuestPanel();
    }
    // =========================================
    // QUEST 2 - MINI GAMES
    // =========================================
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

        if (quest3Panel != null)
            quest3Panel.SetActive(true);

        ShowQuestPanel();

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

    public bool IsQuest2Complete()
    {
        return quest2Complete;
    }

    public int GetFragmentsCollected()
    {
        return fragmentsCollected;
    }
}