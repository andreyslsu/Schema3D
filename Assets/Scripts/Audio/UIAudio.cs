using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIAudio : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler
{
    [Header("Click Sound")]
    public bool playOnClick = true;
    public AudioClip clickSound;
    // Leave null to use default 

    [Header("Hover Sound")]
    public bool playOnHover = false;
    public AudioClip hoverSound;

    public void OnPointerClick(
        PointerEventData eventData)
    {
        if (!playOnClick) return;

        if (AudioManager.Instance == null)
            return;

        if (clickSound != null)
            AudioManager.Instance.PlaySFX(
                clickSound);
        else
            AudioManager.Instance.PlayButtonClick();
    }

    public void OnPointerEnter(
        PointerEventData eventData)
    {
        if (!playOnHover) return;

        if (AudioManager.Instance == null)
            return;

        if (hoverSound != null)
            AudioManager.Instance.PlaySFX(
                hoverSound);
    }
}