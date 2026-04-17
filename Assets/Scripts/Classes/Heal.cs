using UnityEngine;

public class Heal : WorldObject, ISpawnable, ICollectible
{
    public int amountToHeal = 20;

    public void OnSpawn() { }

    public void OnCollect(PlayerController player)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) player.ShowItem(sr.sprite);

        player.Heal(amountToHeal);
        Destroy(gameObject);
    }
}
