using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerProfileManager : MonoBehaviour
{
    public static PlayerProfileManager Instance;

    [Header("Profile Panel")]
    public GameObject profilePanel;

    [Header("Input Fields")]
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;

    [Header("Dropdowns")]
    public TMP_Dropdown courseDropdown;
    public TMP_Dropdown yearLevelDropdown;

    [Header("Buttons")]
    public Button saveButton;
    public Button closeButton;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;

    [Header("Display")]
    public TextMeshProUGUI profileNameDisplay;
    // shows name in main menu corner 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Setup dropdowns 
        SetupDropdowns();

        // Wire buttons 
        if (saveButton != null)
            saveButton.onClick.AddListener(
                SaveProfile);

        if (closeButton != null)
            closeButton.onClick.AddListener(
                CloseProfile);

        // Hide panel 
        if (profilePanel != null)
            profilePanel.SetActive(false);

        // Hide feedback 
        HideFeedback();

        // Load existing data 
        LoadProfile();

        // Update display 
        UpdateProfileDisplay();
    }

    // =========================================
    // DROPDOWN SETUP
    // =========================================

    private void SetupDropdowns()
    {
        // Course dropdown 
        if (courseDropdown != null)
        {
            courseDropdown.ClearOptions();
            courseDropdown.AddOptions(
                new System.Collections.Generic
                .List<string>
            {
                "Select Course",
                "Information Technology"
            });
        }

        // Year level dropdown 
        if (yearLevelDropdown != null)
        {
            yearLevelDropdown.ClearOptions();
            yearLevelDropdown.AddOptions(
                new System.Collections.Generic
                .List<string>
            {
                "Select Year Level",
                "1st Year",
                "2nd Year",
                "3rd Year",
                "4th Year"
            });
        }
    }

    // =========================================
    // OPEN / CLOSE
    // =========================================

    public void OpenProfile()
    {
        if (profilePanel != null)
            profilePanel.SetActive(true);

        // Load latest data 
        LoadProfile();
        HideFeedback();
    }

    public void CloseProfile()
    {
        if (profilePanel != null)
            profilePanel.SetActive(false);
    }

    // =========================================
    // SAVE PROFILE
    // =========================================

    public void SaveProfile()
    {
        string firstName =
            firstNameInput?.text.Trim() ?? "";
        string lastName =
            lastNameInput?.text.Trim() ?? "";

        // Validate 
        if (string.IsNullOrEmpty(firstName))
        {
            ShowFeedback(
                "Please enter your first name!",
                Color.red);
            return;
        }

        if (string.IsNullOrEmpty(lastName))
        {
            ShowFeedback(
                "Please enter your last name!",
                Color.red);
            return;
        }

        if (courseDropdown != null &&
            courseDropdown.value == 0)
        {
            ShowFeedback(
                "Please select your course!",
                Color.red);
            return;
        }

        if (yearLevelDropdown != null &&
            yearLevelDropdown.value == 0)
        {
            ShowFeedback(
                "Please select your year level!",
                Color.red);
            return;
        }

        // Get dropdown values 
        string course = courseDropdown != null ?
            courseDropdown.options[
                courseDropdown.value].text : "";

        string yearLevel =
            yearLevelDropdown != null ?
            yearLevelDropdown.options[
                yearLevelDropdown.value].text : "";

        // Save to PlayerPrefs 
        PlayerPrefs.SetString(
            "firstName", firstName);
        PlayerPrefs.SetString(
            "lastName", lastName);
        PlayerPrefs.SetString(
            "course", course);
        PlayerPrefs.SetString(
            "yearLevel", yearLevel);
        PlayerPrefs.SetString(
            "fullName",
            firstName + " " + lastName);
        PlayerPrefs.SetInt(
            "profileSaved", 1);
        PlayerPrefs.Save();

        ShowFeedback(
            "Profile saved!", Color.green);

        // Update display 
        UpdateProfileDisplay();

        Debug.Log("Profile saved: " +
            firstName + " " + lastName);
    }

    // =========================================
    // LOAD PROFILE
    // =========================================

    public void LoadProfile()
    {
        if (firstNameInput != null)
            firstNameInput.text =
                PlayerPrefs.GetString(
                "firstName", "");

        if (lastNameInput != null)
            lastNameInput.text =
                PlayerPrefs.GetString(
                "lastName", "");

        // Set course dropdown 
        string savedCourse =
            PlayerPrefs.GetString("course", "");

        if (courseDropdown != null &&
            !string.IsNullOrEmpty(savedCourse))
        {
            for (int i = 0;
                i < courseDropdown.options.Count;
                i++)
            {
                if (courseDropdown.options[i]
                    .text == savedCourse)
                {
                    courseDropdown.value = i;
                    break;
                }
            }
        }

        // Set year dropdown 
        string savedYear =
            PlayerPrefs.GetString("yearLevel", "");

        if (yearLevelDropdown != null &&
            !string.IsNullOrEmpty(savedYear))
        {
            for (int i = 0;
                i < yearLevelDropdown.options.Count;
                i++)
            {
                if (yearLevelDropdown.options[i]
                    .text == savedYear)
                {
                    yearLevelDropdown.value = i;
                    break;
                }
            }
        }
    }

    // =========================================
    // DISPLAY UPDATE
    // =========================================

    public void UpdateProfileDisplay()
    {
        if (profileNameDisplay == null) return;

        string firstName =
            PlayerPrefs.GetString("firstName", "");
        string lastName =
            PlayerPrefs.GetString("lastName", "");
        string yearLevel =
            PlayerPrefs.GetString("yearLevel", "");

        if (string.IsNullOrEmpty(firstName) &&
            string.IsNullOrEmpty(lastName))
        {
            profileNameDisplay.text =
                "Guest Player";
            return;
        }

        profileNameDisplay.text =
            firstName + " " + lastName +
            "\n" + yearLevel + " - IT";
    }

    // =========================================
    // GETTERS
    // =========================================

    public string GetFullName()
    {
        return PlayerPrefs.GetString(
            "fullName", "Guest");
    }

    public string GetFirstName()
    {
        return PlayerPrefs.GetString(
            "firstName", "");
    }

    public string GetYearLevel()
    {
        return PlayerPrefs.GetString(
            "yearLevel", "");
    }

    public string GetCourse()
    {
        return PlayerPrefs.GetString(
            "course", "");
    }

    public bool HasProfile()
    {
        return PlayerPrefs.GetInt(
            "profileSaved", 0) == 1;
    }

    // =========================================
    // UTILITIES
    // =========================================

    private void ShowFeedback(
        string message, Color color)
    {
        if (feedbackText == null) return;
        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);
    }

    private void HideFeedback()
    {
        if (feedbackText != null)
            feedbackText.gameObject
                .SetActive(false);
    }
}