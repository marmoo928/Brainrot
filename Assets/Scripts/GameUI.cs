using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public PlayerController player;

    public TMP_Text healthText;
    public TMP_Text scoreText;
    public Image itemImage;

    void Update()
    {
        if (healthText != null)
            healthText.text = "HP: " + player.health;

        if (scoreText != null)
            scoreText.text = "Score: " + player.score;

        if (itemImage != null)
        {
            itemImage.sprite = player.currentItemSprite;
            itemImage.enabled = player.currentItemSprite != null;
        }
    }
}
