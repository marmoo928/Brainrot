using UnityEngine;

/// <summary>
/// Singleton audio manager. Attach to a persistent GameObject (e.g. GameManager).
/// Assign clips in the Inspector. A random clip from each category is played on every call.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────────
    public static AudioController Instance { get; private set; }

    // ── Inspector slots ────────────────────────────────────────────────────────
    [Header("Player Feedback Clips")]
    [Tooltip("One random clip is picked each time the player takes damage.")]
    public AudioClip[] hurtClips;

    [Tooltip("One random clip is picked each time the player collects a powerup.")]
    public AudioClip[] powerupClips;

    [Tooltip("One random clip is picked each time the player collects a score item.")]
    public AudioClip[] scoreItemClips;

    [Tooltip("One random clip is picked each time the player collects a heal item.")]
    public AudioClip[] healClips;

    [Tooltip("One clip for when the Iframe Bubble pops")]
    public AudioClip[] bubblePopClip;

    [Tooltip("One clip for when the Gravity changes")]
    public AudioClip[] gravityChangeClip;


    [Header("Volume")]
    [Range(0f, 1f)] public float hurtVolume      = 1f;
    [Range(0f, 1f)] public float powerupVolume    = 1f;
    [Range(0f, 1f)] public float scoreItemVolume  = 1f;
    [Range(0f, 1f)] public float healVolume       = 1f;
    [Range(0f, 1f)] public float bubbleVolume       = 1f;
    [Range(0f, 1f)] public float gravityChangeVolume       = 1f;



    // ── Private ────────────────────────────────────────────────────────────────
    private AudioSource _source;

    // ── Unity lifecycle ────────────────────────────────────────────────────────
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

        EnvironmentBehaviour env = GetComponentInParent<EnvironmentBehaviour>();
        if (env != null)
        {
            env.onGravityChanged.AddListener(OnGravityChanged);
        }
        else
        {
            Debug.LogWarning("[PlayerController] No EnvironmentBehaviour found in parent.");
        }
    }

    // ── Public API ─────────────────────────────────────────────────────────────
    public void PlayHurt()      => Play(hurtClips,      hurtVolume);
    public void PlayPowerup()   => Play(powerupClips,   powerupVolume);
    public void PlayScoreItem() => Play(scoreItemClips, scoreItemVolume);
    public void PlayHeal()      => Play(healClips,      healVolume);
    public void PlayBubblePop() => Play(bubblePopClip, bubbleVolume);
    public void PlayGravityChange() => Play(gravityChangeClip, gravityChangeVolume);

    // ── Internal ───────────────────────────────────────────────────────────────
    private void Play(AudioClip[] clips, float volume)
    {
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning("[AudioController] Clip array is empty — assign clips in the Inspector.");
            return;
        }

        // Pick a random clip, avoiding a null entry if the array has gaps
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip == null)
        {
            Debug.LogWarning("[AudioController] Selected clip is null — check for empty slots in the array.");
            return;
        }

        _source.PlayOneShot(clip, volume);
    }

    private void OnGravityChanged(Vector2 newGravityDirection){
        PlayGravityChange();
    }
}