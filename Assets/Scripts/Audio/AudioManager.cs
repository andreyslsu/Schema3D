using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambienceSource;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip level3Music;
    public AudioClip bossMusic;
    public AudioClip levelCompleteMusic;
    public AudioClip gameOverMusic;

    [Header("UI Sound Effects")]
    public AudioClip buttonClickSFX;
    public AudioClip panelOpenSFX;
    public AudioClip panelCloseSFX;
    public AudioClip tabSwitchSFX;

    [Header("Gameplay Sound Effects")]
    public AudioClip fragmentPickupSFX;
    public AudioClip keycardReceiveSFX;
    public AudioClip keycardDeniedSFX;
    public AudioClip keycardAcceptedSFX;
    public AudioClip doorOpenSFX;
    public AudioClip doorCloseSFX;
    public AudioClip elevatorDingSFX;
    public AudioClip elevatorHumSFX;
    public AudioClip footstepSFX;

    [Header("Laptop Sound Effects")]
    public AudioClip laptopOpenSFX;
    public AudioClip laptopTypingSFX;
    public AudioClip laptopCorrectSFX;
    public AudioClip laptopWrongSFX;
    public AudioClip laptopShakerSFX;

    [Header("Minigame Sound Effects")]
    public AudioClip correctAnswerSFX;
    public AudioClip wrongAnswerSFX;
    public AudioClip puzzleCompleteSFX;
    public AudioClip buttonSelectSFX;
    public AudioClip matchCorrectSFX;
    public AudioClip matchWrongSFX;
    public AudioClip errorFoundSFX;

    [Header("Dialogue Sound Effects")]
    public AudioClip dialogueOpenSFX;
    public AudioClip dialogueCloseSFX;
    public AudioClip dialogueTypingSFX;
    public AudioClip dialogueNextSFX;

    [Header("Result Sound Effects")]
    public AudioClip starEarnSFX;
    public AudioClip scoreCountSFX;
    public AudioClip levelCompleteJingleSFX;
    public AudioClip gameOverJingleSFX;

    [Header("Ambience")]
    public AudioClip lobbyAmbienceSFX;
    public AudioClip serverRoomAmbienceSFX;
    public AudioClip officeAmbienceSFX;

    [Header("Music Settings")]
    public float musicFadeDuration = 1.5f;
    public float defaultMusicVolume = 0.5f;
    public float defaultSFXVolume = 1f;
    public float defaultAmbienceVolume = 0.3f;

    // State
    private float musicVolume = 0.5f;
    private float sfxVolume = 1f;
    private Coroutine fadeMusicCoroutine;

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
        // Load saved volumes 
        musicVolume = PlayerPrefs.GetFloat(
            "MasterVolume", defaultMusicVolume);
        sfxVolume = PlayerPrefs.GetFloat(
            "SFXVolume", defaultSFXVolume);

        ApplyVolumes();
    }

    // =========================================
    // MUSIC
    // =========================================

    public void PlayMusic(
        AudioClip clip,
        bool loop = true,
        bool fade = true)
    {
        if (clip == null) return;

        if (fade)
            StartFadeMusic(clip, loop);
        else
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public void StopMusic(bool fade = true)
    {
        if (fade)
        {
            if (fadeMusicCoroutine != null)
                StopCoroutine(fadeMusicCoroutine);
            fadeMusicCoroutine = StartCoroutine(
                FadeOutMusic());
        }
        else
            musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    private void StartFadeMusic(
        AudioClip newClip, bool loop)
    {
        if (fadeMusicCoroutine != null)
            StopCoroutine(fadeMusicCoroutine);

        fadeMusicCoroutine = StartCoroutine(
            CrossFadeMusic(newClip, loop));
    }

    private IEnumerator CrossFadeMusic(
        AudioClip newClip, bool loop)
    {
        // Fade out current 
        float elapsed = 0f;
        float startVolume = musicSource.volume;

        while (elapsed < musicFadeDuration * 0.5f)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(
                startVolume, 0f,
                elapsed / (musicFadeDuration * 0.5f));
            yield return null;
        }

        // Switch clip 
        musicSource.clip = newClip;
        musicSource.loop = loop;
        musicSource.Play();

        // Fade in new 
        elapsed = 0f;
        while (elapsed < musicFadeDuration * 0.5f)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(
                0f, musicVolume,
                elapsed / (musicFadeDuration * 0.5f));
            yield return null;
        }

        musicSource.volume = musicVolume;
    }

    private IEnumerator FadeOutMusic()
    {
        float elapsed = 0f;
        float startVolume = musicSource.volume;

        while (elapsed < musicFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(
                startVolume, 0f,
                elapsed / musicFadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = musicVolume;
    }

    // =========================================
    // SOUND EFFECTS
    // =========================================

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlaySFXAt(
        AudioClip clip, float volumeScale)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(
            clip, sfxVolume * volumeScale);
    }

    // =========================================
    // AMBIENCE
    // =========================================

    public void PlayAmbience(AudioClip clip)
    {
        if (clip == null) return;
        if (ambienceSource.clip == clip &&
            ambienceSource.isPlaying) return;

        ambienceSource.clip = clip;
        ambienceSource.loop = true;
        ambienceSource.volume =
            defaultAmbienceVolume;
        ambienceSource.Play();
    }

    public void StopAmbience()
    {
        ambienceSource.Stop();
    }

    // =========================================
    // VOLUME CONTROL
    // =========================================

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;
        PlayerPrefs.SetFloat(
            "MasterVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat(
            "SFXVolume", volume);
        PlayerPrefs.Save();
    }

    private void ApplyVolumes()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume;
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }

    // =========================================
    // SHORTCUT METHODS
    // =========================================

    //ui
    public void PlayButtonClick() =>
        PlaySFX(buttonClickSFX);
    public void PlayPanelOpen() =>
        PlaySFX(panelOpenSFX);
    public void PlayPanelClose() =>
        PlaySFX(panelCloseSFX);
    public void PlayTabSwitch() =>
        PlaySFX(tabSwitchSFX);

    // in game
    public void PlayFragmentPickup() =>
        PlaySFX(fragmentPickupSFX);
    public void PlayKeycardReceive() =>
        PlaySFX(keycardReceiveSFX);
    public void PlayKeycardDenied() =>
        PlaySFX(keycardDeniedSFX);
    public void PlayKeycardAccepted() =>
        PlaySFX(keycardAcceptedSFX);
    public void PlayDoorOpen() =>
        PlaySFX(doorOpenSFX);
    public void PlayDoorClose() =>
        PlaySFX(doorCloseSFX);
    public void PlayElevatorDing() =>
        PlaySFX(elevatorDingSFX);

