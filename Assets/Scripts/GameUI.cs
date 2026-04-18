using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public PlayerController player;

    public TMP_Text scoreText;
    public Image itemImage;

    [Header("Health Bar")]
    public Image healthBarFill;
    public int maxHealth = 25;

    void Update()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + player.score;

        if (healthBarFill != null)
            healthBarFill.fillAmount = Mathf.Clamp01((float)player.health / maxHealth);

        if (itemImage != null)
        {
            itemImage.sprite = player.currentItemSprite;
            itemImage.enabled = player.currentItemSprite != null;
        }
    }
}
