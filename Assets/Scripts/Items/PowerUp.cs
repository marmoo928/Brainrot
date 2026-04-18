using UnityEngine;

public enum BreakType { Wood, Steel }

public class PowerUp : MonoBehaviour
{
    public BreakType canBreak;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;
        Sprite sprite = GetComponent<SpriteRenderer>()?.sprite;
        player.SetItem(canBreak, sprite);
        player.CollectPowerupSound();
        Destroy(gameObject);
    }
}
