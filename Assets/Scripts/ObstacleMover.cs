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
        transform.Translate(gravityDir * speed * Time.deltaTime, Space.World);

        Vector2 pos = transform.position;
        float margin = 0.1f; // small buffer

        if (ShouldDestroy(pos, margin))
        {
            Destroy(gameObject);
        }
    }

    bool ShouldDestroy(Vector2 pos, float margin)
    {
        // Check based on movement direction ONLY

        if (Vector2.Dot(gravityDir, Vector2.down) > 0.9f)
            return pos.y < areaMin.y - margin;

        if (Vector2.Dot(gravityDir, Vector2.up) > 0.9f)
            return pos.y > areaMax.y + margin;

        if (Vector2.Dot(gravityDir, Vector2.left) > 0.9f)
            return pos.x < areaMin.x - margin;

        if (Vector2.Dot(gravityDir, Vector2.right) > 0.9f)
            return pos.x > areaMax.x + margin;

        return false;
    }
}