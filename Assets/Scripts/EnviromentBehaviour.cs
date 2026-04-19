using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Randomly changes the gravity direction (up, down, left, right) at a configurable interval.
/// Also broadcasts which WallSide is now "deadly" (the wall gravity pulls the player toward).
/// Attach this script to any persistent GameObject in your scene (e.g. GameManager).
/// </summary>
public enum GravityMode { Off, UpDown, AllFourSequential, AllFourRandom }

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

    private static readonly Vector2[] AllDirections = { Vector2.down, Vector2.up, Vector2.left, Vector2.right };
    private static readonly Vector2[] UpDownDirections = { Vector2.down, Vector2.up };

    private Vector2 _currentDirection;
    private GravityMode _gravityMode = GravityMode.Off;
    private bool _isPaused = false;
    private int _sequentialIndex = 0;

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

            if (_isPaused || _gravityMode == GravityMode.Off) continue;

            ApplyGravity(PickNewDirection());
        }
    }

    private Vector2 PickNewDirection()
    {
        switch (_gravityMode)
        {
            case GravityMode.UpDown:
                return _currentDirection == Vector2.down ? Vector2.up : Vector2.down;

            case GravityMode.AllFourSequential:
                _sequentialIndex = (_sequentialIndex + 1) % AllDirections.Length;
                return AllDirections[_sequentialIndex];

            case GravityMode.AllFourRandom:
                Vector2 chosen;
                do { chosen = AllDirections[Random.Range(0, AllDirections.Length)]; }
                while (chosen == _currentDirection);
                return chosen;

            default:
                return _currentDirection;
        }
    }

    public void Configure(GravityMode mode, float strength, float minInt, float maxInt)
    {
        _gravityMode = mode;
        gravityStrength = strength;
        minInterval = minInt;
        maxInterval = maxInt;
        Physics2D.gravity = _currentDirection * gravityStrength;
    }

    public void SetPaused(bool paused)
    {
        _isPaused = paused;
    }

    /// <summary>Resets gravity to Vector2.down and restarts the shift timer. Call after Configure() on restart.</summary>
    public void ResetGravity()
    {
        StopAllCoroutines();
        _sequentialIndex = 0;
        ApplyGravity(Vector2.down);
        StartCoroutine(GravityShiftRoutine());
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