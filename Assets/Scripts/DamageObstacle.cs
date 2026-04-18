using UnityEngine;

public class DamageObstacle : MonoBehaviour
{
    public int damage = 10;

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null) return;

        player.TakeDamage(damage);
        Destroy(gameObject);
    }
}
