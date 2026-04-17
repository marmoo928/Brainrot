using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;

    [Header("Heal Settings")]
    public GameObject healPrefab;
    public float healThreshold = 0.6f;
    public float minHealSpawnDelay = 5f;
    public float maxHealSpawnDelay = 10f;

    [Header("PowerUp Settings")]
    public GameObject[] powerUpPrefabs;
    public float minPowerUpSpawnDelay = 10f;
    public float maxPowerUpSpawnDelay = 20f;

    [Header("Spawn Area")]
    public Vector2 areaMin = new Vector2(-3f, -5f);
    public Vector2 areaMax = new Vector2(3f, 5f);

    [Header("Player Max Health")]
    public int maxHealth = 100;

    private float _healTimer;
    private float _powerUpTimer;

    void Start()
    {
        _healTimer = Random.Range(minHealSpawnDelay, maxHealSpawnDelay);
        _powerUpTimer = Random.Range(minPowerUpSpawnDelay, maxPowerUpSpawnDelay);
    }

    void Update()
    {
        float healthPercent = (float)player.health / maxHealth;

        if (healthPercent < healThreshold)
        {
            _healTimer -= Time.deltaTime;
            if (_healTimer <= 0f)
            {
                Spawn(healPrefab);
                _healTimer = Random.Range(minHealSpawnDelay, maxHealSpawnDelay);
            }
        }

        _powerUpTimer -= Time.deltaTime;
        if (_powerUpTimer <= 0f)
        {
            if (powerUpPrefabs != null && powerUpPrefabs.Length > 0)
                Spawn(powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)]);
            _powerUpTimer = Random.Range(minPowerUpSpawnDelay, maxPowerUpSpawnDelay);
        }
    }

    void Spawn(GameObject prefab)
    {
        if (prefab == null) return;
        float x = Random.Range(areaMin.x, areaMax.x);
        float y = Random.Range(areaMin.y, areaMax.y);
        Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity);
    }
}
