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
    private float _spawnInterval = 20f;
    private bool _isPaused = true;
    private List<GameObject> _spawnedItems = new List<GameObject>();
    private GameObject[] _activePrefabs;

    void Awake()
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
        foreach (GameObject item in _spawnedItems)
            if (item != null) Destroy(item);
        _spawnedItems.Clear();

        foreach (var obj in FindObjectsByType<ScoreItem>(FindObjectsSortMode.None))
            Destroy(obj.gameObject);
        foreach (var obj in FindObjectsByType<Heal>(FindObjectsSortMode.None))
            Destroy(obj.gameObject);
        foreach (var obj in FindObjectsByType<PowerUp>(FindObjectsSortMode.None))
            Destroy(obj.gameObject);
    }

    public void SetPaused(bool paused) => _isPaused = paused;

    public void Configure(GameObject[] prefabs, float spawnInterval)
    {
        _activePrefabs = prefabs;
        _spawnInterval = spawnInterval;
        _powerUpTimer = spawnInterval;
    }

    void Update()
    {
        Debug.Log($"[ItemSpawner] isPaused={_isPaused}");
        if (_isPaused) return;

        _spawnedItems.RemoveAll(o => o == null);

        _powerUpTimer -= Time.deltaTime;
        if (_powerUpTimer > 0f) return;

        _powerUpTimer = _spawnInterval;

        if (_activePrefabs == null || _activePrefabs.Length == 0) return;
        Spawn(_activePrefabs[Random.Range(0, _activePrefabs.Length)]);
    }

    void Spawn(GameObject prefab)
    {
        if (prefab == null) return;
        float x = Random.Range(areaMin.x, areaMax.x);
        float y = Random.Range(areaMin.y, areaMax.y);
        GameObject go = Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity);

        Renderer r = go.GetComponent<Renderer>();
        if (r != null)
        {
            float hw = r.bounds.extents.x;
            float hh = r.bounds.extents.y;
            Vector3 pos = go.transform.position;
            pos.x = Mathf.Clamp(pos.x, areaMin.x + hw, areaMax.x - hw);
            pos.y = Mathf.Clamp(pos.y, areaMin.y + hh, areaMax.y - hh);
            go.transform.position = pos;
        }

        _spawnedItems.Add(go);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 bl = new Vector3(areaMin.x, areaMin.y, 0f);
        Vector3 br = new Vector3(areaMax.x, areaMin.y, 0f);
        Vector3 tl = new Vector3(areaMin.x, areaMax.y, 0f);
        Vector3 tr = new Vector3(areaMax.x, areaMax.y, 0f);
        Gizmos.DrawLine(bl, br); Gizmos.DrawLine(br, tr);
        Gizmos.DrawLine(tr, tl); Gizmos.DrawLine(tl, bl);

        float d = 0.08f;
        Gizmos.DrawSphere(bl, d); Gizmos.DrawSphere(br, d);
        Gizmos.DrawSphere(tl, d); Gizmos.DrawSphere(tr, d);
    }
}
