using UnityEngine;

public abstract class WorldObject : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
}