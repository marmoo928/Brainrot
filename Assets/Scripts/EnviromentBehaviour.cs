using System.Collections;
using UnityEngine;

/// <summary>
/// Randomly changes the gravity direction (up, down, left, right) at a configurable interval.
/// Attach this script to any persistent GameObject in your scene (e.g. GameManager).
/// </summary>
public class EnvironmentBehaviour : MonoBehaviour
{
    [Header("Gravity Settings")]
    [Tooltip("Strength of gravity applied in the chosen direction.")]
    public float gravityStrength = 9.81f;

    [Header("Timing Settings")]
    [Tooltip("Minimum seconds before the next gravity change.")]
    public float minInterval = 8f;

    [Tooltip("Maximum seconds before the next gravity change.")]
    public float maxInterval = 20f;

    [Header("Events (optional)")]
    [Tooltip("Fired every time gravity changes. Passes the new direction as a Vector2.")]
    public UnityEngine.Events.UnityEvent<Vector2> onGravityChanged;

    // The four possible gravity directions
    private static readonly Vector2[] Directions = new Vector2[]
    {
        Vector2.down,   // Normal gravity
        Vector2.up,     // Inverted gravity
        Vector2.left,   // Left gravity
        Vector2.right,  // Right gravity
    };

    private Vector2 _currentDirection;

    // -------------------------------------------------------------------------
    private void Start()
    {
        // Start with normal downward gravity
        ApplyGravity(Vector2.down);
        StartCoroutine(GravityShiftRoutine());
    }

    // -------------------------------------------------------------------------
    private IEnumerator GravityShiftRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            Vector2 newDirection = PickNewDirection();
            ApplyGravity(newDirection);
        }
    }

    // -------------------------------------------------------------------------
    /// <summary>Picks a random direction that is different from the current one.</summary>
    private Vector2 PickNewDirection()
    {
        Vector2 chosen;
        do
        {
            chosen = Directions[Random.Range(0, Directions.Length)];
        }
        while (chosen == _currentDirection);

        return chosen;
    }

    // -------------------------------------------------------------------------
    /// <summary>Applies the given direction as Unity's Physics2D gravity.</summary>
    private void ApplyGravity(Vector2 direction)
    {
        _currentDirection = direction;
        Physics2D.gravity = direction * gravityStrength;

        string label = DirectionLabel(direction);
        Debug.Log($"[EnvironmentBehaviour] Gravity changed → {label} ({Physics2D.gravity})");

        onGravityChanged?.Invoke(direction);
    }

    // -------------------------------------------------------------------------
    private static string DirectionLabel(Vector2 dir)
    {
        if (dir == Vector2.down)  return "DOWN";
        if (dir == Vector2.up)    return "UP";
        if (dir == Vector2.left)  return "LEFT";
        if (dir == Vector2.right) return "RIGHT";
        return dir.ToString();
    }

    // -------------------------------------------------------------------------
    /// <summary>
    /// Call this from other scripts (e.g. a UI button) to force an immediate
    /// gravity shift without waiting for the next timed interval.
    /// </summary>
    public void ForceGravityShift()
    {
        StopAllCoroutines();
        ApplyGravity(PickNewDirection());
        StartCoroutine(GravityShiftRoutine());
    }

    // -------------------------------------------------------------------------
    /// <summary>Restore standard gravity when the object is destroyed.</summary>
    private void OnDestroy()
    {
        Physics2D.gravity = new Vector2(0f, -9.81f);
    }
}