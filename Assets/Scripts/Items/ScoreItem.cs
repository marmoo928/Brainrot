using UnityEngine;

public class ScoreItem : MonoBehaviour
{
    public int scoreValue = 10;

    private Collider2D _col;
    private bool _canPickUp = false;

    void Start()
    {
        _col = GetComponent<Collider2D>();
        if (_col != null) _col.enabled = false;
        Invoke(nameof(EnablePickup), 1f);
    }

    void Update()
    {
        Vector2 gravDir = Physics2D.gravity.normalized;
        float angle = Vector2.SignedAngle(Vector2.down, gravDir);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void EnablePickup()
    {
        _canPickUp = true;
        if (_col != null) _col.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_canPickUp) return;
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;
        player.AddScore(scoreValue);
        Destroy(gameObject);
    }
}
