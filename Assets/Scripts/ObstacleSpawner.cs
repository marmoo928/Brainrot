using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attach to the "Obstacles" GameObject.
/// Spawns box obstacles that move at a fixed speed in the gravity direction.
/// Hooks into EnvironmentBehaviour.onGravityChanged — no per-frame Physics2D polling.
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefabs (optional)")]
    [Tooltip("Assign prefabs to use custom visuals. Leave empty to use generated boxes.")]
    public GameObject[] obstaclePrefabs;

    [Header("Spawn Count")]
    [Range(5, 12)]
    [Tooltip("Maximum number of obstacles alive at once.")]
    public int maxObstacleCount = 8;

    [Header("Spawn Timing")]
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 4f;

    [Header("Spawn Area Bounds")]
    [Tooltip("Bottom-left corner of the area obstacles spawn and travel within.")]
    public Vector2 areaMin = new Vector2(-3f, -5f);
    [Tooltip("Top-right corner of the area obstacles spawn and travel within.")]
    public Vector2 areaMax = new Vector2(3f, 5f);

    [Header("Movement")]
    [Tooltip("Speed at which obstacles travel in the gravity direction.")]
    public float obstacleSpeed = 3f;

    [Header("Box Size Range (generated boxes only)")]
    public Vector2 minBoxSize = new Vector2(0.5f, 0.5f);
    public Vector2 maxBoxSize = new Vector2(2f, 2f);

    [Header("Visuals (generated boxes only)")]
    public Color boxColor = new Color(1f, 0.4f, 0.1f, 1f);
    public bool randomizeColor = false;

    // -------------------------------------------------------------------------
    private Vector2 _currentGravityDir = Vector2.down;
    private float _spawnTimer;
    private int _lastPrefabIndex = -1;
    private float _lastSpawnT = -1f;

    private List<GameObject> obstacles = new List<GameObject>();
    // -------------------------------------------------------------------------

    void Start()
    {
        _spawnTimer = Random.Range(minSpawnDelay, maxSpawnDelay);

        EnvironmentBehaviour env = GetComponentInParent<EnvironmentBehaviour>();
        if (env != null)
            env.onGravityChanged.AddListener(OnGravityChanged);
        else
            Debug.LogWarning("[ObstacleSpawner] No EnvironmentBehaviour found in parent.");
    }

    void OnDestroy()
    {
        EnvironmentBehaviour env = GetComponentInParent<EnvironmentBehaviour>();
        if (env != null)
            env.onGravityChanged.RemoveListener(OnGravityChanged);
    }

    void OnGravityChanged(Vector2 newDir)
    {
        // Destroy all existing obstacles
        foreach (GameObject obs in obstacles)
        {
            if (obs != null)
                Destroy(obs);
        }

        obstacles.Clear();

        _currentGravityDir = newDir;
        _lastSpawnT = -1f; // reset lateral spread memory
    }

    // -------------------------------------------------------------------------
    void Update()
    {
        _spawnTimer -= Time.deltaTime;

        // Clean up null references (in case some were destroyed elsewhere)
        obstacles.RemoveAll(o => o == null);

        if (_spawnTimer <= 0f && obstacles.Count < maxObstacleCount)
        {
            SpawnObstacle();
            _spawnTimer = Random.Range(minSpawnDelay, maxSpawnDelay);
        }
    }

    // -------------------------------------------------------------------------
    void SpawnObstacle()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;

        GameObject go = Instantiate(PickPrefab(), transform);
        go.transform.position = GetSpawnPosition();

        obstacles.Add(go);

        ISpawnable spawnable = go.GetComponent<ISpawnable>();
        if (spawnable != null) spawnable.OnSpawn();

        ObstacleMover mover = go.GetComponent<ObstacleMover>();
        if (mover == null) mover = go.AddComponent<ObstacleMover>();

        mover.areaMin    = areaMin;
        mover.areaMax    = areaMax;
        mover.speed      = obstacleSpeed;
        mover.gravityDir = _currentGravityDir;
    }

    // -------------------------------------------------------------------------
    GameObject PickPrefab()
    {
        if (obstaclePrefabs.Length == 1) return obstaclePrefabs[0];

        int index;
        do { index = Random.Range(0, obstaclePrefabs.Length); }
        while (index == _lastPrefabIndex);

        _lastPrefabIndex = index;
        return obstaclePrefabs[index];
    }

    // -------------------------------------------------------------------------
    /// <summary>Spawns on the edge opposite to gravity direction.</summary>
    Vector3 GetSpawnPosition()
    {
        Vector2 spawnEdge = -_currentGravityDir;
        float t = PickLateralT();

        if (Mathf.Abs(spawnEdge.y) >= Mathf.Abs(spawnEdge.x))
        {
            float x = Mathf.Lerp(areaMin.x, areaMax.x, t);
            float y = spawnEdge.y > 0 ? areaMax.y : areaMin.y;
            return new Vector3(x, y, 0f);
        }
        else
        {
            float y = Mathf.Lerp(areaMin.y, areaMax.y, t);
            float x = spawnEdge.x > 0 ? areaMax.x : areaMin.x;
            return new Vector3(x, y, 0f);
        }
    }

    /// <summary>Picks a spawn T value that isn't too close to the last one.</summary>
    float PickLateralT()
    {
        const float minDistance = 0.3f;
        const int maxAttempts = 10;

        float t = 0f;
        for (int i = 0; i < maxAttempts; i++)
        {
            t = Random.Range(0.1f, 0.9f);
            if (_lastSpawnT < 0f || Mathf.Abs(t - _lastSpawnT) >= minDistance)
                break;
        }

        _lastSpawnT = t;
        return t;
    }

    // -------------------------------------------------------------------------
    // GameObject CreateGeneratedBox()
    // {
    //     GameObject go = new GameObject("Obstacle_Box");
    //     go.transform.SetParent(transform, true);

    //     float w = Random.Range(minBoxSize.x, maxBoxSize.x);
    //     float h = Random.Range(minBoxSize.y, maxBoxSize.y);
    //     go.transform.localScale = new Vector3(w, h, 1f);

    //     go.AddComponent<BoxCollider2D>();

    //     SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
    //     sr.sprite = CreateWhiteSquareSprite();
    //     sr.color  = randomizeColor
    //         ? new Color(Random.value, Random.value, Random.value)
    //         : boxColor;

    //     return go;
    // }

    // static Sprite CreateWhiteSquareSprite()
    // {
    //     Texture2D tex = new Texture2D(1, 1);
    //     tex.SetPixel(0, 0, Color.white);
    //     tex.Apply();
    //     return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    // }

    // -------------------------------------------------------------------------
    void OnDrawGizmosSelected()
    {
        Vector2 min = areaMin;
        Vector2 max = areaMax;

        Vector3 bl = new Vector3(min.x, min.y, 0f);
        Vector3 br = new Vector3(max.x, min.y, 0f);
        Vector3 tl = new Vector3(min.x, max.y, 0f);
        Vector3 tr = new Vector3(max.x, max.y, 0f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(bl, br); Gizmos.DrawLine(br, tr);
        Gizmos.DrawLine(tr, tl); Gizmos.DrawLine(tl, bl);

        float d = 0.08f;
        Gizmos.DrawSphere(bl, d);
        Gizmos.DrawSphere(br, d);
        Gizmos.DrawSphere(tl, d);
        Gizmos.DrawSphere(tr, d);

        Vector2 gravDir = Application.isPlaying ? _currentGravityDir : Vector2.down;
        Vector2 edge = -gravDir;

        Gizmos.color = Color.yellow;
        if (Mathf.Abs(edge.y) >= Mathf.Abs(edge.x))
        {
            float y = edge.y > 0 ? max.y : min.y;
            Gizmos.DrawLine(new Vector3(min.x, y), new Vector3(max.x, y));
        }
        else
        {
            float x = edge.x > 0 ? max.x : min.x;
            Gizmos.DrawLine(new Vector3(x, min.y), new Vector3(x, max.y));
        }
    }
}