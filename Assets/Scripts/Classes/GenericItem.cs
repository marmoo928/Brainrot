using UnityEngine;

public class GenericItem : WorldObject, ISpawnable
{
    public GameObject prefab;

    public void OnSpawn() { CreateVisual(prefab); }
}
