using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Handles all UI panels: fragment clues & laptop
public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton

    [Header("Fragment UI")]
    public GameObject fragmentPanel;     // Panel for fragment clues
    public TextMeshProUGUI clueText;     // Text inside fragment panel

    [Header("Laptop UI")]
    public GameObject laptopPanel;       // Laptop coding panel
    public TMP_InputField laptopInput;   // Input field for typing code

    [Header("Hotbar UI")]
    public Transform hotbarContainer; // HotbarPanel transform
    public GameObject hotbarSlotPrefab; // The prefab TMP text

    private void Awake()
    {
        // Singleton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Hide all panels at start
        fragmentPanel.SetActive(false);
        laptopPanel.SetActive(false);
    }

    // Show fragment clue panel
    public void ShowFragment(string clue)
    {
        fragmentPanel.SetActive(true); // Activate panel
        clueText.text = clue;           // Set clue text
        Time.timeScale = 0f;            // Pause game
    }

    // Close fragment panel
    public void CloseFragment()
    {
        fragmentPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game
    }

    // Show laptop coding panel
    public void ShowLaptop()
    {
        laptopPanel.SetActive(true);
        Time.timeScale = 0f; // Pause game
    }

    // Close laptop panel
    public void CloseLaptop()
    {
        laptopPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    public void UpdateFragmentHotbar()
    {
        foreach (Transform child in hotbarContainer)
            Destroy(child.gameObject);

        foreach (string clue in InventoryManager.Instance.GetFragments())
        {
            GameObject slotObj = Instantiate(hotbarSlotPrefab, hotbarContainer);

            Button button = slotObj.GetComponent<Button>();
            TMPro.TextMeshProUGUI text = slotObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();

            // Short label for button
            text.text = clue.Length > 10 ? clue.Substring(0, 10) + "..." : clue;

            // Capture clue for button click
            string capturedClue = clue;

            button.onClick.AddListener(() =>
            {
                ShowFragment(capturedClue);
            });
        }
    }
}