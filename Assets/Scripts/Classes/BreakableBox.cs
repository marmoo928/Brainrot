using UnityEngine;

public enum BoxType { Wood, Steel }

public class BreakableBox : WorldObject, ISpawnable
{
    public GameObject[] possibleItems;

    private BoxType type;

    public void OnSpawn()
    {
        type = (BoxType)Random.Range(0, 2);
    }

    public void Break(PlayerController player)
    {
        if (player.currentItem == null) return;
        if ((int)player.currentItem.Value != (int)type) return;

        if (possibleItems != null && possibleItems.Length > 0)
        {
            int index = Random.Range(0, possibleItems.Length);
            Vector2 offset = Random.insideUnitCircle * 5.5f;
            Instantiate(possibleItems[index], transform.position + (Vector3)offset, Quaternion.identity);
        }

        player.currentItem = null;
        Destroy(gameObject);
    }
}
