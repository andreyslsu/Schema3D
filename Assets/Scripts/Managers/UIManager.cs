using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using StarterAssets;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // singleton

    [Header("Fragment UI")]
    public GameObject fragmentPanel;   
    public TextMeshProUGUI clueText;    

    [Header("Keycard UI")]
    public GameObject keycardPanel;          
    public TextMeshProUGUI keycardText;  

    public Transform hotbarContainer;       // pang scale ng hotbar
    public GameObject hotbarSlotPrefab;     // frag slot
    public GameObject keycardSlotPrefab;    // key slot

    
    private GameObject activeKeycardSlot;

    [Header("Interact Label")]
    public TextMeshProUGUI interactLabel;

    [Header("Pause Menu")]
    public GameObject pauseMenuPanel;

    [Header("Gameplay UI")]
    public GameObject hotbarCanvas;      
    public GameObject pauseButton;

    [Header("Settings Sliders")]
    public Slider volumeSlider;      
    public Slider sensitivitySlider; 

    private bool isPaused = false;

    private void Awake()
    {
       
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
    // =========================================
    // FRAGMENTS
    // =========================================
    public void ShowFragment(string clue)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPanelOpen();

        fragmentPanel.SetActive(true);
        clueText.text = clue;
        HideInteractLabel(); 
    }

    public void CloseFragment()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPanelClose();

        fragmentPanel.SetActive(false);
        //Time.timeScale = 1f; // Resume game
    }
    // =========================================
    // LAPTOP/ COMPUTER
    // =========================================
    public void ShowLaptop()
    {
        if (LaptopManager.Instance != null)
            LaptopManager.Instance.OpenLaptop();
        else
            Debug.LogWarning("LaptopManager not found!");

        HideInteractLabel(); // Hide label when panel opens
    }

    public void CloseLaptop()
    {
        if (LaptopManager.Instance != null)
            LaptopManager.Instance.CloseLaptop();
        else
            Debug.LogWarning("LaptopManager not found!");
    }

    // =========================================
    // HOTBAR SETTINGS ONLE
    // =========================================
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

    // =========================================
    // KEYCARDS
    // =========================================
    public void ShowKeycardPanel()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayKeycardReceive();

        if (keycardPanel != null)
        {
            keycardPanel.SetActive(true);
            Time.timeScale = 0f;

            if (keycardText != null)
                keycardText.text = "ACCESS KEYCARD\nFind the elevator\nand insert this keycard\nto proceed.";
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
        if (activeKeycardSlot != null) return;

        if (keycardSlotPrefab == null)
        {
            Debug.LogWarning("walang keycard");
            return;
        }

        activeKeycardSlot = Instantiate(keycardSlotPrefab, hotbarContainer);

        Button button = activeKeycardSlot.GetComponent<Button>();
        TextMeshProUGUI text = activeKeycardSlot
            .GetComponentInChildren<TextMeshProUGUI>();

        if (text != null)
            text.text = "Keycard";

        if (button != null)
            button.onClick.AddListener(() => ShowKeycardPanel());

        Debug.Log("added kkeycard to the hotbar");
    }

    public void RemoveKeycardFromHotbar()
    {
        if (activeKeycardSlot != null)
        {
            Destroy(activeKeycardSlot);
            activeKeycardSlot = null;
            Debug.Log("keycard removed from hotbar");
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

    // PANG RESUME
    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    // =========================================
    // PAUSE / RESUME
    // =========================================
    public void PauseGame()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPanelOpen();

        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;

        
        if (SettingsManager.Instance != null)
        {
            // register first
            SettingsManager.Instance.RegisterSliders(volumeSlider, sensitivitySlider);
            // Then sync to saved PlayerPrefs values
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
        // pag di nag print di connected to UI manager
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