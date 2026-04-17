public enum BreakType { Wood, Steel }

public class PowerUp : WorldObject, ICollectible
{
    public BreakType canBreak;

    public void OnCollect(PlayerController player)
    {
        player.ApplyPowerUp(this);
        Destroy(gameObject);
    }
}