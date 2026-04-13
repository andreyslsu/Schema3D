using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("UI References - Assign in Inspector")]
    public Slider volumeSlider;       
    public Slider sensitivitySlider;   
    public Slider sfxVolumeSlider;

    [Header("Default Values")]
    public float defaultVolume = 1f;
    public float defaultSensitivity = 2f;
    public float defaultsfx = 2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        ApplySavedSettings();
    }

    public void ApplySavedSettings()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", defaultVolume);
        float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity);

        AudioListener.volume = savedVolume;

        if (FirstPersonController.Instance != null)
            FirstPersonController.Instance.RotationSpeed = savedSensitivity;

        Debug.Log("Applied - Volume: " + savedVolume + " Sens: " + savedSensitivity);
    }

    public void SyncSlidersToSaved()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", defaultVolume);
        float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity);

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

    public void SetVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance
                .SetMusicVolume(value);

        Debug.Log("SetVolume received: " + value);

        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();

        Debug.Log("Volume verified in PlayerPrefs: " + PlayerPrefs.GetFloat("MasterVolume"));
    }

    public void SetSensitivity(float value)
    {
        Debug.Log("SetSensitivity received: " + value);

        if (FirstPersonController.Instance != null)
            FirstPersonController.Instance.RotationSpeed = value;

        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();

        Debug.Log("Sensitivity verified in PlayerPrefs: " + PlayerPrefs.GetFloat("MouseSensitivity"));

    }
//sound effex
    public void SetSFXVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance
                .SetSFXVolume(value);

        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    public void RegisterSliders(Slider volSlider, Slider sensSlider)
    {
        volumeSlider = volSlider;
        sensitivitySlider = sensSlider;

    }

}