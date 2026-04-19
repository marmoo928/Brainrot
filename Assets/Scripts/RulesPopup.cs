using UnityEngine;
using UnityEngine.EventSystems;

public class RulesPopup : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject popupPanel;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (popupPanel != null) popupPanel.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (popupPanel != null) popupPanel.SetActive(false);
    }
}
