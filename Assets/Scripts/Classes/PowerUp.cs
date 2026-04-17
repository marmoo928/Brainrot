using UnityEngine;

public enum BreakType { Wood, Steel }

public class PowerUp : WorldObject, ICollectible
{
    public BreakType canBreak;

    public void OnCollect(PlayerController player)
    {
        player.currentItem = canBreak;
        Destroy(gameObject);
    }
}
