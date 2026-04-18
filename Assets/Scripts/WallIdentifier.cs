using UnityEngine;
 
/// <summary>
/// Attach to each boundary wall. Set the WallSide in the Inspector to match
/// the wall's physical position (Bottom, Top, Left, Right).
/// EnvironmentBehaviour uses this to mark the wall opposite gravity as deadly.
/// </summary>
public class WallIdentifier : MonoBehaviour
{
    public enum WallSide { Bottom, Top, Left, Right }
 
    [Tooltip("Which side of the play area this wall sits on.")]
    public WallSide side;
}
 