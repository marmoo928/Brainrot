using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach to the Button GameObject inside StartMenu.
/// Assign buttonImage, normalSprite, pressedSprite in the Inspector.
/// Make sure the Image on this GameObject has Raycast Target enabled.
/// </summary>
public class PlayButtonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Header("Sprites")]
    public UnityEngine.UI.Image buttonImage;
    public Sprite normalSprite;
    public Sprite pressedSprite;

    void Start()
    {
        // Confirm the script is alive and the GameManager can be found
        Debug.Log($"[PlayButtonScript] Ready. GameManager found: {GameManager.Instance != null}");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("[PlayButtonScript] OnPointerDown");
        if (buttonImage != null && pressedSprite != null)
            buttonImage.sprite = pressedSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("[PlayButtonScript] OnPointerUp");
        if (buttonImage != null && normalSprite != null)
            buttonImage.sprite = normalSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[PlayButtonScript] OnPointerClick — starting game");

        AudioController audio = AudioController.Instance;
        if (audio != null) audio.PlayButtonPress();

        if (GameManager.Instance != null)
            GameManager.Instance.StartGame();
        else
            Debug.LogError("[PlayButtonScript] GameManager.Instance is null!");
    }
}