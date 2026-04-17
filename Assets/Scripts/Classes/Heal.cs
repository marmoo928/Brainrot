public class Heal : WorldObject, ISpawnable, ICollectible
{
    public int amountToHeal = 20;

    public void OnSpawn() {}

    public void OnCollect(PlayerController player)
    {
        player.Heal(amountToHeal);
        Destroy(gameObject);
    }
}