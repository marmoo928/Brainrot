using UnityEngine;

/// <summary>
/// Singleton audio manager. Attach to a persistent GameObject (e.g. GameManager).
/// Assign clips in the Inspector. Call Play* methods from anywhere via AudioController.Instance.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────────
    public static AudioController Instance { get; private set; }

    // ── Inspector slots ────────────────────────────────────────────────────────
    [Header("Player Feedback Clips")]
    [Tooltip("Played when the player takes damage (including deadly wall hits).")]
    public AudioClip hurtClip;

    [Tooltip("Played when the player picks up a powerup.")]
    public AudioClip powerupClip;

    [Tooltip("Played when the player collects a score item.")]
    public AudioClip scoreItemClip;

    [Tooltip("Played when the player collects a heal item.")]
    public AudioClip healClip;

    [Header("Volume")]
    [Range(0f, 1f)] public float hurtVolume     = 1f;
    [Range(0f, 1f)] public float powerupVolume   = 1f;
    [Range(0f, 1f)] public float scoreItemVolume = 1f;
    [Range(0f, 1f)] public float healVolume      = 1f;

    [Header("Background Music")]
    public AudioClip musicClip;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    // ── Private ────────────────────────────────────────────────────────────────
    private AudioSource _source;
    private AudioSource _musicSource;

    // ── Unity lifecycle ────────────────────────────────────────────────────────
    private void Awake()
    {
        // Enforce singleton
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

    // ── Public API ─────────────────────────────────────────────────────────────

    /// <summary>Play the hurt sound (damage / deadly wall).</summary>
    public void PlayHurt()      => Play(hurtClip,      hurtVolume);

    /// <summary>Play the powerup collection sound.</summary>
    public void PlayPowerup()   => Play(powerupClip,   powerupVolume);

    /// <summary>Play the score item collection sound.</summary>
    public void PlayScoreItem() => Play(scoreItemClip, scoreItemVolume);

    /// <summary>Play the heal item collection sound.</summary>
    public void PlayHeal()      => Play(healClip,      healVolume);

    // ── Internal ───────────────────────────────────────────────────────────────
    private void Play(AudioClip clip, float volume)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioController] Clip is null — assign it in the Inspector.");
            return;
        }
        _source.PlayOneShot(clip, volume);
    }
}