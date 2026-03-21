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

    [Header("Keycard UI")]
    public GameObject keycardPanel;          // Panel showing keycard info
    public TextMeshProUGUI keycardText;      // Text inside keycard panel
    public GameObject keycardHotbarSlot;

    public Transform hotbarContainer;        // HotbarPanel transform
    public GameObject hotbarSlotPrefab;      // Fragment slot prefab
    public GameObject keycardSlotPrefab;     // Keycard slot prefab (separate design)

    // Tracks keycard slot separately
    private GameObject activeKeycardSlot;

    [Header("Interact Label")]
    public TextMeshProUGUI interactLabel;

    [Header("Pause Menu")]
    public GameObject pauseMenuPanel;

    [Header("Gameplay UI")]
    public GameObject hotbarCanvas;      // drag your hotbar canvas ✓
    public GameObject pauseButton;

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
        
    }

    // Show fragment clue panel
    public void ShowFragment(string clue)
    {
        fragmentPanel.SetActive(true);
        clueText.text = clue;
        HideInteractLabel(); // Hide label when panel opens 
    }

    // Close fragment panel
    public void CloseFragment()
    {
        fragmentPanel.SetActive(false);
        //Time.timeScale = 1f; // Resume game
    }

    public void ShowLaptop()
    {
        if (LaptopManager.Instance != null)
            LaptopManager.Instance.OpenLaptop();
        else
            Debug.LogWarning("LaptopManager not found!");

        HideInteractLabel(); // Hide label when laptop opens 
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
        // Only destroy fragment slots not keycard slot
        foreach (Transform child in hotbarContainer)
        {
            // Skip keycard slot when clearing
            if (activeKeycardSlot != null && child.gameObject == activeKeycardSlot)
                continue;

            Destroy(child.gameObject);
        }

        // Rebuild fragment slots
        foreach (string clue in InventoryManager.Instance.GetFragments())
        {
            GameObject slotObj = Instantiate(hotbarSlotPrefab, hotbarContainer);

            Button button = slotObj.GetComponent<Button>();
            TextMeshProUGUI text = slotObj.GetComponentInChildren<TextMeshProUGUI>();

            // Show short label on button
            // e.g "Fragment 1" or first 10 chars of clue
            text.text = clue.Length > 10 ? clue.Substring(0, 10) + "..." : clue;

            // Capture clue for button click
            string capturedClue = clue;
            button.onClick.AddListener(() => ShowFragment(capturedClue));
        }
    }

    public void ShowKeycardPanel()
    {
        if (keycardPanel != null)
        {
            keycardPanel.SetActive(true);
            Time.timeScale = 0f;

            if (keycardText != null)
                keycardText.text = "ACCESS KEYCARD\n\nFind the elevator\nand insert this keycard\nto proceed.";
        }
        else
            Debug.LogWarning("KeycardPanel is NULL!");
    }

    public void CloseKeycardPanel()
    {
        if (keycardPanel != null)
            keycardPanel.SetActive(false);

        Time.timeScale = 1f;
    }
    public void AddKeycardToHotbar()
    {
        // Dont add twice
        if (activeKeycardSlot != null) return;

        if (keycardSlotPrefab == null)
        {
            Debug.LogWarning("KeycardSlotPrefab is NULL!");
            return;
        }

        // Instantiate keycard slot in hotbar
        activeKeycardSlot = Instantiate(keycardSlotPrefab, hotbarContainer);

        Button button = activeKeycardSlot.GetComponent<Button>();
        TextMeshProUGUI text = activeKeycardSlot
            .GetComponentInChildren<TextMeshProUGUI>();

        // Set label
        if (text != null)
            text.text = "Keycard";

        // Open keycard panel on click
        if (button != null)
            button.onClick.AddListener(() => ShowKeycardPanel());

        Debug.Log("Keycard added to hotbar!");
    }

    // Destroys keycard slot from hotbar
    public void RemoveKeycardFromHotbar()
    {
        if (activeKeycardSlot != null)
        {
            Destroy(activeKeycardSlot);
            activeKeycardSlot = null;
            Debug.Log("Keycard removed from hotbar!");
        }
    }
    // show interact label need para ishow pag naka hover
    public void ShowInteractLabel(string text)
    {
        if (interactLabel != null)
        {
            interactLabel.gameObject.SetActive(true);
            interactLabel.text = text;
        }
    }

    public void HideInteractLabel()
    {
        if (interactLabel != null)
            interactLabel.gameObject.SetActive(false);
    }

    public void HideGameplayUI()
    {
        if (hotbarCanvas != null)
            hotbarCanvas.SetActive(false);

        if (pauseButton != null)
            pauseButton.SetActive(false);
    }

    public void ShowGameplayUI()
    {
        if (hotbarCanvas != null)
            hotbarCanvas.SetActive(true);

        if (pauseButton != null)
            pauseButton.SetActive(true);
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

    private void ResetState()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        
        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameplayUI();
    }
    public void RetryLevel()
    {
        ResetState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }
}