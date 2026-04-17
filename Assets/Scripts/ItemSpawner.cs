using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;

    [Header("PowerUps (spawn every 20s)")]
    public GameObject sekeraPrefab;
    public GameObject kliestePrefab;

    [Header("Spawn Area")]
    public Vector2 areaMin = new Vector2(-3f, -5f);
    public Vector2 areaMax = new Vector2(3f, 5f);

    private float _powerUpTimer = 20f;

    void Update()
    {
        _powerUpTimer -= Time.deltaTime;
        if (_powerUpTimer > 0f) return;

        _powerUpTimer = 20f;

        bool spawnSekera = Random.value > 0.5f;
        Spawn(spawnSekera ? sekeraPrefab : kliestePrefab);
    }

    void Spawn(GameObject prefab)
    {
        if (prefab == null) return;
        float x = Random.Range(areaMin.x, areaMax.x);
        float y = Random.Range(areaMin.y, areaMax.y);
        Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity);
    }
}
