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

    private Rigidbody2D rb;
    private Vector2 swipeStart;
    private bool isSwiping = false;

    // Cached velocity from the frame BEFORE collision — so we always reflect a real value
    private Vector2 _velocityBeforeCollision;

    public int health = 25;
    public int score = 0;

    public BreakType? currentItem = null;
    public Sprite currentItemSprite = null;
    public bool isInvincible = false;

    public void SetItem(BreakType type, Sprite sprite)
    {
        currentItem = type;
        currentItemSprite = sprite;
        StopCoroutine(nameof(ClearItemRoutine));
        StartCoroutine(nameof(ClearItemRoutine));
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
    }

    [Header("UI")]
    public UnityEngine.UI.Image itemSlotUI;

    private float damageCooldown = 0.5f;
    private float lastDamageTime = -999f;

    // -------------------------------------------------------------------------
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        EnvironmentBehaviour env = GetComponentInParent<EnvironmentBehaviour>();
        if (env != null)
            env.onGravityChanged.AddListener(OnGravityChanged);
        else
            Debug.LogWarning("[PlayerController] No EnvironmentBehaviour found in parent.");
    }

    void OnDestroy()
    {
        EnvironmentBehaviour env = GetComponentInParent<EnvironmentBehaviour>();
        if (env != null)
            env.onGravityChanged.RemoveListener(OnGravityChanged);
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

    // Cache velocity every physics step BEFORE Unity resolves any collision
    void FixedUpdate()
    {
        _velocityBeforeCollision = rb.linearVelocity;
    }

    //-------------------------------------------------------------------------
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Wall")) return;
    
        // Average all contact normals (handles corners correctly)
        Vector2 normal = Vector2.zero;
        for (int i = 0; i < collision.contactCount; i++)
            normal += collision.GetContact(i).normal;
        normal.Normalize();
    
        // Reflect the PRE-collision velocity (not the post-collision zeroed one)
        Vector2 reflected = Vector2.Reflect(_velocityBeforeCollision, normal) * bounciness;
        rb.linearVelocity = reflected;
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
            StopAllCoroutines();
            StartCoroutine(RotateToRoutine(targetRotation));
        }
    }

    private System.Collections.IEnumerator RotateToRoutine(Quaternion target)
    {
        while (Quaternion.Angle(transform.rotation, target) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        transform.rotation = target;
    }

    // -------------------------------------------------------------------------
    public void Heal(int amount)
    {
        health += amount;
    }

    public void AddScore(int amount)
    {
        score += amount;
    }



    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        if (Time.time - lastDamageTime < damageCooldown)
            return;

        lastDamageTime = Time.time;
        health -= amount;

        if (health <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Player died");
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        gameObject.SetActive(false);
    }
}