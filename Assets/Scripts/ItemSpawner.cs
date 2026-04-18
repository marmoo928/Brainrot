using UnityEngine;
using System.Collections.Generic;

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
    private List<GameObject> _spawnedItems = new List<GameObject>();

    void Start()
    {
        EnvironmentBehaviour env = GetComponentInParent<EnvironmentBehaviour>();
        if (env != null)
            env.onGravityChanged.AddListener(OnGravityChanged);
    }

    void OnDestroy()
    {
        EnvironmentBehaviour env = GetComponentInParent<EnvironmentBehaviour>();
        if (env != null)
            env.onGravityChanged.RemoveListener(OnGravityChanged);
    }

    void OnGravityChanged(Vector2 newDir)
    {
        // Zrus vsetky spawnute itemy
        foreach (GameObject item in _spawnedItems)
        {
            if (item != null) Destroy(item);
        }
        _spawnedItems.Clear();

        // Zrus aj volne lezace dropy zo sceny
        foreach (var obj in FindObjectsByType<ScoreItem>(FindObjectsSortMode.None))
            Destroy(obj.gameObject);
        foreach (var obj in FindObjectsByType<Heal>(FindObjectsSortMode.None))
            Destroy(obj.gameObject);
        foreach (var obj in FindObjectsByType<PowerUp>(FindObjectsSortMode.None))
            Destroy(obj.gameObject);
    }

    void Update()
    {
        _spawnedItems.RemoveAll(o => o == null);

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
        GameObject go = Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity);
        _spawnedItems.Add(go);
    }
}
