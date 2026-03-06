using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("UI References - Assign in Inspector")]
    public Slider volumeSlider;       // Drag your volume slider here
    public Slider sensitivitySlider;  // Drag your sensitivity slider here

    [Header("Default Values")]
    public float defaultVolume = 1f;
    public float defaultSensitivity = 2f;

    private void Awake()
    {
        // DontDestroyOnLoad keeps this alive between scene changes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persists across scenes
        }
        else
        {
            // Destroy duplicate if one already exists from previous scene
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Apply saved settings when game first starts
        ApplySavedSettings();
    }

    // Call this to apply saved PlayerPrefs values to everything
    public void ApplySavedSettings()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", defaultVolume);
        float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity);

        // Apply volume to game
        AudioListener.volume = savedVolume;

        // Apply sensitivity if controller exists in this scene
        if (FirstPersonController.Instance != null)
            FirstPersonController.Instance.RotationSpeed = savedSensitivity;

        Debug.Log("Applied - Volume: " + savedVolume + " Sens: " + savedSensitivity);
    }

    // Call this when settings panel opens to sync sliders to saved values
    public void SyncSlidersToSaved()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", defaultVolume);
        float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity);

        // This will show exactly what value is being read from PlayerPrefs
        Debug.Log("SyncSlidersToSaved - Volume: " + savedVolume + " Sensitivity: " + savedSensitivity);

        if (volumeSlider != null)
        {
            Debug.Log("Volume slider range: " + volumeSlider.minValue + " to " + volumeSlider.maxValue);
            volumeSlider.SetValueWithoutNotify(savedVolume);
        }
        else
            Debug.LogWarning("volumeSlider is NULL!");

        if (sensitivitySlider != null)
        {
            Debug.Log("Sensitivity slider range: " + sensitivitySlider.minValue + " to " + sensitivitySlider.maxValue);
            sensitivitySlider.SetValueWithoutNotify(savedSensitivity);
        }
        else
            Debug.LogWarning("sensitivitySlider is NULL!");
    }

    // Called by volume slider On Value Changed (Dynamic float)
    public void SetVolume(float value)
    {
        // Shows exactly what value slider is sending
        Debug.Log("SetVolume received: " + value);

        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();

        // Verify it actually saved
        Debug.Log("Volume verified in PlayerPrefs: " + PlayerPrefs.GetFloat("MasterVolume"));
    }

    // Called by sensitivity slider On Value Changed (Dynamic float)
    public void SetSensitivity(float value)
    {
        Debug.Log("SetSensitivity received: " + value);

        if (FirstPersonController.Instance != null)
            FirstPersonController.Instance.RotationSpeed = value;

        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();

        // Verify it actually saved
        Debug.Log("Sensitivity verified in PlayerPrefs: " + PlayerPrefs.GetFloat("MouseSensitivity"));

    }

    // Call this from UIManager and MainMenuManager when settings panel opens
    public void RegisterSliders(Slider volSlider, Slider sensSlider)
    {
        // Re-assign sliders since they change between scenes
        volumeSlider = volSlider;
        sensitivitySlider = sensSlider;

    }
}