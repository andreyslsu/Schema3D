using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMusicController : MonoBehaviour
{
    [Header("Music for this Scene")]
    public AudioClip sceneMusic;
    public AudioClip sceneAmbience;
    public bool fadeIn = true;

    private void Start()
    {
        if (AudioManager.Instance == null)
            return;

        if (sceneMusic != null)
            AudioManager.Instance.PlayMusic(
                sceneMusic, true, fadeIn);

        if (sceneAmbience != null)
            AudioManager.Instance.PlayAmbience(
                sceneAmbience);
    }
}