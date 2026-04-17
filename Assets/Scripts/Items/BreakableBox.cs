using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    public GameObject linuxPrefab;
    public GameObject pikoPrefab;

    [Range(0, 100)]
    [Tooltip("Sanca ze vypadne piko (%). Zvysok je linux.")]
    public int pikoChance = 20;

    public BreakType type;

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null) return;

        if (player.currentItem == null || player.currentItem.Value != type)
        {
            player.TakeDamage(5);
            return;
        }

        GameObject drop = Random.Range(0, 100) < pikoChance ? pikoPrefab : linuxPrefab;
        if (drop != null)
        {
            Vector2 offset = Random.insideUnitCircle * 1.5f;
            Instantiate(drop, transform.position + (Vector3)offset, Quaternion.identity);
        }

        player.currentItem = null;
        Destroy(gameObject);
    }
}
