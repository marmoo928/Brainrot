using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    public GameObject linuxPrefab;
    public GameObject pikoPrefab;

    [Range(0, 100)]
    [Tooltip("Sanca ze vypadne piko (%). Zvysok je linux.")]
    public int pikoChance = 20;

    public BreakType type;

    [Header("Drop Launch")]
    [Tooltip("Sila s akou vyleti drop (Impulse). Ladi podla PPU prefabu.")]
    public float launchForce = 6f;
    [Tooltip("Nahodny rozptyl smeru v stupnoch.")]
    public float launchSpread = 20f;
    [Tooltip("O kolko jednotiek sa spawn posunie dopredu v smere letu.")]
    public float spawnOffset = 1.5f;

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
            // Smer = proti gravitacii
            Vector2 launchDir = -Physics2D.gravity.normalized;

            float spread = Random.Range(-launchSpread * 0.5f, launchSpread * 0.5f);
            launchDir = Quaternion.Euler(0f, 0f, spread) * launchDir;

            Vector3 spawnPos = transform.position + (Vector3)(launchDir * spawnOffset);
            GameObject spawned = Instantiate(drop, spawnPos, Quaternion.identity);

            Rigidbody2D dropRb = spawned.GetComponent<Rigidbody2D>();
            if (dropRb != null)
                dropRb.AddForce(launchDir * launchForce, ForceMode2D.Impulse);
        }

        player.ClearItem();
        Destroy(gameObject);
    }
}
