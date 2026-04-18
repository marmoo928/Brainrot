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

    [Header("Drop Spawn Bounds")]
    [Tooltip("Spawn sa clampne do tejto oblasti (rovnaka ako hracia plocha).")]
    public Vector2 boundsMin = new Vector2(-3.5f, -2.5f);
    public Vector2 boundsMax = new Vector2(3.5f, 2.5f);

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 bl = new Vector3(boundsMin.x, boundsMin.y, 0f);
        Vector3 br = new Vector3(boundsMax.x, boundsMin.y, 0f);
        Vector3 tl = new Vector3(boundsMin.x, boundsMax.y, 0f);
        Vector3 tr = new Vector3(boundsMax.x, boundsMax.y, 0f);
        Gizmos.DrawLine(bl, br); Gizmos.DrawLine(br, tr);
        Gizmos.DrawLine(tr, tl); Gizmos.DrawLine(tl, bl);
    }

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

            Vector3 raw = transform.position + (Vector3)(launchDir * spawnOffset);
            GameObject spawned = Instantiate(drop, raw, Quaternion.identity);

            Renderer r = spawned.GetComponent<Renderer>();
            float hw = r != null ? r.bounds.extents.x : 0f;
            float hh = r != null ? r.bounds.extents.y : 0f;
            spawned.transform.position = new Vector3(
                Mathf.Clamp(raw.x, boundsMin.x + hw, boundsMax.x - hw),
                Mathf.Clamp(raw.y, boundsMin.y + hh, boundsMax.y - hh),
                0f);

            Rigidbody2D dropRb = spawned.GetComponent<Rigidbody2D>();
            if (dropRb != null)
                dropRb.AddForce(launchDir * launchForce, ForceMode2D.Impulse);
        }

        AudioController audio = AudioController.Instance;
        if (audio != null)
        {
            if (type == BreakType.Wood)  audio.PlayBreakWood();
            else                         audio.PlayBreakSteel();
        }

        player.ClearItem();
        Destroy(gameObject);
    }
}