// computer
    public void PlayLaptopOpen() =>
        PlaySFX(laptopOpenSFX);
    public void PlayLaptopCorrect() =>
        PlaySFX(laptopCorrectSFX);
    public void PlayLaptopWrong() =>
        PlaySFX(laptopWrongSFX);

// answers
    public void PlayCorrectAnswer() =>
        PlaySFX(correctAnswerSFX);
    public void PlayWrongAnswer() =>
        PlaySFX(wrongAnswerSFX);
    public void PlayPuzzleComplete() =>
        PlaySFX(puzzleCompleteSFX);
    public void PlayMatchCorrect() =>
        PlaySFX(matchCorrectSFX);
    public void PlayMatchWrong() =>
        PlaySFX(matchWrongSFX);
    public void PlayErrorFound() =>
        PlaySFX(errorFoundSFX);

    //dialogue
    public void PlayDialogueOpen() =>
        PlaySFX(dialogueOpenSFX);
    public void PlayDialogueClose() =>
        PlaySFX(dialogueCloseSFX);
    public void PlayDialogueNext() =>
        PlaySFX(dialogueNextSFX);

    //level complete
    public void PlayStarEarn() =>
        PlaySFX(starEarnSFX);
    public void PlayLevelCompleteJingle() =>
        PlaySFX(levelCompleteJingleSFX);
    public void PlayGameOverJingle() =>
        PlaySFX(gameOverJingleSFX);

    // =========================================
    // SCENE MUSIC HELPER
    // =========================================

    public void PlayMusicForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
                PlayMusic(mainMenuMusic);
                break;
            case "Level1":
                PlayMusic(level1Music);
                PlayAmbience(lobbyAmbienceSFX);
                break;
            case "Level2":
                PlayMusic(level2Music);
                PlayAmbience(serverRoomAmbienceSFX);
                break;
            case "Level3":
                PlayMusic(level3Music);
                PlayAmbience(officeAmbienceSFX);
                break;
            default:
                PlayMusic(level1Music);
                break;
        }
    }
}