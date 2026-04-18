using UnityEngine;

/// <summary>
/// Pripoj na kazdy dekoracny objekt na stene.
/// Objekt sa zobrazí len ked gravitacia smeruje k tejto stene.
/// </summary>
public class WallDecoration : MonoBehaviour
{
    [Tooltip("Ktora stena toto patri.")]
    public WallIdentifier.WallSide side;

    void Start()
    {
        EnvironmentBehaviour env = FindAnyObjectByType<EnvironmentBehaviour>();
        if (env != null)
            env.onGravityChanged.AddListener(OnGravityChanged);

        // Nastav hned podla aktualnej gravitacie
        OnGravityChanged(Physics2D.gravity.normalized);
    }

    void OnDestroy()
    {
        EnvironmentBehaviour env = FindAnyObjectByType<EnvironmentBehaviour>();
        if (env != null)
            env.onGravityChanged.RemoveListener(OnGravityChanged);
    }

    void OnGravityChanged(Vector2 gravDir)
    {
        WallIdentifier.WallSide deadly = GravityToWallSide(gravDir);
        gameObject.SetActive(deadly == side);
    }

    WallIdentifier.WallSide GravityToWallSide(Vector2 dir)
    {
        if (Vector2.Dot(dir, Vector2.down)  > 0.7f) return WallIdentifier.WallSide.Bottom;
        if (Vector2.Dot(dir, Vector2.up)    > 0.7f) return WallIdentifier.WallSide.Top;
        if (Vector2.Dot(dir, Vector2.left)  > 0.7f) return WallIdentifier.WallSide.Left;
        if (Vector2.Dot(dir, Vector2.right) > 0.7f) return WallIdentifier.WallSide.Right;
        return WallIdentifier.WallSide.Bottom;
    }
}
