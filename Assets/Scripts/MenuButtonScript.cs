using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Generic button with normal/pressed sprite swap.
/// Assign buttonImage, normalSprite, pressedSprite in the Inspector.
/// The actual OnClick action is set via the Button component's On Click () event.
/// </summary>
public class MenuButtonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Sprites")]
    public UnityEngine.UI.Image buttonImage;
    public Sprite normalSprite;
    public Sprite pressedSprite;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonImage != null && pressedSprite != null)
            buttonImage.sprite = pressedSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buttonImage != null && normalSprite != null)
            buttonImage.sprite = normalSprite;

        AudioController audio = AudioController.Instance;
        if (audio != null) audio.PlayButtonPress();
    }
}
