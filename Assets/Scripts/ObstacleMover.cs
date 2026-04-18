using UnityEngine;

/// <summary>
/// Moves an obstacle at a fixed speed in the gravity direction.
/// Destroys itself when it reaches the opposite side of the spawn area.
/// </summary>
public class ObstacleMover : MonoBehaviour
{
    [HideInInspector] public Vector2 gravityDir = Vector2.down;
    [HideInInspector] public float speed = 3f;
    [HideInInspector] public Vector2 areaMin;
    [HideInInspector] public Vector2 areaMax;

    void Update()
    {
        float angle = Vector2.SignedAngle(Vector2.down, gravityDir);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        transform.Translate(gravityDir * speed * Time.deltaTime, Space.World);

        Vector2 pos = transform.position;

        // Pouzijeme skutocnu velkost spriteu aby box zmizol presne na hranici
        Renderer r = GetComponent<Renderer>();
        float halfW = r != null ? r.bounds.extents.x : 0f;
        float halfH = r != null ? r.bounds.extents.y : 0f;

        if (ShouldDestroy(pos, halfW, halfH))
            Destroy(gameObject);
    }

    bool ShouldDestroy(Vector2 pos, float halfW, float halfH)
    {
        // Destroy az ked cely box prejde za hranicu (zadna hrana)
        if (Vector2.Dot(gravityDir, Vector2.down) > 0.9f)
            return pos.y + halfH < areaMin.y;

        if (Vector2.Dot(gravityDir, Vector2.up) > 0.9f)
            return pos.y - halfH > areaMax.y;

        if (Vector2.Dot(gravityDir, Vector2.left) > 0.9f)
            return pos.x + halfW < areaMin.x;

        if (Vector2.Dot(gravityDir, Vector2.right) > 0.9f)
            return pos.x - halfW > areaMax.x;

        return false;
    }
}