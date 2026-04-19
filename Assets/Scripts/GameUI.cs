using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public PlayerController player;

    public TMP_Text scoreText;
    public Image itemImage;
    public TMP_Text itemTimerText;

    [Header("Health Bar")]
    public Image healthBarFill;

    void Update()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + player.score;

        if (healthBarFill != null)
            healthBarFill.fillAmount = Mathf.Clamp01((float)player.health / player.maxHealth);

        if (itemImage != null)
        {
            itemImage.sprite = player.currentItemSprite;
            itemImage.enabled = player.currentItemSprite != null;
        }

        if (itemTimerText != null)
        {
            if (player.itemTimeRemaining > 0f)
            {
                itemTimerText.text = Mathf.CeilToInt(player.itemTimeRemaining).ToString();
                itemTimerText.enabled = true;
            }
            else
            {
                itemTimerText.enabled = false;
            }
        }
    }
}
