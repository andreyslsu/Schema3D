using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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

    [Header("Quest 2 - Use Laptop")]
    public GameObject quest2Panel;              // Panel for laptop quest
                                                // Hidden until quest 1 complete

    // Private tracking variables
    private int fragmentsCollected = 0;          // Current fragment count
    private bool quest1Complete = false;         // True when all fragments collected
    private bool quest2Complete = false;         // True when laptop answer correct

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

        // Set initial count
        UpdateQuest1UI();

        // Auto open quest panel at start so player knows what to do
        questPanel.SetActive(true);

        StartCoroutine(AutoCloseQuestPanel());

    }
    private IEnumerator AutoCloseQuestPanel()
    {
        // Show for 3 seconds then auto close
        yield return new WaitForSeconds(3f);
        CloseQuestPanel();
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

    // Updates the count text e.g "0/3" "1/3" "2/3" "3/3"
    private void UpdateQuest1UI()
    {
        if (quest1CountText == null)
        {
            Debug.LogWarning("Quest1CountText is NULL! Drag it into QuestManager Inspector!");
            return;
        }

        // Update count text only
        // Label design is handled freely in Unity Inspector
        quest1CountText.text = fragmentsCollected + "/" + fragmentsRequired;
        quest1CountText.color = Color.white;
    }

    // Called when all fragments are collected
    private void CompleteQuest1()
    {
        quest1Complete = true;

        // Turn count text green to show completion
        if (quest1CountText != null)
        {
            quest1CountText.text = fragmentsRequired + "/" + fragmentsRequired;
            quest1CountText.color = Color.green;
        }

        // Show quest 2 panel now that quest 1 is done
        if (quest2Panel != null)
            quest2Panel.SetActive(true);

        // Auto open quest panel so player sees quest 2
        ShowQuestPanel();

        Debug.Log("Quest 1 complete! Quest 2 unlocked!");
    }

    // =========================================
    // QUEST 2 - USE LAPTOP
    // =========================================

    // Called by LaptopManager when correct answer submitted
    public void OnLaptopQuestComplete()
    {
        quest2Complete = true;

        Debug.Log("Quest 2 complete! Level done!");
    }

    // =========================================
    // GETTERS
    // =========================================

    // Returns whether quest 1 is complete
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