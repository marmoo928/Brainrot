using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class LevelConfig
{
    public string levelName;
    public int scoreThreshold;

    [Header("Obstacles")]
    public GameObject[] obstaclePrefabs;
    public float obstacleSpeed = 2f;

    [Header("Gravity")]
    public GravityMode gravityMode = GravityMode.Off;
    public float gravityStrength = 9.81f;
    public float gravityMinInterval = 8f;
    public float gravityMaxInterval = 20f;

    [Header("Items")]
    public GameObject[] itemPrefabs;
    public float itemSpawnInterval = 10f;

    [Header("Transition")]
    public AudioClip voiceClip;       // placeholder — doplnit nadabing
    public Sprite rewardSprite;       // PNG suciastky pre motherboard
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public PlayerController player;
    public ObstacleSpawner obstacleSpawner;
    public ItemSpawner itemSpawner;
    public EnvironmentBehaviour environment;

    [Header("Levels")]
    public LevelConfig[] levels;

    [Header("Transition UI")]
    public GameObject transitionPanel;
    public AudioSource voiceAudioSource;
    public RewardFlyIn[] rewardSlots;         // Sloty na motherboarde — kazdy ma RewardFlyIn komponent

    // -------------------------------------------------------------------------
    private int _currentLevel = 0;
    private bool _inTransition = false;

    // -------------------------------------------------------------------------
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (transitionPanel != null) transitionPanel.SetActive(false);
        ApplyLevel(0);
    }

    void Update()
    {
        if (player == null) { Debug.LogWarning("[GameManager] Player je null!"); return; }
        if (levels == null || levels.Length == 0) { Debug.LogWarning("[GameManager] Levels nie su nastavene!"); return; }
        if (_inTransition) return;
        if (_currentLevel >= levels.Length - 1) return;

        Debug.Log($"[GameManager] Score: {player.score} | Next threshold: {levels[_currentLevel + 1].scoreThreshold} | Level: {_currentLevel}");

        if (player.score >= levels[_currentLevel + 1].scoreThreshold)
        {
            Debug.Log($"[GameManager] Level transition → {levels[_currentLevel + 1].levelName}");
            StartCoroutine(LevelTransition());
        }
    }

    // -------------------------------------------------------------------------
    private IEnumerator LevelTransition()
    {
        _inTransition = true;
        _currentLevel++;

        // 1. Pauza — zastav vsetko
        obstacleSpawner.SetPaused(true);
        itemSpawner.SetPaused(true);
        environment.SetPaused(true);
        obstacleSpawner.ClearAll();

        // 2. Zobraz panel
        if (transitionPanel != null) transitionPanel.SetActive(true);

        // 3. Animacia odmeny na motherboarde
        int rewardIndex = _currentLevel - 1;
        if (rewardSlots != null && rewardIndex < rewardSlots.Length && rewardSlots[rewardIndex] != null)
        {
            LevelConfig prev = levels[_currentLevel - 1];
            if (prev.rewardSprite != null)
                rewardSlots[rewardIndex].PlayFlyIn(prev.rewardSprite);
        }

        // 4. Prehraj nadabing (placeholder — AudioClip priradit v Inspektore)
        float waitTime = 3f;
        if (voiceAudioSource != null && levels[_currentLevel].voiceClip != null)
        {
            voiceAudioSource.clip = levels[_currentLevel].voiceClip;
            voiceAudioSource.Play();
            waitTime = levels[_currentLevel].voiceClip.length;
        }

        yield return new WaitForSeconds(waitTime);

        // 5. Skry panel a aplikuj novy level
        if (transitionPanel != null) transitionPanel.SetActive(false);

        ApplyLevel(_currentLevel);

        obstacleSpawner.SetPaused(false);
        itemSpawner.SetPaused(false);
        environment.SetPaused(false);

        _inTransition = false;
    }

    // -------------------------------------------------------------------------
    private void ApplyLevel(int index)
    {
        LevelConfig cfg = levels[index];

        obstacleSpawner.Configure(cfg.obstacleSpeed, cfg.obstaclePrefabs);
        itemSpawner.Configure(cfg.itemPrefabs, cfg.itemSpawnInterval);
        environment.Configure(cfg.gravityMode, cfg.gravityStrength, cfg.gravityMinInterval, cfg.gravityMaxInterval);
    }
}
