using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SignInManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject signinPanel;
    public GameObject quit1Panel;
    public GameObject guestPanel;


    // =========================================
    // signin
    // =========================================
    public void OpenSignin()
    {
        signinPanel.SetActive(true);
    }

    public void CloseSignin()
    {
        signinPanel.SetActive(false);
    }

    public void LoadMain(string MainMenu)
    {
        SceneManager.LoadScene(MainMenu);
    }

    // =========================================
    // guest
    // =========================================
    public void OpenGuest()
    {
        guestPanel.SetActive(true);
    }

    public void CloseGuest()
    {
        guestPanel.SetActive(false);
    }

    // =========================================
    // QUIT
    // =========================================

    public void OpenQuit1()
    {
        quit1Panel.SetActive(true);
    }

    public void CloseQuit1()
    {
        quit1Panel.SetActive(false);
    }

    public void QuitGame1()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}