using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using StarterAssets;
using UnityEngine.SceneManagement;

// Handles all UI panels: fragment clues & laptop
public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton

    [Header("Fragment UI")]
    public GameObject fragmentPanel;     // Panel for fragment clues
    public TextMeshProUGUI clueText;     // Text inside fragment panel

    [Header("Hotbar UI")]
    public Transform hotbarContainer; // HotbarPanel transform
    public GameObject hotbarSlotPrefab; // The prefab TMP text

    [Header("Pause Menu")]
    public GameObject pauseMenuPanel;

    [Header("Settings Sliders")]
    public Slider volumeSlider;      // Drag pause menu volume slider here
    public Slider sensitivitySlider; // Drag pause menu sensitivity slider here

    private bool isPaused = false;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Time.timeScale = 1f;

        // Hide all panels at start
        fragmentPanel.SetActive(false);
       //laptopPanel.SetActive(false);

    }
    private void Start()
    {
        // Start score tracking when level loads
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.StartTracking();
        else
            Debug.LogWarning("ScoreManager not found!");
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

    public void ShowLaptop()
    {
        if (LaptopManager.Instance != null)
            LaptopManager.Instance.OpenLaptop();
        else
            Debug.LogWarning("LaptopManager not found!");
    }

    public void CloseLaptop()
    {
        if (LaptopManager.Instance != null)
            LaptopManager.Instance.CloseLaptop();
        else
            Debug.LogWarning("LaptopManager not found!");
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
    //Resume
    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;

        // Register and sync pause menu sliders when pausing
        if (SettingsManager.Instance != null)
        {
            // Register sliders first so SettingsManager knows which ones to update
            SettingsManager.Instance.RegisterSliders(volumeSlider, sensitivitySlider);
            // Then sync them to saved PlayerPrefs values
            SettingsManager.Instance.SyncSlidersToSaved();
        }
        else
            Debug.LogWarning("SettingsManager not found!");
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
    }

    public void SetVolume(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.SetVolume(value);
        else
            Debug.LogWarning("SettingsManager not found!");
    }

    public void SetSensitivity(float value)
    {
        // If this never prints, slider is not connected to UIManager
        Debug.Log("UIManager SetSensitivity called: " + value);

        if (SettingsManager.Instance != null)
            SettingsManager.Instance.SetSensitivity(value);
        else
            Debug.LogWarning("SettingsManager not found!");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with your scene name
    }
}