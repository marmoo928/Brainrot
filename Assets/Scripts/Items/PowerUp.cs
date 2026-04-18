using UnityEngine;

public enum BreakType { Wood, Steel }

public class PowerUp : MonoBehaviour
{
    public BreakType canBreak;

    void Update()
    {
        Vector2 gravDir = Physics2D.gravity.normalized;
        float angle = Vector2.SignedAngle(Vector2.down, gravDir);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

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
