using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

// Handles all UI panels: fragment clues & laptop
public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton

    [Header("Fragment UI")]
    public GameObject fragmentPanel;     // Panel for fragment clues
    public TextMeshProUGUI clueText;     // Text inside fragment panel
    public TextMeshProUGUI feedbackText; // feedback for answer

    public float feedbackDuration = 4f; // duration of feedbackk 

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

    private List<string> correctAnswers = new List<string>
    {
    "www",
    "WWW"
    };

    public void CheckAnswer()
    {
        string playerSQL = laptopInput.text.Trim();
        playerSQL = System.Text.RegularExpressions.Regex.Replace(playerSQL, @"\s+", " "); // remove extra spaces

        bool correct = false;

        foreach (string ans in correctAnswers)
        {
            if (playerSQL.Equals(ans, System.StringComparison.OrdinalIgnoreCase))
            {
                correct = true;
                break;
            }
        }

        if (correct)
        {
            feedbackText.text = "Yessir! Level Complete.";
            feedbackText.color = Color.green;

            // Optional: Trigger next level or win
            // LevelManager.Instance.CompleteLevel();
        }
        else
        {
            feedbackText.text = "FAHHHH. Check your fragments.";
            feedbackText.color = Color.red;
        }
        // Start coroutine to hide feedback automatically
        StartCoroutine(HideFeedbackAfterTime());
    }
    private IEnumerator HideFeedbackAfterTime()
    {
        feedbackText.gameObject.SetActive(true); // ensure it’s visible
        yield return new WaitForSecondsRealtime(feedbackDuration);
        feedbackText.gameObject.SetActive(false);
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