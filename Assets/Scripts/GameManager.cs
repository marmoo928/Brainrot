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
    public AudioClip voiceClip;
    public Sprite rewardSprite;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References — drag from scene root")]
    public PlayerController player;
    public ObstacleSpawner obstacleSpawner;   // drag Obstacles GameObject
    public ItemSpawner itemSpawner;           // drag Items GameObject
    public EnvironmentBehaviour environment;  // drag Environment/GameManager... or the object EnvironmentBehaviour is on

    [Header("Levels")]
    public LevelConfig[] levels;

    [Header("Transition UI")]
    public GameObject transitionPanel;
    public AudioSource voiceAudioSource;
    public RewardFlyIn[] rewardSlots;         // Sloty na motherboarde — kazdy ma RewardFlyIn komponent

    [Header("Doom Slots")]
    [Tooltip("Sloty ktore sa odomykaju postupne po ziskani zakladnych odmien.")]
    public GameObject[] doomSlots;
    [Tooltip("Po kolkom leveli sa zacnu odomykat doom sloty. Default=4 (po ziskani vsetkych 4 odmien).")]
    public int doomSlotsStartLevel = 4;

    [Header("Canvases")]
    public GameObject startMenuCanvas;   // drag StartMenu
    public GameObject hudCanvas;         // drag HUD

    // -------------------------------------------------------------------------
    private int _currentLevel = 0;
    private bool _inTransition = false;
    private bool _gameStarted = false;

    // -------------------------------------------------------------------------
    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        // Show start menu, hide HUD
        if (startMenuCanvas != null) startMenuCanvas.SetActive(true);
        if (hudCanvas != null)       hudCanvas.SetActive(false);

        ApplyLevel(0);

        // Pause after ApplyLevel so spawners are configured before being paused
        SetGameplayPaused(true);

        Debug.Log("[GameManager] Start menu shown, gameplay paused.");
    }

    void Update()
    {
        if (!_gameStarted || _inTransition) return;
        if (player == null || levels == null || levels.Length == 0) return;
        if (_currentLevel >= levels.Length - 1) return;

        if (player.score >= levels[_currentLevel + 1].scoreThreshold)
        {
            Debug.Log($"[GameManager] Level transition → {levels[_currentLevel + 1].levelName}");
            StartCoroutine(LevelTransition());
        }
    }

    // ── Called by PlayButtonScript ────────────────────────────────────────────
    public void StartGame()
    {
        if (_gameStarted) return;

        if (startMenuCanvas != null) startMenuCanvas.SetActive(false);
        if (hudCanvas != null)       hudCanvas.SetActive(true);

        SetGameplayPaused(false);
        _gameStarted = true;
        AudioController audioController = AudioController.Instance;
        if (audioController != null) audioController.NotifyGameStarted();
        Debug.Log("[GameManager] Game started.");
    }

    // ── Pause helper ──────────────────────────────────────────────────────────
    private void SetGameplayPaused(bool paused)
    {
        if (obstacleSpawner != null) obstacleSpawner.SetPaused(paused);
        else Debug.LogWarning("[GameManager] obstacleSpawner is null!");

        if (itemSpawner != null) itemSpawner.SetPaused(paused);
        else Debug.LogWarning("[GameManager] itemSpawner is null!");

        if (environment != null) environment.SetPaused(paused);
        else Debug.LogWarning("[GameManager] environment is null!");

        Debug.Log($"[GameManager] Gameplay paused={paused}");

        if (player != null) player.GetComponent<Rigidbody2D>().simulated = !paused;
        if (player != null) player.isInvincible = paused;
    }

    // ── Level Transition ──────────────────────────────────────────────────────
    private IEnumerator LevelTransition()
    {
        _inTransition = true;
        _currentLevel++;

        AudioController audio = AudioController.Instance;
        if (audio != null) audio.SetLevelTransition(true);

        SetGameplayPaused(true);
        obstacleSpawner.ClearAll();

        if (transitionPanel != null) transitionPanel.SetActive(true);

        // 3. Animacia odmeny na motherboarde + zvuk odmeny
        int rewardIndex = _currentLevel - 1;
        if (rewardSlots != null && rewardIndex < rewardSlots.Length && rewardSlots[rewardIndex] != null)
        {
            LevelConfig prev = levels[_currentLevel - 1];
            if (prev.rewardSprite != null)
                rewardSlots[rewardIndex].PlayFlyIn(prev.rewardSprite);
        }

        if (audio != null)
        {
            switch (rewardIndex)
            {
                case 0: audio.PlayRewardKabel();   break;
                case 1: audio.PlayRewardRamka();   break;
                case 2: audio.PlayRewardGrafika(); break;
                case 3: audio.PlayRewardCpu();     break;
            }
        }

        float waitTime = 3f;
        if (voiceAudioSource != null && levels[_currentLevel].voiceClip != null)
        {
            voiceAudioSource.clip = levels[_currentLevel].voiceClip;
            voiceAudioSource.Play();
            waitTime = levels[_currentLevel].voiceClip.length;
        }

        yield return new WaitForSeconds(waitTime);

        // Post hlaška z AudioController
        if (audio != null)
        {
            float postLength = audio.PlayLevelVoicePost(rewardIndex);
            if (postLength > 0f)
                yield return new WaitForSeconds(audio.levelVoicePostDelay + postLength);
        }

        if (transitionPanel != null) transitionPanel.SetActive(false);

        // Odomkni doom sloty postupne — jeden per level po doomSlotsStartLevel
        if (doomSlots != null)
        {
            int doomIndex = _currentLevel - doomSlotsStartLevel;
            if (doomIndex >= 0 && doomIndex < doomSlots.Length && doomSlots[doomIndex] != null)
                doomSlots[doomIndex].SetActive(true);
        }

        ApplyLevel(_currentLevel);
        SetGameplayPaused(false);

        if (audio != null) audio.SetLevelTransition(false);
        _inTransition = false;
    }

    // ── Apply Level ───────────────────────────────────────────────────────────
    private void ApplyLevel(int index)
    {
        LevelConfig cfg = levels[index];
        obstacleSpawner.Configure(cfg.obstacleSpeed, cfg.obstaclePrefabs);
        itemSpawner.Configure(cfg.itemPrefabs, cfg.itemSpawnInterval);
        environment.Configure(cfg.gravityMode, cfg.gravityStrength, cfg.gravityMinInterval, cfg.gravityMaxInterval);
    }
}