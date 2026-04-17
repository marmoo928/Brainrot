using UnityEngine;

public class Heal : MonoBehaviour
{
    public int amountToHeal = 20;

    private bool _canPickUp = false;

    void Start()
    {
        Invoke(nameof(EnablePickup), 0.5f);
    }

    void Update()
    {
        Vector2 gravDir = Physics2D.gravity.normalized;
        float angle = Vector2.SignedAngle(Vector2.down, gravDir);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void EnablePickup() => _canPickUp = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_canPickUp) return;
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;
        player.Heal(amountToHeal);
        Destroy(gameObject);
    }
}
