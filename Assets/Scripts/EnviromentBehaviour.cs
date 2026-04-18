using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Randomly changes the gravity direction (up, down, left, right) at a configurable interval.
/// Also broadcasts which WallSide is now "deadly" (the wall gravity pulls the player toward).
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
    public UnityEvent<Vector2> onGravityChanged;

    [Tooltip("Fired every time gravity changes. Passes the WallSide the player will fall toward (i.e. the deadly wall).")]
    public UnityEvent<WallIdentifier.WallSide> onDeadlyWallChanged;

    // The four possible gravity directions
    private static readonly Vector2[] Directions = new Vector2[]
    {
        Vector2.down,   // Normal gravity  → Bottom wall is deadly
        Vector2.up,     // Inverted gravity → Top wall is deadly
        Vector2.left,   // Left gravity    → Left wall is deadly
        Vector2.right,  // Right gravity   → Right wall is deadly
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

        WallIdentifier.WallSide deadlySide = DirectionToDeadlyWall(direction);

        Debug.Log($"[EnvironmentBehaviour] Gravity → {DirectionLabel(direction)} | Deadly wall: {deadlySide}");

        onGravityChanged?.Invoke(direction);
        onDeadlyWallChanged?.Invoke(deadlySide);
    }

    // -------------------------------------------------------------------------
    /// <summary>
    /// Maps a gravity direction to the wall the player will be pulled into.
    /// Down gravity → Bottom wall hurts, Up gravity → Top wall hurts, etc.
    /// </summary>
    private static WallIdentifier.WallSide DirectionToDeadlyWall(Vector2 dir)
    {
        if (dir == Vector2.down)  return WallIdentifier.WallSide.Bottom;
        if (dir == Vector2.up)    return WallIdentifier.WallSide.Top;
        if (dir == Vector2.left)  return WallIdentifier.WallSide.Left;
        if (dir == Vector2.right) return WallIdentifier.WallSide.Right;

        // Fallback for diagonal/custom gravity: pick the dominant axis
        if (Mathf.Abs(dir.y) >= Mathf.Abs(dir.x))
            return dir.y < 0 ? WallIdentifier.WallSide.Bottom : WallIdentifier.WallSide.Top;
        else
            return dir.x < 0 ? WallIdentifier.WallSide.Left : WallIdentifier.WallSide.Right;
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