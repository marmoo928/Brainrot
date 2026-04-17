public class ScoreItem : WorldObject, ISpawnable, ICollectible
{
    public int scoreValue = 10;
    public bool canBePickedUp = true;

    public void OnSpawn() {}

    public void OnCollect(PlayerController player)
    {
        if (!canBePickedUp) return;

        player.AddScore(scoreValue);
        Destroy(gameObject);
    }
}