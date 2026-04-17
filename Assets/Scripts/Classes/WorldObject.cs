using UnityEngine;

public abstract class WorldObject : MonoBehaviour
{
    protected void CreateVisual(GameObject prefab)
    {
        Instantiate(prefab, transform.position, Quaternion.identity, transform);
    }
}