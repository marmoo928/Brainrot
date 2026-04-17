public enum BoxType { Wood, Steel }

public class BreakableBox : WorldObject, ISpawnable
{
    public BoxType type;
    // public MonoBehaviour containedItem; // must implement ICollectible

    public void OnSpawn()
    {
        // spawn logic
    }

    // public void Break(PlayerController player)
    // {
    //     if (containedItem is ICollectible collectible)
    //     {
    //         collectible.OnCollect(player);
    //     }

    //     Destroy(gameObject);
    // }
}