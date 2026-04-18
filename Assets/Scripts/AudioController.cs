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

    [Header("Volume")]
    [Range(0f, 1f)] public float hurtVolume         = 1f;
    [Range(0f, 1f)] public float powerupVolume       = 1f;
    [Range(0f, 1f)] public float scoreItemVolume     = 1f;
    [Range(0f, 1f)] public float healVolume          = 1f;
    [Range(0f, 1f)] public float buttonPressVolume   = 1f;
    [Range(0f, 1f)] public float bubbleVolume        = 1f;
    [Range(0f, 1f)] public float gravityChangeVolume = 1f;

    [Header("Background Music")]
    public AudioClip musicClip;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    // ── Private ────────────────────────────────────────────────────────────────
    private AudioSource _source;
    private AudioSource _musicSource;

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
        _musicSource.clip = musicClip;
        _musicSource.loop = true;
        _musicSource.volume = musicVolume;
        _musicSource.playOnAwake = false;
        if (musicClip != null) _musicSource.Play();
    }


        EnvironmentBehaviour env = GetComponentInParent<EnvironmentBehaviour>();
        if (env != null)
            env.onGravityChanged.AddListener(OnGravityChanged);
    }

    /// <summary>Called by GameManager when the play button is pressed.</summary>
    public void NotifyGameStarted() => _gameStarted = true;

    public void PlayHurt()          => Play(hurtClips,          hurtVolume);
    public void PlayPowerup()       => Play(powerupClips,        powerupVolume);
    public void PlayScoreItem()     => Play(scoreItemClips,      scoreItemVolume);
    public void PlayHeal()          => Play(healClips,           healVolume);
    public void PlayButtonPress()   => Play(buttonPressClips,    buttonPressVolume);
    public void PlayBubblePop()     => Play(bubblePopClips,      bubbleVolume);
    public void PlayGravityChange() => Play(gravityChangeClips,  gravityChangeVolume);

    private void Play(AudioClip[] clips, float volume)
    {
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