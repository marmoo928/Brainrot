using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    [Header("Player Feedback Clips")]
    public AudioClip[] hurtClips;
    public AudioClip[] powerupClips;
    public AudioClip[] scoreItemClips;
    public AudioClip[] healClips;

    [Header("UI Clips")]
    public AudioClip[] buttonPressClips;

    [Header("Environment Clips")]
    public AudioClip[] bubblePopClips;
    public AudioClip[] gravityChangeClips;

    [Header("Box Break Clips")]
    public AudioClip[] breakWoodClips;
    public AudioClip[] breakSteelClips;
    [Range(0f, 1f)] public float breakVolume = 1f;

    [Header("Reward Clips")]
    public AudioClip[] rewardKabelClips;
    public AudioClip[] rewardRamkaClips;
    public AudioClip[] rewardGrafikaClips;
    public AudioClip[] rewardCpuClips;

    [Header("Volume")]
    [Range(0f, 1f)] public float hurtVolume         = 1f;
    [Range(0f, 1f)] public float powerupVolume       = 1f;
    [Range(0f, 1f)] public float scoreItemVolume     = 1f;
    [Range(0f, 1f)] public float healVolume          = 1f;
    [Range(0f, 1f)] public float buttonPressVolume   = 1f;
    [Range(0f, 1f)] public float bubbleVolume        = 1f;
    [Range(0f, 1f)] public float gravityChangeVolume = 1f;
    [Range(0f, 1f)] public float rewardVolume        = 1f;

    [Header("Game Start")]
    [Tooltip("Zvuk ktory sa zapusti tesne pred spustenim hry (po kliknuti Play).")]
    public AudioClip gameStartClip;
    [Range(0f, 1f)] public float gameStartVolume = 1f;

    [Header("Background Music")]
    [Tooltip("Hudba ktora hraje na uvodnom menu.")]
    public AudioClip menuMusicClip;
    [Tooltip("Hudba ktora hraje pocas hry.")]
    public AudioClip gameMusicClip;
    [Range(0f, 1f)] public float menuMusicVolume = 0.5f;
    [Range(0f, 1f)] public float gameMusicVolume = 0.5f;

    // ── Private ────────────────────────────────────────────────────────────────
    private AudioSource _source;
    private AudioSource _musicSource;
    private bool _gameStarted = false;
    private bool _inLevelTransition = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;
        _musicSource.playOnAwake = false;

        // Spusti menu hudbu
        if (menuMusicClip != null)
        {
            _musicSource.clip = menuMusicClip;
            _musicSource.volume = menuMusicVolume;
            _musicSource.Play();
        }
    }

    private void Start()
    {
        EnvironmentBehaviour env = GetComponentInParent<EnvironmentBehaviour>();
        if (env != null)
            env.onGravityChanged.AddListener(OnGravityChanged);
    }

    /// <summary>Called by GameManager at start/end of level transition.</summary>
    public void SetLevelTransition(bool active) => _inLevelTransition = active;

    /// <summary>Called by GameManager when the play button is pressed.</summary>
    public void NotifyGameStarted()
    {
        _gameStarted = true;

        if (gameStartClip != null)
            _source.PlayOneShot(gameStartClip, gameStartVolume);

        // Prepni z menu hudby na game hudbu
        _musicSource.Stop();
        if (gameMusicClip != null)
        {
            _musicSource.clip = gameMusicClip;
            _musicSource.volume = gameMusicVolume;
            _musicSource.Play();
        }
    }

    public void PlayHurt()          => Play(hurtClips,          hurtVolume);
    public void PlayPowerup()       => Play(powerupClips,        powerupVolume);
    public void PlayScoreItem()     => Play(scoreItemClips,      scoreItemVolume);
    public void PlayHeal()          => Play(healClips,           healVolume);
    public void PlayButtonPress()   => Play(buttonPressClips,    buttonPressVolume);
    public void PlayBubblePop()     => Play(bubblePopClips,      bubbleVolume);
    public void PlayGravityChange() => Play(gravityChangeClips,  gravityChangeVolume);
    public void PlayBreakWood()     => Play(breakWoodClips,      breakVolume);
    public void PlayBreakSteel()    => Play(breakSteelClips,     breakVolume);
    public void PlayRewardKabel()   => Play(rewardKabelClips,    rewardVolume,  ignoreTransition: true);
    public void PlayRewardRamka()   => Play(rewardRamkaClips,    rewardVolume,  ignoreTransition: true);
    public void PlayRewardGrafika() => Play(rewardGrafikaClips,  rewardVolume,  ignoreTransition: true);
    public void PlayRewardCpu()     => Play(rewardCpuClips,      rewardVolume,  ignoreTransition: true);

    private void Play(AudioClip[] clips, float volume, bool ignoreTransition = false)
    {
        if (_inLevelTransition && !ignoreTransition) return;

        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning("[AudioController] Clip array is empty — assign clips in the Inspector.");
            return;
        }

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip == null)
        {
            Debug.LogWarning("[AudioController] Selected clip is null — check for empty slots in the array.");
            return;
        }

        _source.PlayOneShot(clip, volume);
    }

    // Only play the gravity sound after the game has actually started
    private void OnGravityChanged(Vector2 _)
    {
        if (!_gameStarted) return;
        PlayGravityChange();
    }
}