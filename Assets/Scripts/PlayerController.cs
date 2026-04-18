using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float forceMultiplier = -2f;
    public float maxForce = 20f;
    public float minSwipeDistance = 50f;

    [Header("Rotation Settings")]
    [Tooltip("How fast the sprite rotates to align with gravity. Set to 0 for instant snap.")]
    public float rotationSpeed = 10f;

    [Header("Wall Bounce")]
    [Tooltip("How much velocity is preserved after bouncing. 1 = full bounce, 0 = no bounce.")]
    [Range(0f, 1f)]
    public float bounciness = 0.8f;

    [Header("Deadly Wall")]
    [Tooltip("Damage dealt when the player hits the wall gravity pulls them toward.")]
    public int deadlyWallDamage = 5;
    [Tooltip("Cooldown in seconds between deadly-wall hits (independent of normal damage cooldown).")]
    public float deadlyWallDamageCooldown = 0.5f;

    [Header("Invincibility Frames")]
    [Tooltip("How long the player is invincible after taking any damage.")]
    public float iFrameDuration = 2f;

    private Rigidbody2D rb;
    private Vector2 swipeStart;
    private bool isSwiping = false;

    // Cached velocity from the frame BEFORE collision
    private Vector2 _velocityBeforeCollision;

    // Which wall side is currently deadly (set by EnvironmentBehaviour event)
    private WallIdentifier.WallSide _deadlyWallSide = WallIdentifier.WallSide.Bottom;
    private float _lastDeadlyWallHitTime = -999f;

    public int health = 25;
    public int score = 0;

    public BreakType? currentItem = null;
    public Sprite currentItemSprite = null;
    public bool isInvincible = false;

    // Tracked individually so StopAllCoroutines() is never needed
    private Coroutine _rotationCoroutine;
    private Coroutine _invincibilityCoroutine;
    private Coroutine _clearItemCoroutine;

    public void SetItem(BreakType type, Sprite sprite)
    {
        currentItem = type;
        currentItemSprite = sprite;
        if (_clearItemCoroutine != null) StopCoroutine(_clearItemCoroutine);
        _clearItemCoroutine = StartCoroutine(ClearItemRoutine());
    }

    public void ClearItem()
    {
        StopCoroutine(nameof(ClearItemRoutine));
        currentItem = null;
        currentItemSprite = null;
    }

    private System.Collections.IEnumerator ClearItemRoutine()
    {
        yield return new WaitForSeconds(5f);
        currentItem = null;
        currentItemSprite = null;
        _clearItemCoroutine = null;
    }

    [Header("UI")]
    public UnityEngine.UI.Image itemSlotUI;
    public BubbleEffect bubbleEffect;

    // -------------------------------------------------------------------------
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        EnvironmentBehaviour env = GetComponentInParent<EnvironmentBehaviour>();
        if (env != null)
        {
            env.onGravityChanged.AddListener(OnGravityChanged);
            env.onDeadlyWallChanged.AddListener(OnDeadlyWallChanged);
        }
        else
        {
            Debug.LogWarning("[PlayerController] No EnvironmentBehaviour found in parent.");
        }
    }

    void OnDestroy()
    {
        EnvironmentBehaviour env = GetComponentInParent<EnvironmentBehaviour>();
        if (env != null)
        {
            env.onGravityChanged.RemoveListener(OnGravityChanged);
            env.onDeadlyWallChanged.RemoveListener(OnDeadlyWallChanged);
        }
    }

    // -------------------------------------------------------------------------
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
            isSwiping = true;
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            isSwiping = false;
            Vector2 swipeEnd = Input.mousePosition;
            Vector2 swipeDelta = swipeEnd - swipeStart;

            if (swipeDelta.magnitude < minSwipeDistance) return;

            Vector2 direction = swipeDelta.normalized;
            float strength = Mathf.Min(swipeDelta.magnitude * forceMultiplier / 100f, maxForce);

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(direction * strength, ForceMode2D.Impulse);
        }
    }

    void FixedUpdate()
    {
        _velocityBeforeCollision = rb.linearVelocity;
    }

    // -------------------------------------------------------------------------
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Wall")) return;

        // --- Bounce ---
        Vector2 normal = Vector2.zero;
        for (int i = 0; i < collision.contactCount; i++)
            normal += collision.GetContact(i).normal;
        normal.Normalize();

        Vector2 reflected = Vector2.Reflect(_velocityBeforeCollision, normal) * bounciness;
        rb.linearVelocity = reflected;

        // --- Deadly wall check ---
        WallIdentifier wall = collision.gameObject.GetComponent<WallIdentifier>();
        if (wall != null && wall.side == _deadlyWallSide)
        {
            if (Time.time - _lastDeadlyWallHitTime >= deadlyWallDamageCooldown)
            {
                _lastDeadlyWallHitTime = Time.time;
                TakeDamage(deadlyWallDamage);
            }
        }
    }

    // -------------------------------------------------------------------------
    private void OnGravityChanged(Vector2 newGravityDirection)
    {
        float targetAngle = Vector2.SignedAngle(Vector2.down, newGravityDirection);
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        if (rotationSpeed <= 0f)
        {
            transform.rotation = targetRotation;
        }
        else
        {
            // Only stop and restart the rotation coroutine — nothing else
            if (_rotationCoroutine != null) StopCoroutine(_rotationCoroutine);
            _rotationCoroutine = StartCoroutine(RotateToRoutine(targetRotation));
        }
    }

    private void OnDeadlyWallChanged(WallIdentifier.WallSide deadlySide)
    {
        _deadlyWallSide = deadlySide;
        Debug.Log($"[PlayerController] Deadly wall is now: {deadlySide}");
    }

    private System.Collections.IEnumerator RotateToRoutine(Quaternion target)
    {
        while (Quaternion.Angle(transform.rotation, target) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        transform.rotation = target;
        _rotationCoroutine = null;
    }

    // -------------------------------------------------------------------------
    public void Heal(int amount)
    {
        health += amount;
        AudioController audio = AudioController.Instance;
        if (audio != null) audio.PlayHeal();
    }

    public void AddScore(int amount)
    {
        score += amount;
        AudioController audio = AudioController.Instance;
        if (audio != null) audio.PlayScoreItem();
    }

    public void CollectPowerupSound()
    {
        AudioController audio = AudioController.Instance;
        if (audio != null) audio.PlayPowerup();
    }

    public void PopBubbleSound()
    {
        AudioController audio = AudioController.Instance;
        if (audio != null) audio.PlayBubblePop();
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        health -= amount;

        AudioController audio = AudioController.Instance;
        if (audio != null) audio.PlayHurt();

        // Restart i-frames (cancel any existing ones first)
        if (_invincibilityCoroutine != null) StopCoroutine(_invincibilityCoroutine);
        _invincibilityCoroutine = StartCoroutine(InvincibilityRoutine());

        if (health <= 0)
            Die();
    }

    private System.Collections.IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        if (bubbleEffect != null) bubbleEffect.ShowBubble();
        yield return new WaitForSeconds(iFrameDuration);
        isInvincible = false;
        _invincibilityCoroutine = null;
        if (bubbleEffect != null) bubbleEffect.PopBubble();
        PopBubbleSound();
    }

    void Die()
    {
        Debug.Log("Player died");
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        gameObject.SetActive(false);
    }
}